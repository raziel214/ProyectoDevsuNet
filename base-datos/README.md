# Base de Datos — Proyecto Devsu

Documentación del esquema de datos y la estrategia de migraciones.

## 🧭 Estrategia: Flyway como fuente de verdad

El esquema **NO** se gestiona a mano: cada microservicio usa **Flyway**
(migraciones versionadas) que corren **automáticamente al arrancar** la app.

| Artefacto | Rol |
|-----------|-----|
| **Migraciones Flyway** (en cada micro) | **Fuente de verdad** — crean y versionan el esquema |
| **[`BaseDatos.sql`](BaseDatos.sql)** | Snapshot **consolidado** para la entrega / creación manual |

> Ventaja: el esquema es **versionado, reproducible e idéntico** en cualquier
> entorno (dev, test con Testcontainers, y la BD del evaluador).

## 🗄️ Database-per-service (2 bases de datos)

```
PostgreSQL
├── devsu_clientes   → ms-clientes   (tabla: cliente)
└── devsu_cuentas    → ms-cuentas    (tablas: cuenta, movimiento, cliente_ref)
```

Cada microservicio es **dueño** de su BD; nadie accede a la del otro. Por eso
el reporte (F4) usa una **réplica local** (`cliente_ref`) sincronizada por
eventos, no una consulta directa a la BD de clientes.

## 📜 Migraciones documentadas

### ms-clientes → BD `devsu_clientes`
| Versión | Archivo | Qué hace |
|---------|---------|----------|
| **V1** | `ms-clientes/src/main/resources/db/migration/V1__crear_tabla_cliente.sql` | Crea la tabla `cliente` (Persona `@MappedSuperclass` + Cliente en una sola tabla). PK `id`, únicos `identificacion` y `cliente_id`. |

### ms-cuentas → BD `devsu_cuentas`
| Versión | Archivo | Qué hace |
|---------|---------|----------|
| **V1** | `ms-cuentas/.../V1__crear_tablas_cuenta_movimiento.sql` | Crea `cuenta` (único `numero_cuenta`) y `movimiento` (FK a `cuenta` + índice). Saldos `NUMERIC(19,2)`. |
| **V2** | `ms-cuentas/.../V2__crear_tabla_cliente_ref.sql` | Crea `cliente_ref` (réplica read-model del cliente, PK `cliente_id`). |

> Regla Flyway: una migración aplicada **nunca se edita**. Un cambio futuro =
> una nueva versión (V3, V4...). Flyway lleva el control en la tabla
> `flyway_schema_history` de cada BD.

## ▶️ Cómo usar este script

### Opción A — Automática (recomendada)
No hagas nada: al arrancar cada microservicio, **Flyway crea las tablas solo**.

### Opción B — Manual (con `BaseDatos.sql`)
El script tiene 3 pasos (comentados dentro del archivo):
1. **Crear las bases** (`CREATE DATABASE`) — como superusuario.
2. **Conectarse a `devsu_clientes`** y ejecutar la sección de la tabla `cliente`.
3. **Conectarse a `devsu_cuentas`** y ejecutar la sección de `cuenta`,
   `movimiento` y `cliente_ref`.

> Se separó en secciones por base (en lugar de `\connect`) para que el script
> sea SQL estándar y abra limpio en cualquier cliente/IDE.

## 🧩 Modelo de datos (resumen)

```
devsu_clientes                          devsu_cuentas
┌────────────────┐                      ┌────────────────┐   ┌──────────────┐
│ cliente        │                      │ cuenta         │1 ∞│ movimiento   │
│ id (PK)        │   ── evento async ──►│ id (PK)        │───│ id (PK)      │
│ cliente_id (U) │      (RabbitMQ)      │ numero_cta (U) │   │ cuenta_id(FK)│
│ nombre, ...    │           │          │ saldos, ...    │   │ valor, saldo │
│ contrasena     │           │          │ cliente_id     │   └──────────────┘
└────────────────┘           ▼          └────────────────┘
                       ┌──────────────┐
                       │ cliente_ref  │  réplica (read-model) para el reporte F4
                       │ cliente_id PK│
                       │ nombre, ...  │
                       └──────────────┘
```
