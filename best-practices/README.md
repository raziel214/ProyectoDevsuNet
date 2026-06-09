# Buenas Prácticas — Proyecto Devsu

Esta carpeta documenta los principios y convenciones que guían el desarrollo de
los microservicios **ms-clientes** y **ms-cuentas**. El objetivo es que el código,
la API y el historial de git sean **consistentes, predecibles y profesionales**.

| Documento | Tema | En una frase |
|-----------|------|--------------|
| [01-api-first.md](01-api-first.md) | **API First** | El contrato de la API se diseña **antes** que el código |
| [02-conventional-commits.md](02-conventional-commits.md) | **Conventional Commits** | Mensajes de commit con formato estándar y semántico |
| [03-git-flow.md](03-git-flow.md) | **Git Flow** | Modelo de ramas para organizar el trabajo y las entregas |
| [04-patrones-diseno.md](04-patrones-diseno.md) | **Patrones de Diseño** | Qué patrón usamos, dónde y por qué (y cuáles NO forzamos) |
| [05-database-per-service-consistencia.md](05-database-per-service-consistencia.md) | **DB-per-Service** | Defensa de BD separadas + consistencia eventual (para la entrevista) |

## Principios transversales aplicados

- **Clean Code & Clean Architecture** — separación por capas, responsabilidades
  únicas, nombres expresivos.
- **SOLID** — especialmente inversión de dependencias (interfaces de repositorio).
- **Database-per-service** — cada microservicio dueño de su propia BD.
- **Comunicación asíncrona** — desacoplamiento vía eventos (RabbitMQ).
- **12-Factor App** — configuración por entorno, secretos fuera del código (Vault).
- **DRY / KISS / YAGNI** — sin sobre-ingeniería; lo simple que cumple.
