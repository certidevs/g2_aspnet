# G2 · Gestión de academias y centros de formación

Repositorio del **Grupo 2** del curso de .NET: aplicación web con **C#, ASP.NET Core MVC, Razor y Entity Framework Core**, desarrollada en equipo con Git y GitHub.

## Temática: a elegir por el equipo

Elegid **una** de estas temáticas durante el fin de semana. Todas parten del mismo esqueleto (usuarios con roles `User` y `Admin`); el dominio lo construís vosotros.

6. **Gestión de academias y centros de formación:** administración de alumnos, profesores, cursos, matrículas y calificaciones.

Cuando el equipo lo haya decidido, sustituid esta sección por la descripción de la temática elegida y de sus funcionalidades principales (por ejemplo, con una Pull Request).

## Qué incluye el esqueleto

Punto de partida común de todos los grupos: **solo la entidad `ApplicationUser` (usuario)** y la infraestructura para que todo arranque a la primera. Las entidades del dominio, sus relaciones, servicios, controladores y vistas las construye el equipo.

| Área | Contenido |
| --- | --- |
| Usuarios | `ApplicationUser` (ASP.NET Core Identity) con nombre visible, avatar, fecha de alta y estado activo; roles `Admin` y `User`. |
| Cuenta | Registro, login por usuario o email, logout y página de acceso denegado. |
| Perfil | Consulta y edición del perfil, avatar y cambio de contraseña. |
| Administración | Panel `/Users` (solo `Admin`): alta, edición, activación y borrado de usuarios. |
| Datos | Entity Framework Core + SQLite (`App_Data/g2.db`), base recreada y datos demo al arrancar. |
| Imágenes | `ImageStorage` para guardar imágenes subidas; hoy solo avatares (añadid vuestras carpetas en `ImageFolder`). |
| Interfaz | Razor + Bootstrap 5 en local, con tema claro/oscuro. |
| Calidad y despliegue | Proyecto de tests xUnit, GitHub Actions (compilar y testear) y `Dockerfile`. |

## Requisitos

- SDK de .NET 10. `global.json` acepta cualquier SDK de la versión 10.0.
- Visual Studio Code y C# Dev Kit. Es el IDE estándar del curso en Windows, macOS y
  Linux.
- Git. Docker es opcional y solo se usa al explicar despliegue.

No instales ASP.NET, C#, SQLite ni Entity Framework por separado: los aporta el SDK o
se restauran como dependencias del proyecto.

## Instalación inicial

Instala .NET 10 SDK, Visual Studio Code y C# Dev Kit una sola vez en el ordenador.

### Windows (PowerShell)

```powershell
winget install --id Microsoft.DotNet.SDK.10 --exact
winget install --id Microsoft.VisualStudioCode --exact
winget install --id Git.Git --exact
```

Cierra y abre una terminal nueva; después instala la extensión:

```powershell
code --install-extension ms-dotnettools.csdevkit
```

### macOS (Terminal, con Homebrew)

```bash
brew install dotnet
brew install --cask visual-studio-code
brew install git
code --install-extension ms-dotnettools.csdevkit
```

