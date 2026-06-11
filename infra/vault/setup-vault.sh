#!/usr/bin/env sh
# ============================================================
# Configura Vault para el proyecto Devsu .NET:
#   1) Habilita el motor KV v2 en "secret"
#   2) Siembra los secretos de ms-clientes y ms-cuentas
#   3) Crea la policy de solo-lectura "devsu-reader"
#   4) Crea el usuario userpass "devsu-reader" con esa policy
#
# Pensado para ejecutarse DENTRO del contenedor de Vault (tiene el CLI):
#   docker cp infra/vault/setup-vault.sh devsu_vault:/tmp/setup-vault.sh
#   docker exec -e VAULT_TOKEN=devsu-root-token devsu_vault sh /tmp/setup-vault.sh
#
# La contraseña del usuario se pasa por DEVSU_READER_PASSWORD (no se commitea).
# ============================================================
set -e

export VAULT_ADDR="${VAULT_ADDR:-http://127.0.0.1:8200}"
export VAULT_TOKEN="${VAULT_TOKEN:-devsu-root-token}"
DEVSU_READER_PASSWORD="${DEVSU_READER_PASSWORD:-devsu-reader-pass}"

# 1) Motor KV v2 (en dev ya viene habilitado; idempotente)
vault secrets enable -path=secret kv-v2 2>/dev/null || true

# 2) Secretos por microservicio (claves compatibles con la infra compartida)
vault kv put secret/ms-clientes \
  spring.datasource.username="devsu" \
  spring.datasource.password="devsu" \
  spring.rabbitmq.username="devsu" \
  spring.rabbitmq.password="devsu"

vault kv put secret/ms-cuentas \
  spring.datasource.username="devsu" \
  spring.datasource.password="devsu" \
  spring.rabbitmq.username="devsu" \
  spring.rabbitmq.password="devsu"

# 3) Policy de solo-lectura sobre los secretos (KV v2 -> ruta data/)
cat > /tmp/devsu-reader.hcl <<'EOF'
path "secret/data/ms-clientes" { capabilities = ["read"] }
path "secret/data/ms-cuentas"  { capabilities = ["read"] }
EOF
vault policy write devsu-reader /tmp/devsu-reader.hcl

# 4) Autenticacion userpass + usuario devsu-reader
vault auth enable userpass 2>/dev/null || true
vault write auth/userpass/users/devsu-reader \
  password="${DEVSU_READER_PASSWORD}" \
  policies="devsu-reader"

echo "============================================================"
echo "Vault configurado:"
echo "  - secretos: secret/ms-clientes, secret/ms-cuentas"
echo "  - policy:   devsu-reader (solo lectura)"
echo "  - usuario:  devsu-reader (userpass)"
echo "============================================================"
