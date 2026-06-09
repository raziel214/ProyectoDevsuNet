# Colecciones Postman — Proyecto Devsu

Una colección por microservicio, reproduciendo el flujo del enunciado.

| Archivo | Microservicio | Puerto |
|---------|---------------|:------:|
| [`Dev-su-MSclients.postman_collection.json`](Dev-su-MSclients.postman_collection.json) | ms-clientes (CRUD, F1) | 8085 |
| [`Dev-su-MScuentas.postman_collection.json`](Dev-su-MScuentas.postman_collection.json) | ms-cuentas (F1–F4) | 8086 |

## ▶️ Cómo usarlas

### 1. Importar
En Postman: **Import** → arrastra los **2** archivos `*.postman_collection.json`.

### 2. Completar el `clientSecret` (en cada colección)
Cada colección trae sus variables en la pestaña **Variables**. Completa una sola:

| Variable | Valor |
|----------|-------|
| `clientSecret` | Keycloak → Clients → **ms-clientes** → Credentials → *Client secret* |

> El resto ya viene listo: `baseUrl`, `keycloakUrl`, `realm`, `clientId`.

### 3. Ejecutar en orden
**Primero la de clientes, luego la de cuentas** (las cuentas referencian clientes
que deben existir). En cada colección, corre **"0. Obtener Token"** primero.

**Dev-su-MSclients:**
| Request | Qué hace |
|---------|----------|
| 0. Obtener Token | JWT → se guarda solo en `{{token}}` |
| Crear Cliente ×4 | Jose Lema (CLI-001), Marianela (CLI-002), Juan Osorio (CLI-003), John Quimbaya |
| Listar / Obtener / Actualizar / Eliminar | CRUD completo (F1) |

**Dev-su-MScuentas:**
| Carpeta | Qué hace |
|---------|----------|
| 0. Obtener Token | JWT → `{{token}}` |
| **Cuentas (F1)** | Crea las 5 cuentas del enunciado + CRU |
| **Movimientos (F2/F3)** | Los 4 movimientos + caso **422 "Saldo no disponible"** + PUT descripción |
| **Reporte (F4)** | Estado de cuenta por rango y cliente |

> El token se inyecta automáticamente (auth heredada *Bearer* a nivel de
> colección). Si expira (~5 min), vuelve a correr **0. Obtener Token**.

## 💡 Notas

- Los **ids** de `GET/PUT/DELETE por id` usan `/1` de ejemplo; ajústalos a los ids
  que devuelva tu base (son autogenerados).
- El **reporte (F4)** filtra por fecha: usa un rango que incluya la fecha de los
  movimientos (se registran con la fecha/hora actual).
- Puedes correr cada colección completa con el **Collection Runner**.

## 🔒 Seguridad

El `clientSecret` va como **placeholder** (no se versiona un secreto real). Es una
credencial de **desarrollo** del realm de la prueba; en producción iría en Vault,
nunca en el repositorio.
