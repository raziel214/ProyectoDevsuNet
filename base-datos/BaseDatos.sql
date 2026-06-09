-- ============================================================
-- BaseDatos.sql — Proyecto Devsu (prueba técnica)
-- Esquema consolidado de las DOS bases de datos
-- (patrón database-per-service: una BD por microservicio).
--
-- NOTA: las migraciones Flyway de cada microservicio son la FUENTE DE VERDAD;
-- al arrancar cada app, Flyway crea las tablas automáticamente. Este script es
-- un snapshot consolidado / referencia para crear las BDs a mano si se desea.
-- Ver README.md de esta carpeta.
-- ============================================================


-- ============================================================
-- PASO 1 — Crear las bases de datos (ejecutar como superusuario)
-- ============================================================
CREATE DATABASE devsu_clientes;
CREATE DATABASE devsu_cuentas;

-- (Opcional) usuario dedicado de la aplicación:
-- CREATE ROLE devsu WITH LOGIN PASSWORD 'Devsu.Db.2026';
-- ALTER DATABASE devsu_clientes OWNER TO devsu;
-- ALTER DATABASE devsu_cuentas  OWNER TO devsu;


-- ============================================================
-- PASO 2 — Conéctate a la base "devsu_clientes" y ejecuta esto
--          (microservicio ms-clientes · migración V1__crear_tabla_cliente.sql)
-- ============================================================

-- Persona es @MappedSuperclass (no es tabla); sus columnas viven aquí.
-- Cliente es la única entidad concreta -> una sola tabla "cliente".
CREATE TABLE cliente (
    id              BIGSERIAL    PRIMARY KEY,   -- PK (de Persona)
    nombre          VARCHAR(120) NOT NULL,
    genero          VARCHAR(20),
    edad            INTEGER,
    identificacion  VARCHAR(20)  NOT NULL,
    direccion       VARCHAR(200),
    telefono        VARCHAR(20),
    cliente_id      VARCHAR(50)  NOT NULL,      -- clave única de negocio (de Cliente)
    contrasena      VARCHAR(255) NOT NULL,      -- hasheada con BCrypt
    estado          BOOLEAN      NOT NULL,
    CONSTRAINT uk_cliente_identificacion UNIQUE (identificacion),
    CONSTRAINT uk_cliente_cliente_id     UNIQUE (cliente_id)
);


-- ============================================================
-- PASO 3 — Conéctate a la base "devsu_cuentas" y ejecuta esto
--          (microservicio ms-cuentas · migraciones V1 y V2)
-- ============================================================

CREATE TABLE cuenta (
    id               BIGSERIAL     PRIMARY KEY,
    numero_cuenta    VARCHAR(20)   NOT NULL,
    tipo_cuenta      VARCHAR(20)   NOT NULL,    -- AHORROS / CORRIENTE
    saldo_inicial    NUMERIC(19,2) NOT NULL,
    saldo_disponible NUMERIC(19,2) NOT NULL,
    estado           BOOLEAN       NOT NULL,
    cliente_id       VARCHAR(50)   NOT NULL,    -- referencia al cliente (otro microservicio)
    CONSTRAINT uk_cuenta_numero UNIQUE (numero_cuenta)
);

CREATE TABLE movimiento (
    id               BIGSERIAL     PRIMARY KEY,
    fecha            TIMESTAMP     NOT NULL,
    tipo_movimiento  VARCHAR(20)   NOT NULL,    -- DEPOSITO / RETIRO
    valor            NUMERIC(19,2) NOT NULL,    -- con signo (+ depósito, - retiro)
    saldo            NUMERIC(19,2) NOT NULL,    -- saldo resultante tras el movimiento
    cuenta_id        BIGINT        NOT NULL,
    descripcion      VARCHAR(255),              -- metadata editable (único campo mutable; el ledger es inmutable)
    CONSTRAINT fk_movimiento_cuenta FOREIGN KEY (cuenta_id) REFERENCES cuenta (id)
);

CREATE INDEX idx_movimiento_cuenta ON movimiento (cuenta_id);

-- Réplica de cliente (read-model / CQRS-lite). Se sincroniza de forma
-- asíncrona desde ms-clientes vía eventos RabbitMQ. La usa el reporte (F4).
CREATE TABLE cliente_ref (
    cliente_id     VARCHAR(50)  PRIMARY KEY,    -- clave de negocio (no autogenerada)
    nombre         VARCHAR(120) NOT NULL,
    identificacion VARCHAR(20),
    estado         BOOLEAN      NOT NULL
);
