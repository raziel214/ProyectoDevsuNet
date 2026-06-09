-- ============================================================
-- Inicialización de bases de datos del stack Devsu
-- Se ejecuta automáticamente en el PRIMER arranque del contenedor
-- (cuando el volumen de datos está vacío).
--
-- Patrón database-per-service: cada microservicio tiene su propia BD,
-- aislada de las demás. Keycloak también usa su propia BD.
-- El esquema de tablas lo gestiona cada microservicio vía JPA/Hibernate
-- (o el script BaseDatos.sql del entregable).
-- ============================================================

-- Base de datos del microservicio de Clientes
CREATE DATABASE devsu_clientes;

-- Base de datos del microservicio de Cuentas
CREATE DATABASE devsu_cuentas;

-- Base de datos para Keycloak (proveedor de identidad)
CREATE DATABASE keycloak;

-- El usuario POSTGRES_USER (devsu) es superusuario y dueño por defecto
-- de las BDs creadas arriba, por lo que tiene todos los privilegios.
