# Arquitectura

La aplicación sigue el patrón MVC: los controladores reciben las peticiones, utilizan el contexto de datos y preparan los modelos que muestran las vistas Razor. `Program.cs` configura los servicios, Identity, las cookies, el pipeline HTTP y el arranque de la base de datos.

```text
Controllers/             Controladores MVC
Data/                    Contexto de datos e inicialización
Models/                  Entidades y enumeraciones
Services/                Gestión de usuarios y almacenamiento de imágenes
Utilities/               Utilidades para identificar al usuario autenticado
ViewModels/              Modelos de formularios y listados
Views/                   Vistas Razor
wwwroot/                 Estilos, JavaScript y bibliotecas locales
App_Data/                Base de datos SQLite
Properties/              Perfiles de ejecución
tests/G2AspNet.Tests/     Proyecto de pruebas xUnit
docs/                    Documentación del entorno de desarrollo
.config/                 Herramientas locales de .NET
.github/workflows/       Integración continua
Dockerfile               Construcción y ejecución del contenedor
```

## Servicios y peticiones

Registramos `ApplicationDbContext` y `UserService` con duración por petición y `ImageStorage` como servicio único. El contexto se conecta a SQLite. Identity utiliza ese contexto para almacenar las cuentas y los roles.

El pipeline configura redirección HTTPS, archivos estáticos, enrutamiento, autenticación y autorización. Fuera de desarrollo activa el manejador de errores y HSTS. La ruta convencional es `{controller=Home}/{action=Index}/{id?}`.

Puedes seguir el arranque en [Program.cs](https://github.com/certidevs/g2_aspnet/blob/main/Program.cs) y las pantallas en [Controladores y vistas](https://github.com/certidevs/g2_aspnet/wiki/Controladores-y-vistas).

---

[Inicio](https://github.com/certidevs/g2_aspnet/wiki/Home) · [Documentación](https://github.com/certidevs/g2_aspnet/wiki/Home#documentación) · [Repositorio](https://github.com/certidevs/g2_aspnet) · [Comunicar un error](https://github.com/certidevs/g2_aspnet/issues)
