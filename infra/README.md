# Infraestructura — Proyecto Devsu

Manifiestos Docker Compose de la infraestructura de soporte para los
microservicios **ms-clientes** y **ms-cuentas**.

| Aplicación | Carpeta | Imagen | Rol |
|------------|---------|--------|-----|
| PostgreSQL | [`postgres/`](postgres/) | postgres:16 | Base de datos relacional (database-per-service) |
| RabbitMQ   | [`rabbitmq/`](rabbitmq/) | rabbitmq:3.13-management | Mensajería asíncrona entre microservicios |
| Keycloak   | [`keycloak/`](keycloak/) | keycloak:26.0 | Proveedor de identidad (OIDC/OAuth2, JWT) |
| Vault      | [`vault/`](vault/) | hashicorp/vault | Gestión centralizada de secretos |

Cada aplicación tiene su **propio `docker-compose.yml`** y todas comparten la
red Docker externa **`devsu_network`** con alias estables (`postgres`,
`rabbitmq`, `keycloak`, `vault`) para que la configuración de los microservicios
sea idéntica en cualquier entorno.

---

## 🚀 Despliegue (desde cero)

Levanta toda la infraestructura desde cero — lo que ejecuta quien califique la
prueba en una máquina limpia.

### 1. Requisitos
- Docker Desktop / Docker Engine con Docker Compose v2.20+

### 2. Crear la red compartida (una sola vez)
```bash
docker network create devsu_network
```

### 3. (Opcional) Configurar variables
```bash
cp .env.example .env     # ajusta puertos/credenciales si lo necesitas
```

### 4. Levantar toda la infraestructura
```bash
# Desde la carpeta infra/ — usa el orquestador con include
docker compose up -d
```
O aplicación por aplicación, respetando el orden (Postgres primero):
```bash
docker compose -f postgres/docker-compose.yml up -d
docker compose -f rabbitmq/docker-compose.yml up -d
docker compose -f keycloak/docker-compose.yml up -d
docker compose -f vault/docker-compose.yml up -d
```

### 5. Sembrar los secretos en Vault (una vez)
```bash
docker exec -e VAULT_TOKEN=devsu-root-token devsu_vault \
  sh -c "$(cat vault/seed-secrets.sh)"
```

### 6. Levantar los microservicios
Desde la raíz del repo (`../`):
```bash
docker compose up -d        # ms-clientes (8085) + ms-cuentas (8086)
```

---

## 🔌 Puertos y credenciales

| Servicio | URL | Usuario | Contraseña |
|----------|-----|---------|------------|
| PostgreSQL | `localhost:5432` | `devsu` | `devsu` |
| RabbitMQ (UI) | http://localhost:15672 | `devsu` | `devsu` |
| Keycloak | http://localhost:8081 | `admin` | `admin` |
| Vault | http://localhost:8200 | token: `devsu-root-token` | — |

Bases de datos creadas automáticamente: `devsu_clientes`, `devsu_cuentas`, `keycloak`.

> ⚠️ Credenciales de **desarrollo**. En un entorno real irían en Vault / variables
> seguras y nunca en el repositorio.

---

## ✅ Verificación rápida
```bash
docker compose ps                 # estado y health de los contenedores
docker network inspect devsu_network
```
Espera a que Postgres, RabbitMQ y Keycloak estén `healthy` antes de arrancar
los microservicios.

## 🧹 Apagar / limpiar
```bash
docker compose down               # detiene los contenedores
docker compose down -v            # + elimina volúmenes (datos)
docker network rm devsu_network   # elimina la red (si ya nada la usa)
```