Si no utilizas Homebrew, instala el SDK de .NET 10 con el instalador oficial de
[macOS](https://dotnet.microsoft.com/download/dotnet/10.0) y añade C# Dev Kit desde
el panel Extensions de VS Code.

### Ubuntu 26.04 (Terminal)

```bash
sudo apt-get update
sudo apt-get install -y dotnet-sdk-10.0 git
sudo snap install code --classic
code --install-extension ms-dotnettools.csdevkit
```

Para otra distribución Linux, sigue el instalador oficial de
[.NET para Linux](https://learn.microsoft.com/dotnet/core/install/linux) y, una vez
instalado VS Code, ejecuta el último comando.

Comprueba la instalación desde esta carpeta:

```bash
dotnet --version
```

Debe mostrar una versión que empiece por `10.0`.
Si el comando `code` no se reconoce, abre VS Code e instala C# Dev Kit desde
**Extensions**.

## Primer arranque

Clona el repositorio y abre **su carpeta** (no una carpeta superior) en VS Code. C# Dev Kit carga automáticamente `G2AspNet.sln`.

```bash
git clone https://github.com/certidevs/g2_aspnet.git
cd g2_aspnet
code .
dotnet restore G2AspNet.sln
dotnet tool restore
dotnet run --project G2AspNet.csproj --launch-profile http
```

Abre `http://localhost:5108`. El primer arranque crea `App_Data/g2.db`, crea las tablas y añade los usuarios demo:

| Usuario | Contraseña | Rol |
| --- | --- | --- |
| `admin` | `Admin123!` | Admin |
| `user` | `User123!` | Teacher (profesor) |
| `teacher` | `Teacher123!` | Teacher (profesor) |
| `student` | `Student123!` | Student (alumno) |
| `student2` | `Student123!` | Student (alumno) |
| `student3` | `Student123!` | Student (alumno) |

## Trabajo en equipo en GitHub

Todo el grupo tiene permiso de escritura en este repositorio. Para no pisaros el trabajo:

1. **Se trabaja directamente en `main`.** Quien ya sepa usar ramas puede usarlas, pero no es obligatorio.
2. **Siempre `git pull` antes de `git push`** (y también antes de ponerte a trabajar, para tener lo último de los demás).
3. **Un commit pequeño por cada cambio**, con un mensaje claro que diga qué has hecho y la issue a la que corresponde (por ejemplo, `#12 Añadir el campo IsPaid a Enrollment`).
4. **No toques la entidad, el controlador ni las vistas de otra persona sin avisarle antes.** La tabla de abajo dice quién hace cada cosa.
5. **No hagáis `git push --force`.**
6. Repartid el trabajo con **Issues** (una persona por issue) y, si queréis, un tablero de **Projects**.

El ciclo de trabajo diario es siempre el mismo:

```bash
git pull
# haces un cambio pequeño y compruebas que la aplicación arranca
git add <fichero>
git commit -m "#12 Descripción corta del cambio"
git pull
git push
```

### Entidades y quién las hace

Tomado de las issues del repositorio. El responsable es la persona asignada a la issue; la entidad, su controlador y sus vistas tienen el mismo responsable.

| Entidad | Controlador | Vistas | Responsable | Ya hecho y pendiente |
| --- | --- | --- | --- | --- |
| `Academy` (#5) | `AcademyController` (#17) | `Views/Academy` (#18) | Poulgar | **Ejemplo completo**: listado, detalle, crear, editar, borrar y formulario. |
| `Classroom` (#13) | `ClassroomController` (#19) | `Views/Classroom` (#20) | Poulgar | Listado y detalle hechos. Pendiente: crear, editar, borrar y formulario. |
| `Course` (#6) | `CoursesController` (#21) | `Views/Courses` (#22) | rubenbitrian | Listado, detalle, formulario, crear, editar y borrar hechos. Pendiente: que el formulario permita elegir centro y profesor. |
| `Lesson` (#7) | `LessonsController` (#25) | `Views/Lessons` (#26) | Dracorufo | Listado y detalle hechos. Pendiente: crear, editar, borrar y formulario. |
| `Attendance` (#14) | `AttendancesController` (#27) | `Views/Attendances` (#28) | rubenbitrian | Listado, detalle, crear (con formulario) y aviso de duplicados hechos. Pendiente: editar y borrar. |
| `Enrollment` (#12) | `EnrollmentsController` (#23) | `Views/Enrollments` (#24) | alfonvazquez | Listado, detalle, formulario, crear, editar y borrar hechos. |
| `Grade` (#4) | `GradesController` (#10) | `Views/Grades` (#9) | alfonvazquez | Listado, detalle, formulario, crear, editar y borrar hechos. Pendiente: elegir la matrícula en una lista desplegable. |

**Ejemplo para copiar:** `Academy` es la entidad completa y funcionando (`AcademyController` y `Views/Academy/Form.cshtml`). En `Classroom` y `Lesson` el código de crear, editar y borrar ya está escrito pero **comentado**: buscad el marcador `DESCOMENTA` en su controlador, su `Form.cshtml`, `Index` y `Details`, descomentadlo y ajustadlo.

### Base de datos en desarrollo

Este proyecto **no usa migraciones**. Al arrancar, la aplicación borra `App_Data/g2.db`, la vuelve a crear a partir de las entidades y carga los datos demo de `Data/DbInitializer.cs` (es el equivalente a `create-drop` con H2 en Spring Boot). Por eso:

- Si cambias una entidad o una relación, **solo tienes que volver a ejecutar la aplicación**.
- Si haces `git pull` con cambios de otras personas, **solo tienes que volver a ejecutarla**.
- No hay que ejecutar `dotnet ef` ni borrar la base a mano.
- Los datos que crees a mano al probar se pierden al reiniciar. Lo que deba existir siempre se añade a `DbInitializer`.

## Comandos de trabajo diario

Todos se ejecutan desde la raíz de este repositorio.

```bash
# Restaurar dependencias NuGet y herramientas locales tras clonar o actualizar
dotnet restore G2AspNet.sln
dotnet tool restore

# Ejecutar la aplicación
dotnet run --project G2AspNet.csproj --launch-profile http

# Ejecutar con recarga al guardar archivos
dotnet watch --project G2AspNet.csproj run --launch-profile http

# Compilar y ejecutar los tests
dotnet build G2AspNet.sln
dotnet test G2AspNet.sln
```

### Entity Framework y SQLite

La base de datos se recrea al arrancar (ver *Base de datos en desarrollo*): no hay migraciones que crear ni aplicar.

### CI y Docker

[GitHub Actions](.github/workflows/build-and-test.yml) ejecuta restauración, compilación Release y tests en cada `push` a `main` y en cada *pull request* hacia `main`.

Docker es opcional:

```bash
docker build -t g2_aspnet:local .
docker run --rm -p 10000:10000 g2_aspnet:local
```

El contenedor queda disponible en `http://localhost:10000`.

## IDEs

En VS Code, selecciona el perfil `http` y pulsa F5 para depurar. Las tareas `G2: compilar` y `G2: ejecutar tests` están disponibles en **Terminal → Run Task**. En Windows, Visual Studio 2026 también abre `G2AspNet.sln`; consulta [la guía de IDEs](docs/IDE-SETUP.md).

[ Firmado por Poulgar usando Push ]
[ Firmado por Alfonso usando Push y rama auxiliar]
[ Firmado por Rubén usando Push y sin rama auxiliar]
[ Firmado por Juanma usando Push y rama auxiliar]
