# Conventional Commits

## ¿Qué es?

[**Conventional Commits**](https://www.conventionalcommits.org/) es una
especificación para escribir mensajes de commit con un **formato estándar y
semántico**. Esto hace el historial legible, permite **automatizar** el
versionado (SemVer) y la generación de *changelogs*.

## Formato

```
<tipo>(<alcance opcional>): <descripción breve>

[cuerpo opcional]

[footer opcional]
```

- **tipo** — naturaleza del cambio (ver tabla).
- **alcance** — parte del proyecto afectada (opcional pero recomendado).
- **descripción** — en imperativo, minúscula, sin punto final. (ej: "agrega", no "agregado").

## Tipos

| Tipo | Uso |
|------|-----|
| `feat` | Una nueva funcionalidad (genera versión MINOR) |
| `fix` | Corrección de un bug (genera versión PATCH) |
| `docs` | Solo documentación |
| `style` | Formato, espacios, sin cambio de lógica |
| `refactor` | Cambio de código que no corrige bug ni agrega feature |
| `perf` | Mejora de rendimiento |
| `test` | Agregar o corregir pruebas |
| `build` | Cambios en build, dependencias (Maven, Dockerfile) |
| `ci` | Configuración de integración continua |
| `chore` | Tareas varias que no tocan src ni tests |
| `revert` | Revierte un commit previo |

## Alcances (scopes) sugeridos para este proyecto

`clientes`, `cuentas`, `movimientos`, `reportes`, `contracts`, `infra`,
`docker`, `security`, `rabbitmq`, `db`, `docs`.

## Breaking Changes

Cambios incompatibles se marcan con `!` después del tipo/alcance **o** con un
footer `BREAKING CHANGE:` (generan versión MAJOR):

```
feat(cuentas)!: cambia el contrato de /reportes a rango de fechas obligatorio
```

## Ejemplos aplicados a Devsu

```bash
feat(clientes): implementa CRUD de Cliente (F1)
feat(cuentas): registra movimientos con actualización de saldo (F2)
fix(cuentas): retorna 422 "Saldo no disponible" en retiro sin fondos (F3)
feat(reportes): genera estado de cuenta por rango de fechas y cliente (F4)
feat(rabbitmq): publica ClienteEvent y sincroniza réplica local
test(clientes): agrega prueba unitaria del dominio Cliente (F5)
test(cuentas): agrega prueba de integración con Testcontainers (F6)
build(docker): agrega Dockerfile multi-stage de ms-cuentas (F7)
docs(contracts): define contrato OpenAPI de ms-clientes
chore(infra): agrega docker-compose de Postgres, RabbitMQ, Keycloak y Vault
```

## Reglas que seguimos

1. **Un commit = un cambio lógico.** Atómico y coherente.
2. **Descripción en imperativo presente.** "agrega", "corrige", "implementa".
3. **Máx. ~72 caracteres** en la primera línea.
4. **Referencia la funcionalidad** del assessment cuando aplique (F1–F7).
5. **El cuerpo explica el "por qué"**, no el "qué" (eso lo dice el diff).
