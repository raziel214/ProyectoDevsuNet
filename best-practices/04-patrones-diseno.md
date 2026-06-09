# Patrones de Diseño Aplicados

> **Principio rector:** un patrón se usa cuando **resuelve un problema concreto**,
> no por decorar. Sobre-aplicar patrones (*pattern soup*) es un anti-patrón.
> Aquí está el mapa **curado** de los que SÍ aportan valor en este proyecto.

---

## 🟢 Patrones que aplicamos (y dónde)

### Creacionales

| Patrón | Dónde en el proyecto | Por qué |
|--------|----------------------|---------|
| **Builder** | `@SuperBuilder`/`@Builder` (Lombok) en entidades de dominio y construcción de DTOs | Construcción legible paso a paso de objetos con muchos campos; soporta herencia (Persona→Cliente) |
| **Factory Method** | `Movimiento.crear(valor, saldoActual)` decide tipo (DEPÓSITO/RETIRO) y calcula saldo | Encapsula la lógica de creación + reglas (F2/F3) en un único lugar del dominio |
| **Singleton** | *Gratis* — todos los `@Service`, `@Repository`, `@Component` de Spring son singletons gestionados por el contenedor | No lo implementamos a mano (eso sería anti-patrón); lo provee el IoC de Spring |

### Estructurales

| Patrón | Dónde en el proyecto | Por qué |
|--------|----------------------|---------|
| **Adapter** ⭐ | **Núcleo de Hexagonal**: `ClientePersistenceAdapter`, `ClienteController`, `ClienteEventPublisherAdapter` | Traduce entre el mundo externo (JPA, REST, AMQP) y los puertos del dominio. Es EL patrón de Ports & Adapters |
| **Facade** | `ReporteService` unifica datos de Cuenta + Movimiento + réplica de Cliente (F4) | Una interfaz simple sobre un subsistema de varias fuentes |
| **Proxy** | *Gratis* — `@Transactional`, seguridad y AOP de Spring usan proxies dinámicos | Lo aprovechamos, no lo escribimos |
| **DTO** | `records` para Request/Response y eventos | Desacopla el contrato de la API del modelo de dominio |
| **Mapper** | `ClienteDtoMapper`, `ClientePersistenceMapper` (dominio ↔ JPA ↔ DTO) | Mantiene el dominio puro; aísla las traducciones |

### De Comportamiento

| Patrón | Dónde en el proyecto | Por qué |
|--------|----------------------|---------|
| **Strategy** | Procesamiento del movimiento (depósito vs retiro) como estrategias intercambiables | Cada algoritmo encapsulado; abre/cierra para nuevos tipos sin tocar el existente (OCP) |
| **Observer / Pub-Sub** ⭐ | Eventos `ClienteEvent` vía RabbitMQ: ms-clientes publica, ms-cuentas reacciona | Comunicación asíncrona y desacoplada entre microservicios |
| **Template Method** | (Opcional) esqueleto común en servicios/validaciones base | Solo si aparece duplicación real de algoritmo |

---

## 🧬 Decisión: Records vs Clases vs Lombok

Combinamos las tres herramientas **según el caso** (esto es clave):

| Herramienta | Para qué la usamos | Por qué |
|-------------|--------------------|---------|
| **`record`** | DTOs (Request/Response), eventos (`ClienteEvent`), objetos de reporte | Inmutables, concisos, `equals/hashCode/toString` automáticos. Ideales para transportar datos |
| **Clase + Lombok + herencia** | Entidades de dominio `Persona` → `Cliente` | Un `record` **no puede heredar** (es `final`). La herencia del enunciado obliga a clases. Lombok quita el boilerplate |
| **Enum** | `Genero`, `TipoCuenta`, `TipoMovimiento` | *Value objects* con dominio cerrado y tipado fuerte |

> ⚠️ Por eso `Persona`/`Cliente` son **clases** (necesitan herencia) y los DTOs son
> **records** (no heredan, solo transportan datos). No es contradicción: es elegir
> la herramienta correcta para cada responsabilidad.

---

## 🏛️ Patrones arquitectónicos / de microservicios

| Patrón | Aplicación |
|--------|------------|
| **Hexagonal (Ports & Adapters)** | Estructura interna de cada microservicio |
| **Vertical Slicing** | Organización de paquetes por feature/dominio |
| **Repository** | `Spring Data` + puerto `ClienteRepositoryPort` (exigido por el assessment) |
| **Database per Service** | `devsu_clientes` y `devsu_cuentas` independientes |
| **Event-Driven Architecture** | Mensajería asíncrona con RabbitMQ |
| **Read Model / CQRS-lite** | Réplica local `cliente_ref` en ms-cuentas para el reporte (F4) |

---

## 🔴 Patrones que NO forzamos (y por qué)

Saber **cuándo NO** usar un patrón también es seniority:

| Patrón | Por qué lo omitimos |
|--------|---------------------|
| **Abstract Factory** | No hay familias de objetos que varíen juntas; Factory Method basta |
| **Saga / Circuit Breaker** | Sin transacciones distribuidas ni cascada de fallos que lo justifiquen en este alcance |
| **Decorator / Bridge / Composite / Flyweight** | No hay jerarquías parte-todo, variación de implementación ni presión de memoria |
| **Visitor / Interpreter / Memento / Mediator / State** | Añadirían complejidad sin resolver un problema presente (YAGNI) |
| **Singleton manual** | Anti-patrón teniendo el contenedor de Spring |

> **Regla:** si un patrón no elimina un dolor concreto **hoy**, no entra. El código
> simple y claro le gana siempre al código "lleno de patrones".
