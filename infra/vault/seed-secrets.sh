#!/usr/bin/env sh
# ============================================================
# Carga los secretos iniciales en Vault (KV v2) para Devsu.
# Ejecutar UNA vez tras levantar el contenedor de Vault.
#
# Uso:
#   docker exec -e VAULT_TOKEN=devsu-root-token devsu_vault \
#     sh /vault/seed/seed-secrets.sh
# o montando este script, o simplemente copiando/pegando los comandos.
# ============================================================
set -e

export VAULT_ADDR="${VAULT_ADDR:-http://127.0.0.1:8200}"
export VAULT_TOKEN="${VAULT_TOKEN:-devsu-root-token}"

# Habilitar el motor KV v2 en la ruta "secret" (en dev ya viene habilitado)
vault secrets enable -path=secret kv-v2 2>/dev/null || true

# ---- Secretos del microservicio de Clientes ----
vault kv put secret/ms-clientes \
  spring.datasource.username="devsu" \
  spring.datasource.password="devsu" \
  spring.rabbitmq.username="devsu" \
  spring.rabbitmq.password="devsu"

# ---- Secretos del microservicio de Cuentas ----
vault kv put secret/ms-cuentas \
  spring.datasource.username="devsu" \
  spring.datasource.password="devsu" \
  spring.rabbitmq.username="devsu" \
  spring.rabbitmq.password="devsu"

echo "Secretos cargados en Vault correctamente."
vault kv get secret/ms-clientes
vault kv get secret/ms-cuentas
