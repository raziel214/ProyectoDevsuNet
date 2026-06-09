# Contrato de Comunicación Asíncrona (RabbitMQ)

Define el contrato de mensajería entre **ms-clientes** (productor) y **ms-cuentas**
(consumidor). El objetivo es que ms-cuentas mantenga una **réplica local de solo
lectura** de los clientes, para poder:

1. Validar el `clienteId` al crear una cuenta (F1) sin llamada síncrona.
2. Resolver el **nombre del cliente** en el reporte de estado de cuenta (F4).

Esto desacopla los microservicios: si ms-clientes está caído, ms-cuentas sigue
operando con su réplica (consistencia eventual).

---

## Topología AMQP

| Recurso        | Nombre                              | Notas |
|----------------|-------------------------------------|-------|
| Exchange       | `devsu.clientes.exchange`           | Tipo `topic`, durable |
| Queue          | `devsu.cuentas.cliente-sync.queue`  | Durable, en ms-cuentas |
| Routing keys   | `cliente.created`, `cliente.updated`, `cliente.deleted` | |
| Binding        | `cliente.*` → queue                 | ms-cuentas escucha todos los eventos de cliente |
| Dead Letter    | `devsu.cuentas.cliente-sync.dlq`    | Mensajes fallidos tras reintentos |
| vhost          | `itilsupport` (infra local)         | Ver infra/ para entorno de entrega |

> En local se usa la instancia existente `itil_rabbitmq`
> (host `localhost:5673`, user `admin`, vhost `itilsupport`).
> Alias interno de red: `rabbitmq:5672`.

---

## Esquema del mensaje: `ClienteEvent`

Publicado por ms-clientes en cada alta/cambio/baja de un cliente.

```json
{
  "eventType": "CREATED",
  "clienteId": "mmontalvo",
  "nombre": "Marianela Montalvo",
  "identificacion": "0102030406",
  "estado": true,
  "timestamp": "2022-02-10T10:15:30Z"
}
```

| Campo            | Tipo                | Descripción |
|------------------|---------------------|-------------|
| `eventType`      | enum CREATED / UPDATED / DELETED | Tipo de evento de dominio |
| `clienteId`      | string              | Identificador de negocio (clave de la réplica) |
| `nombre`         | string              | Nombre del cliente (usado en el reporte F4) |
| `identificacion` | string              | Documento de identidad |
| `estado`         | boolean             | Estado del cliente |
| `timestamp`      | ISO-8601 (UTC)      | Momento del evento |

En `DELETED` solo se garantiza `eventType`, `clienteId` y `timestamp`.

---

## Réplica local en ms-cuentas

Tabla `cliente_ref` (read-model, NO es la fuente de verdad):

| Columna        | Tipo        |
|----------------|-------------|
| cliente_id     | varchar PK  |
| nombre         | varchar     |
| identificacion | varchar     |
| estado         | boolean     |
| actualizado_en | timestamp   |

- `CREATED` / `UPDATED` → `upsert` por `cliente_id`.
- `DELETED` → marca inactivo o elimina la fila (a definir; por defecto inactivar).

---

## Garantías y manejo de errores

- **Idempotencia**: el consumidor hace `upsert`, por lo que reprocesar un
  mensaje no genera inconsistencias.
- **Reintentos**: 3 intentos; al agotar, el mensaje va a la DLQ
  `devsu.cuentas.cliente-sync.dlq`.
- **Serialización**: JSON (Jackson). `Content-Type: application/json`.
