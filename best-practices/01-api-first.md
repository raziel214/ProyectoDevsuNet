# API First

## ¿Qué es?

**API First** (o *contract-first*) es un principio de diseño donde la **API se
define primero**, como un contrato formal y acordado, **antes** de escribir la
implementación. El contrato (OpenAPI/Swagger) es la **fuente de verdad**: tanto
el productor (backend) como los consumidores (frontend, otros microservicios,
QA) parten del mismo documento.

Es lo opuesto a *code-first*, donde la API "emerge" del código ya escrito.

## ¿Por qué lo aplicamos en este proyecto?

| Beneficio | Impacto en Devsu |
|-----------|------------------|
| **Acuerdo temprano** | Los endpoints F1–F4 quedan definidos y validados antes de codificar |
| **Trabajo en paralelo** | ms-clientes y ms-cuentas se desarrollan contra contratos claros |
| **Menos retrabajo** | Los cambios de diseño se discuten en el YAML, no en el código |
| **Documentación viva** | El contrato se publica como Swagger UI en cada microservicio |
| **Pruebas guiadas** | Postman/Karate se construyen directamente desde el contrato |

## Cómo lo aplicamos (flujo de trabajo)

```
1. Diseñar el contrato        →  contracts/*.openapi.yaml   (ANTES del código)
2. Revisar / acordar          →  validar reglas de negocio F1–F4
3. Implementar contra él      →  controllers que cumplen el contrato
4. Exponer la doc viva        →  springdoc-openapi → Swagger UI
5. Validar con pruebas        →  Postman / Karate contra los endpoints
```

## Artefactos del contrato en este repo

| Archivo | Define |
|---------|--------|
| [`../contracts/ms-clientes.openapi.yaml`](../contracts/ms-clientes.openapi.yaml) | API REST de Clientes (CRUD, F1) |
| [`../contracts/ms-cuentas.openapi.yaml`](../contracts/ms-cuentas.openapi.yaml) | API REST de Cuentas/Movimientos/Reportes (F1–F4) |
| [`../contracts/async-eventos-rabbitmq.md`](../contracts/async-eventos-rabbitmq.md) | Contrato del evento asíncrono `ClienteEvent` |

## Reglas que seguimos

1. **El contrato manda.** Si el código y el contrato difieren, gana el contrato
   (o se actualiza el contrato de forma explícita y acordada).
2. **Versionado.** Cambios incompatibles → nueva versión del contrato (`v2`).
3. **Nombres y códigos HTTP consistentes** con el contrato (ej. `422` para
   "Saldo no disponible", `404` para recurso inexistente).
4. **La documentación viva** (Swagger UI) se genera del código pero debe
   **coincidir** con el OpenAPI de `contracts/`.

## Selección de verbos REST (decisión de diseño)

El enunciado pide *"Manejar los verbos: Get, Post, Put, Patch, Delete, **para las
acciones que apliquen según su criterio**"*. La coletilla es clave: **no exige
implementar los 5 verbos en cada recurso**, sino usar el verbo que corresponda a
cada acción, a criterio del desarrollador.

Nuestro criterio (REST estándar) por recurso:

| Recurso | GET | POST | PUT | PATCH | DELETE | Notas |
|---------|:---:|:----:|:---:|:-----:|:------:|-------|
| **Cliente** | ✅ | ✅ | ✅ | ➖ | ✅ | CRUD completo (F1) |
| **Cuenta** | ✅ | ✅ | ✅ | ➖ | — | CRU (F1); sin DELETE (no se borran cuentas) |
| **Movimiento** | ✅ | ✅ | ✅ | ➖ | — | CRU (F1) |

**¿Por qué omitimos PATCH (➖)?** Decisión consciente, amparada en *"según su
criterio"*:

- **PUT ya cubre la acción "Actualizar"** de F1. Un `PATCH` (actualización
  parcial) no aporta una **acción de negocio distinta** en este dominio.
- Añadirlo duplicaría endpoints y superficie de pruebas sin valor funcional.
- **El contrato describe exactamente lo implementado** (no promete PATCH que no
  exista) → sin *contract drift*.

**Matiz por recurso en el PUT** (mismo verbo, distinto alcance, por reglas de
negocio):

- **Cliente → PUT total** (`ClienteRequest`): todos sus datos son editables
  (nombre, dirección, teléfono, clave...). La identidad (`identificacion`,
  `clienteId`) no cambia.
- **Cuenta → PUT acotado** (`CuentaUpdateRequest`: solo `tipoCuenta` + `estado`):
  el `saldoDisponible` es un **invariante del *ledger*** — solo cambia vía
  movimientos (F2), nunca por edición directa. Editarlo a mano rompería la
  trazabilidad contable.

## Herramientas

- **OpenAPI 3.0** — lenguaje del contrato.
- **springdoc-openapi** — expone Swagger UI (`/swagger-ui.html`) en cada servicio.
- **Swagger Editor** — para visualizar/editar los `.yaml`.
- **Postman / Karate** — validación de los endpoints contra el contrato.
