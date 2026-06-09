# Evidencias de Funcionamiento — Proyecto Devsu

Capturas que demuestran que la solución funciona end-to-end. Organizadas por
funcionalidad del assessment (F1–F7) e infraestructura.

> 💡 Nombra las imágenes con un prefijo numérico para que queden ordenadas:
> `01-post-cliente-201.png`, `02-get-clientes.png`, etc.

---

## 📁 Estructura

| Carpeta | Qué documenta | Capturas sugeridas |
|---------|---------------|--------------------|
| [`01-clientes-crud/`](01-clientes-crud/) | **F1** — CRUD de Cliente | POST 201, GET lista, GET por id, PUT, DELETE 204, 404 no encontrado |
| [`02-cuentas-movimientos/`](02-cuentas-movimientos/) | **F1/F2** — Cuentas y movimientos | Crear cuenta, depósito, retiro, saldo actualizado |
| [`03-saldo-no-disponible/`](03-saldo-no-disponible/) | **F3** — Validación de saldo | Retiro sin fondos → 422 "Saldo no disponible" |
| [`04-reporte-estado-cuenta/`](04-reporte-estado-cuenta/) | **F4** — Reporte | `/reportes` con rango de fechas + cliente, JSON resultante |
| [`05-pruebas-unitarias/`](05-pruebas-unitarias/) | **F5** — Test unitario | Test del dominio Cliente en verde (JUnit) |
| [`06-pruebas-integracion/`](06-pruebas-integracion/) | **F6** — Test integración | Test con Testcontainers en verde |
| [`07-docker/`](07-docker/) | **F7** — Contenedores | `docker ps`, contenedores healthy, app respondiendo |
| [`08-seguridad/`](08-seguridad/) | Seguridad (extra) | Token Keycloak, 401 sin token, Authorize en Swagger |
| [`09-vault/`](09-vault/) | Secretos (extra) | Secret en Vault UI, log de la app leyendo el secret |
| [`10-swagger/`](10-swagger/) | Documentación API | Swagger UI con los endpoints |

---

## ✅ Checklist de evidencias

- [ ] **F1** ms-clientes: POST/GET/PUT/DELETE funcionando
- [ ] **F1** ms-cuentas: CRU de Cuenta y Movimiento
- [ ] **F2** Movimiento actualiza el saldo (depósito/retiro)
- [ ] **F3** "Saldo no disponible" al retirar sin fondos
- [ ] **F4** Reporte de estado de cuenta en JSON
- [ ] **F5** Prueba unitaria del dominio Cliente
- [ ] **F6** Prueba de integración
- [ ] **F7** Solución corriendo en Docker
- [ ] Seguridad: endpoints protegidos con JWT (401 sin token)
- [ ] Vault: credenciales de BD cargadas desde Vault
- [ ] Swagger UI documentando la API
