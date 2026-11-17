# G2 · Gestión de academias y centros de formación

Aplicación web del Grupo 2 para gestionar centros de formación, cursos, matrículas, calificaciones y asistencias. La estamos desarrollando como proyecto del curso de .NET.

> **En desarrollo:** cada arranque elimina y recrea la base de datos. No uses una base con datos que quieras conservar.

## Funcionalidades

- Registro, inicio de sesión, perfil con avatar y cambio de contraseña.
- Administración de usuarios y roles.
- Gestión de centros y cursos, con selección de centro y profesor.
- Consulta de aulas y lecciones.
- Matrículas, calificaciones y registro de asistencias.
- Tema claro u oscuro.

Las operaciones disponibles y las pendientes están en la [guía funcional](https://github.com/certidevs/g2_aspnet/wiki/Guia-funcional).

## Perfiles

| Perfil | Uso |
| --- | --- |
| `Admin` | Administra usuarios, consulta todas las matrículas y gestiona notas. |
| `Teacher` | Consulta y gestiona calificaciones; accede al detalle de matrículas. |
| `Student` | Consulta sus matrículas y notas y solicita matrícula para sí mismo. |

Tenemos pendientes los permisos de algunos módulos. La tabla completa está en [Autenticación y roles](https://github.com/certidevs/g2_aspnet/wiki/Autenticacion-y-roles).

## Stack

| Componente | Tecnología |
| --- | --- |
| Plataforma | .NET 10 y ASP.NET Core MVC con Razor. |
| Datos | Entity Framework Core 10.0.11 y SQLite. |
| Cuentas | ASP.NET Core Identity con cookies y roles. |
| Interfaz | Bootstrap 5 y jQuery, con bibliotecas locales. |
| Pruebas y entrega | xUnit, GitHub Actions y Docker. |

Consulta [Dependencias](https://github.com/certidevs/g2_aspnet/wiki/Dependencias) y [Arquitectura](https://github.com/certidevs/g2_aspnet/wiki/Arquitectura).

## Requisitos e instalación

Necesitamos el SDK de .NET 10 y Git. [global.json](https://github.com/certidevs/g2_aspnet/blob/main/global.json) establece la versión del SDK; la [guía de IDE](https://github.com/certidevs/g2_aspnet/wiki/Guia-de-IDE) explica cómo preparar el entorno.

```powershell
# Descarga el proyecto y restaura las dependencias.
git clone https://github.com/certidevs/g2_aspnet.git
cd g2_aspnet
dotnet restore G2AspNet.sln
dotnet tool restore

# Arranca la aplicación; este paso recrea la base de datos.
dotnet run --project G2AspNet.csproj --launch-profile http
```

Abre [http://localhost:5108](http://localhost:5108). El perfil HTTPS utiliza [https://localhost:7022](https://localhost:7022). Más detalles en [Primeros pasos](https://github.com/certidevs/g2_aspnet/wiki/Primeros-pasos).

## Configuración y base de datos

Los valores iniciales están en [appsettings.json](https://github.com/certidevs/g2_aspnet/blob/main/appsettings.json). Usamos SQLite en `App_Data/g2.db`. [DbInitializer](https://github.com/certidevs/g2_aspnet/blob/main/Data/DbInitializer.cs) borra y recrea la base en cada arranque, incluso con `SeedData:Enabled` desactivado.

Consulta [Configuración](https://github.com/certidevs/g2_aspnet/wiki/Configuracion), [Base de datos y migraciones](https://github.com/certidevs/g2_aspnet/wiki/Base-de-datos-y-migraciones) y [Modelo de datos](https://github.com/certidevs/g2_aspnet/wiki/Modelo-de-datos). Las cuentas de prácticas están en [Demostración](https://github.com/certidevs/g2_aspnet/wiki/Demostracion).

## Pruebas y Docker

```powershell
# Compila la solución y ejecuta las pruebas.
dotnet build G2AspNet.sln
dotnet test G2AspNet.sln

# Construye y ejecuta el contenedor.
docker build -t g2-aspnet .
docker run --rm -p 10000:10000 g2-aspnet
```

El contenedor se abre en [http://localhost:10000](http://localhost:10000). El proyecto tiene una prueba unitaria; tenemos pendiente ampliar la cobertura. Consulta [Testing y CI](https://github.com/certidevs/g2_aspnet/wiki/Testing-y-CI) y [Despliegue](https://github.com/certidevs/g2_aspnet/wiki/Despliegue).

## Estructura del proyecto

Los controladores están en [Controllers](https://github.com/certidevs/g2_aspnet/tree/main/Controllers), las entidades en [Models](https://github.com/certidevs/g2_aspnet/tree/main/Models), los datos en [Data](https://github.com/certidevs/g2_aspnet/tree/main/Data) y las pantallas en [Views](https://github.com/certidevs/g2_aspnet/tree/main/Views). La estructura completa se explica en [Arquitectura](https://github.com/certidevs/g2_aspnet/wiki/Arquitectura).

## Documentación

| Documento | Contenido |
| --- | --- |
| [Presentación del proyecto](https://github.com/certidevs/g2_aspnet/wiki/Presentacion-del-proyecto) | Descripción, recorrido y próximos pasos. |
| [Guía funcional](https://github.com/certidevs/g2_aspnet/wiki/Guia-funcional) | Funciones, permisos y trabajo pendiente. |
| [Modelo de datos](https://github.com/certidevs/g2_aspnet/wiki/Modelo-de-datos) | Entidades, relaciones y diagrama. |
| [Primeros pasos](https://github.com/certidevs/g2_aspnet/wiki/Primeros-pasos) | Requisitos, instalación y arranque. |
| [Arquitectura](https://github.com/certidevs/g2_aspnet/wiki/Arquitectura) | MVC, servicios y pipeline HTTP. |
| [Base de datos y migraciones](https://github.com/certidevs/g2_aspnet/wiki/Base-de-datos-y-migraciones) | SQLite y recreación de la base al arrancar. |
| [Autenticación y roles](https://github.com/certidevs/g2_aspnet/wiki/Autenticacion-y-roles) | Identity, cuentas y permisos actuales. |
| [Controladores y vistas](https://github.com/certidevs/g2_aspnet/wiki/Controladores-y-vistas) | Pantallas y enlaces al código de cada módulo. |
| [Configuración](https://github.com/certidevs/g2_aspnet/wiki/Configuracion) | Conexión, datos de ejemplo y subida de imágenes. |
| [Testing y CI](https://github.com/certidevs/g2_aspnet/wiki/Testing-y-CI) | Pruebas y GitHub Actions. |
| [Despliegue](https://github.com/certidevs/g2_aspnet/wiki/Despliegue) | Docker y tareas antes de publicar. |
| [Demostración](https://github.com/certidevs/g2_aspnet/wiki/Demostracion) | Cuentas de prácticas y recorrido. |
| [Problemas frecuentes](https://github.com/certidevs/g2_aspnet/wiki/Problemas-frecuentes) | Incidencias habituales y comprobaciones. |
| [Guía de contribución](https://github.com/certidevs/g2_aspnet/wiki/Guia-de-contribucion) | Trabajo compartido con Git y GitHub. |
| [Guía de IDE](https://github.com/certidevs/g2_aspnet/wiki/Guia-de-IDE) | Preparación del editor y del SDK. |
| [Dependencias](https://github.com/certidevs/g2_aspnet/wiki/Dependencias) | Paquetes y bibliotecas del proyecto. |

## Equipo

Grupo 2:

- [Poulgar](https://github.com/Poulgar)
- [rubenbitrian](https://github.com/rubenbitrian)
- [Dracorufo](https://github.com/Dracorufo)
- [alfonvazquez](https://github.com/alfonvazquez)

Las tareas se organizan en las [issues](https://github.com/certidevs/g2_aspnet/issues) y las aportaciones quedan en el [historial](https://github.com/certidevs/g2_aspnet/commits/main/).

## Licencia

Sin licencia declarada en la raíz del proyecto.

---

[Inicio](https://github.com/certidevs/g2_aspnet/wiki/Home) · [Documentación](https://github.com/certidevs/g2_aspnet/wiki/Home#documentación) · [Repositorio](https://github.com/certidevs/g2_aspnet) · [Comunicar un error](https://github.com/certidevs/g2_aspnet/issues)
