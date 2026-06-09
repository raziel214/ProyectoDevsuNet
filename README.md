# Proyecto Devsu .NET — Microservicios de Clientes y Cuentas

Versión **.NET** de la solución a la prueba técnica **Devsu** (Arquitectura de
Microservicios): dos microservicios que se comunican de forma **asíncrona** y
resuelven la gestión de Clientes, Cuentas y Movimientos de un dominio bancario.

> 🔗 Equivalente funcional del proyecto Java original. Los **contratos**
> (OpenAPI), el **esquema de BD**, la **infraestructura** y la **documentación de
> decisiones** son **idénticos** y se comparten entre ambas versiones.

| Microservicio | Puerto | Responsabilidad |
|---------------|:------:|-----------------|
| **ms-clientes** | `8085` | CRUD de Cliente (hereda de Persona). Publica eventos. |
| **ms-cuentas** | `8086` | CRU de Cuenta y Movimiento, saldos, reporte. Consume eventos. |

---

## ✅ Funcionalidades

| # | Requisito | Estado |
|---|-----------|:------:|
| **F1** | CRUD de Cliente; CRU de Cuenta y Movimiento (`/clientes`, `/cuentas`, `/movimientos`) | 🚧 |
| **F2** | Registro de movimientos (valor con signo) y actualización de saldo | 🚧 |
| **F3** | Validación de saldo → `"Saldo no disponible"` (HTTP 422) | 🚧 |
| **F4** | Reporte "Estado de cuenta" por rango de fechas y cliente (`/reportes`, JSON) | 🚧 |
| **F5** | Prueba unitaria de la entidad de dominio Cliente | 🚧 |
| **F6** | Prueba de integración (Testcontainers) | 🚧 |
| **F7** | Despliegue en contenedores (Docker) | 🚧 |
| **Extra** | Seguridad JWT (Keycloak), secretos (Vault), migraciones, E2E | 🚧 |

> 🚧 Estado inicial: estructura del repositorio. La implementación .NET se
> desarrolla sobre este esqueleto.

---

## 🏛️ Arquitectura

- **Arquitectura Hexagonal** (Puertos y Adaptadores): dominio puro aislado de la
  infraestructura. Dependencias hacia adentro (DIP).
- **Database-per-service**: cada microservicio es dueño de su propia BD.
- **Comunicación asíncrona** vía **RabbitMQ**: ms-clientes publica eventos
  (`ClienteCreado/Actualizado/Eliminado`); ms-cuentas mantiene una **réplica
  local** (`cliente_ref`, CQRS-lite) que usa el reporte F4.

```
                 evento ClienteEvent (RabbitMQ)
   ms-clientes  ───────────────────────────────►  ms-cuentas
  (devsu_clientes)                                (devsu_cuentas)
   CRUD Cliente                                    CRU Cuenta/Movimiento
   publica eventos                                 réplica cliente_ref → reporte F4
```

> 📘 El **porqué** de cada decisión está documentado en [`best-practices/`](best-practices/).
> 📐 Los **diagramas C4** están en [`docs/diagramas/`](docs/diagramas/).

---

## 🧰 Stack tecnológico (.NET)

- **.NET 8** (LTS), **C#**
- **ASP.NET Core** (Web API), **Entity Framework Core** (PostgreSQL / Npgsql)
- **RabbitMQ** (mensajería asíncrona)
- **Keycloak** (OIDC/JWT) · **Vault** (secretos)
- **Swashbuckle** (Swagger / OpenAPI)
- Pruebas: **xUnit**, **Moq/NSubstitute**, **Testcontainers for .NET**
- **Docker** / Docker Compose

---

## 📂 Estructura del repositorio

```
ProyectoDevsuNet/
├── ms-clientes/                # Microservicio de Clientes (hexagonal .NET)
│   ├── src/
│   │   ├── Devsu.Clientes.Domain/          # Modelo + excepciones de dominio
│   │   ├── Devsu.Clientes.Application/      # Casos de uso (Port/In, Port/Out, Service)
│   │   ├── Devsu.Clientes.Infrastructure/   # Persistencia, mensajería, seguridad
│   │   └── Devsu.Clientes.Api/              # Adaptador HTTP + composition root
│   └── tests/                  # UnitTests + IntegrationTests
├── ms-cuentas/                 # Microservicio de Cuentas/Movimientos (+ reporte F4)
│
├── contracts/                  # Contratos OpenAPI (API-first) + evento async  [compartido]
├── base-datos/                 # BaseDatos.sql (esquema) + doc de migraciones   [compartido]
├── infra/                      # Docker Compose de Postgres, RabbitMQ, Keycloak, Vault [compartido]
├── best-practices/             # Principios y decisiones de diseño              [compartido]
├── docs/diagramas/             # Diagramas de arquitectura C4                   [compartido]
├── postman/                    # Colecciones Postman                            [compartido]
├── evidencias/                 # Evidencias de funcionamiento (.NET)
└── README.md                   # (este archivo)
```

> Las carpetas marcadas **[compartido]** son **idénticas** a las del proyecto
> Java: misma fuente de verdad para contratos, esquema de BD e infraestructura.

---

## 📦 Entregables y documentación

| Artefacto | Ubicación |
|-----------|-----------|
| **Esquema de BD** (`BaseDatos.sql`) | [`base-datos/`](base-datos/) |
| **Contratos OpenAPI** (API-first) | [`contracts/`](contracts/) |
| **Colección Postman** | [`postman/`](postman/) |
| **Documentación de decisiones** | [`best-practices/`](best-practices/) |
| **Infraestructura** | [`infra/`](infra/) |

---

## 📄 Licencia

Distribuido bajo licencia **MIT**. Ver [`LICENSE`](LICENSE).

## 👤 Autor

**John Fredy Quimbaya** — Prueba técnica Devsu (versión .NET).
