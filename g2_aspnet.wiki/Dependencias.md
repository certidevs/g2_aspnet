# Dependencias

| Componente | Detalle |
| --- | --- |
| Plataforma | .NET 10, destino `net10.0`. |
| Aplicación web | ASP.NET Core MVC y vistas Razor. |
| Acceso a datos | Entity Framework Core 10.0.11 con SQLite. |
| Autenticación | ASP.NET Core Identity 10.0.11, roles y cookies. |
| Interfaz | Bootstrap 5, jQuery 3.7.1 y validación de formularios, con recursos locales en `wwwroot/lib`. |
| Pruebas | xUnit 2.9.3 y Microsoft.NET.Test.Sdk 17.14.1. |
| Integración continua | GitHub Actions: restauración, compilación y pruebas. |
| Contenedor | Docker, con imágenes de SDK y runtime de .NET 10. |

Versiones declaradas en [G2AspNet.csproj](https://github.com/certidevs/g2_aspnet/blob/main/G2AspNet.csproj) y en el [proyecto de pruebas](https://github.com/certidevs/g2_aspnet/blob/main/tests/G2AspNet.Tests/G2AspNet.Tests.csproj).

## Actualización

Las dependencias NuGet se restauran con `dotnet restore G2AspNet.sln`. Al cambiar versiones revisamos la compatibilidad con `net10.0`, compilamos y ejecutamos las pruebas. Las bibliotecas de la interfaz están en [wwwroot/lib](https://github.com/certidevs/g2_aspnet/tree/main/wwwroot/lib).

El inventario de paquetes no equivale a una auditoría de vulnerabilidades. Las comprobaciones automáticas actuales se explican en [Testing y CI](https://github.com/certidevs/g2_aspnet/wiki/Testing-y-CI).

---

[Inicio](https://github.com/certidevs/g2_aspnet/wiki/Home) · [Documentación](https://github.com/certidevs/g2_aspnet/wiki/Home#documentación) · [Repositorio](https://github.com/certidevs/g2_aspnet) · [Comunicar un error](https://github.com/certidevs/g2_aspnet/issues)
