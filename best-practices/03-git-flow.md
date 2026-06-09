# Git Flow

## ¿Qué es?

**Git Flow** es un modelo de ramificación (branching model) que define ramas con
roles claros para organizar el desarrollo, las versiones y las correcciones.
Aporta un historial ordenado y un proceso predecible de integración y entrega.

## Ramas del modelo

| Rama | Vida | Propósito |
|------|------|-----------|
| `main` | permanente | Código **estable y entregable** (cada commit es una versión potencial) |
| `develop` | permanente | Integración de funcionalidades terminadas (próxima versión) |
| `feature/*` | temporal | Una funcionalidad nueva; nace de `develop` y vuelve a `develop` |
| `release/*` | temporal | Preparación de una versión (ajustes finales, no nuevas features) |
| `hotfix/*` | temporal | Corrección urgente sobre `main`; vuelve a `main` y `develop` |

## Flujo visual

```
main      ●───────────────────────────●────────────●  (v1.0.0 ... releases)
           \                         /            /
release     \                   ●───●            /  (release/1.0.0)
             \                 /                 /
develop  ●────●────●────●────●─────────────●────●   (integración)
          \    \         \              /
feature    ●─●  ●──●──●    ●──●──●──●  (feature/clientes-crud, etc.)
```

## Convención de nombres de rama (este proyecto)

```
feature/clientes-crud
feature/cuentas-movimientos
feature/reporte-estado-cuenta
feature/seguridad-keycloak
feature/async-rabbitmq
release/1.0.0
hotfix/saldo-negativo
```

> Las ramas se nombran con `kebab-case` y un prefijo que indica su tipo.
> Se integran preferentemente vía **Pull Request** (revisión + historial claro).

## Recomendación para este assessment

Git Flow completo está pensado para equipos y releases frecuentes. Para una
**prueba técnica individual** aplicamos una versión **pragmática y ligera** que
demuestra dominio del modelo sin burocracia:

```
main                         ← estable / entregable (lo que se evalúa)
└── develop                  ← integración
    ├── feature/clientes-crud
    ├── feature/cuentas-movimientos
    ├── feature/reporte-estado-cuenta
    ├── feature/seguridad-keycloak
    └── feature/infra-docker
```

- Cada **funcionalidad (F1–F7)** o bloque de trabajo → su `feature/*`.
- Se integra a `develop` con commits [Conventional Commits](02-conventional-commits.md).
- Al terminar, se hace merge `develop → main` y se etiqueta **`v1.0.0`**.
- `hotfix/*` solo si aparece un bug crítico tras "entregar".

## Etiquetas (tags) de versión

Siguiendo **SemVer** (`MAJOR.MINOR.PATCH`), alineado con Conventional Commits:

```bash
git tag -a v1.0.0 -m "Entrega prueba técnica Devsu — F1 a F7"
git push origin v1.0.0
```

## Reglas que seguimos

1. **`main` siempre desplegable.** Nunca se commitea directo a `main`.
2. **Una rama por funcionalidad.** Aislada, enfocada, fácil de revisar.
3. **Merge vía PR** cuando sea posible (deja rastro y permite revisión).
4. **Tag por versión entregada.** Reproducibilidad de la entrega.
5. **Commits semánticos** en todas las ramas.
