# Database-per-Service y Consistencia Eventual — Defensa

Documento de defensa de las decisiones de datos del proyecto. Pensado para
sustentar la solución en la entrevista técnica.

---

## 1. La decisión: cada microservicio, su propia base de datos

```
PostgreSQL (mismo servidor en dev)
├── devsu_clientes   → ms-clientes  (tabla: cliente)
└── devsu_cuentas    → ms-cuentas   (tablas: cuenta, movimiento, cliente_ref)
```

**Regla de oro:** ningún microservicio accede a la BD de otro. Cada uno es el
**único dueño** de sus datos.

> *"Apliqué database-per-service: cada microservicio posee y gestiona su propio
> esquema. Nadie más lo lee ni lo escribe directamente."*

---

## 2. Por qué (beneficios)

| Beneficio | Explicación |
|-----------|-------------|
| **Bajo acoplamiento** | El esquema de un servicio puede evolucionar sin romper al otro |
| **Despliegue/escala independiente** | Cada microservicio se despliega y escala por separado |
| **Propiedad de datos clara** | Una sola fuente de verdad por dato; sin "quién mandó qué" |
| **Aislamiento de fallos** | Un problema en una BD no tumba al otro servicio |
| **Heterogeneidad** | Mañana ms-cuentas podría usar otra tecnología de BD sin afectar a clientes |

---

## 3. El costo (y por qué lo aceptamos)

Database-per-service **renuncia** a dos cosas que da una BD compartida:

| Lo que pierdes | Cómo lo resolvemos |
|----------------|--------------------|
| **JOINs entre servicios** | Réplica local de solo lectura (el nombre del cliente en `cliente_ref`) |
| **Transacciones ACID distribuidas** | No las necesitamos: cada operación crítica vive dentro de UN servicio |

> El costo es real, pero **controlado** y **justificado** por el desacoplamiento.

---

## 4. La clave de la defensa: consistencia FUERTE donde importa, EVENTUAL donde se acepta

Esta es la frase que más impresiona:

> **"Uso consistencia fuerte dentro de cada servicio (transacciones ACID) y
> consistencia eventual SOLO entre servicios, para datos no críticos."**

| Dato | Tipo de consistencia | Por qué |
|------|----------------------|---------|
| **Saldo de la cuenta** (F2/F3) | 🔒 **FUERTE** (ACID) | Es la operación crítica. Vive 100% dentro de `devsu_cuentas`, en una transacción `@Transactional`. **Nunca** hay un saldo inconsistente |
| **Nombre del cliente** en el reporte (F4) | 🔄 **EVENTUAL** | Es un dato denormalizado, descriptivo, no transaccional. Una pequeña demora en reflejar un cambio de nombre es aceptable |

→ **El dinero es fuertemente consistente. Solo un nombre (informativo) es
eventualmente consistente.** Ese matiz es lo que demuestra criterio.

---

## 5. Cómo manejamos la consistencia eventual

El reporte (F4) necesita el **nombre del cliente**, pero ms-cuentas no puede
consultar `devsu_clientes`. Solución: **replicación dirigida por eventos**.

```
ms-clientes                         RabbitMQ                    ms-cuentas
(crea/edita/borra cliente)  ──ClienteEvent──►  consume  ──►  upsert cliente_ref
   FUENTE DE VERDAD                                          (réplica de lectura)
                                                                   │
                                                                   ▼
                                                   el reporte lee el nombre de aquí
```

### Garantías que aplicamos (esto es lo que preguntan)

| Mecanismo | Para qué |
|-----------|----------|
| **Idempotencia** (upsert por `clienteId`) | Reprocesar un evento no genera inconsistencia |
| **Colas/mensajes durables** | Sobreviven reinicios de RabbitMQ |
| **Reintentos + Dead Letter Queue** | Un mensaje que falla no se pierde; va a la DLQ para inspección |
| **Réplica de solo lectura** | `cliente_ref` nunca es la fuente de verdad; solo una copia para consultar |

### Resiliencia (un plus enorme)
> *"Si ms-clientes se cae, ms-cuentas sigue operando: usa su réplica local. El
> reporte sigue funcionando con el último estado conocido."*

---

## 6. Preguntas trampa del evaluador (y tus respuestas)

**P: ¿Por qué no una sola base de datos compartida?**
R: Genera acoplamiento fuerte: los servicios no podrían evolucionar ni
desplegarse de forma independiente, y se vuelve un punto único de falla. La BD
compartida es un anti-patrón en microservicios.

**P: ¿No es malo duplicar el dato del cliente?**
R: Es **duplicación controlada de un read-model** (patrón CQRS). La fuente de
verdad sigue siendo ms-clientes; `cliente_ref` es solo una proyección de lectura.

**P: ¿Qué pasa si se pierde un evento?**
R: Las colas son durables y hay reintentos con DLQ. Como el consumidor es
idempotente, se puede reprocesar. Para robustez extra se podría añadir un job de
reconciliación periódica.

**P: ¿Y la consistencia del saldo? ¿No es riesgosa la consistencia eventual?**
R: El saldo **NO** es eventualmente consistente. Vive dentro de `devsu_cuentas`
y se actualiza en una **transacción ACID**. La consistencia eventual aplica
**solo** al nombre del cliente en el reporte, que es un dato informativo.

**P: ¿Por qué asíncrono y no una llamada REST síncrona a ms-clientes?**
R: Una llamada síncrona acoplaría temporalmente los servicios: si ms-clientes
está lento o caído, el reporte fallaría. El evento asíncrono **desacopla** y da
**resiliencia**. Además el enunciado pide comunicación asíncrona.

---

## 7. Resumen ejecutivo (el pitch de 30 segundos)

> *"Cada microservicio tiene su propia base de datos (database-per-service) para
> garantizar bajo acoplamiento e independencia. La consistencia es **fuerte**
> dentro de cada servicio —el saldo se maneja con transacciones ACID— y
> **eventual** entre servicios, solo para datos descriptivos como el nombre del
> cliente, que replico vía eventos de RabbitMQ con un consumidor idempotente.
> Esto me da resiliencia: ms-cuentas sigue operando aunque ms-clientes esté
> caído."*
