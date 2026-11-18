# Controladores y vistas

Cada módulo tiene su controlador y una carpeta de vistas Razor. En los formularios compartimos la vista `Form` cuando la misma pantalla sirve para crear y editar.

| Módulo | Controlador | Vistas |
| --- | --- | --- |
| Cuentas | [AccountController](https://github.com/certidevs/g2_aspnet/blob/main/Controllers/AccountController.cs) | [Views/Account](https://github.com/certidevs/g2_aspnet/tree/main/Views/Account) |
| Perfil | [ProfileController](https://github.com/certidevs/g2_aspnet/blob/main/Controllers/ProfileController.cs) | [Views/Profile](https://github.com/certidevs/g2_aspnet/tree/main/Views/Profile) |
| Usuarios | [UsersController](https://github.com/certidevs/g2_aspnet/blob/main/Controllers/UsersController.cs) | [Views/Users](https://github.com/certidevs/g2_aspnet/tree/main/Views/Users) |
| Centros | [AcademyController](https://github.com/certidevs/g2_aspnet/blob/main/Controllers/AcademyController.cs) | [Views/Academy](https://github.com/certidevs/g2_aspnet/tree/main/Views/Academy) |
| Aulas | [ClassroomController](https://github.com/certidevs/g2_aspnet/blob/main/Controllers/ClassroomController.cs) | [Views/Classroom](https://github.com/certidevs/g2_aspnet/tree/main/Views/Classroom) |
| Cursos | [CoursesController](https://github.com/certidevs/g2_aspnet/blob/main/Controllers/CoursesController.cs) | [Views/Courses](https://github.com/certidevs/g2_aspnet/tree/main/Views/Courses) |
| Lecciones | [LessonsController](https://github.com/certidevs/g2_aspnet/blob/main/Controllers/LessonsController.cs) | [Views/Lessons](https://github.com/certidevs/g2_aspnet/tree/main/Views/Lessons) |
| Matrículas | [EnrollmentsController](https://github.com/certidevs/g2_aspnet/blob/main/Controllers/EnrollmentsController.cs) | [Views/Enrollments](https://github.com/certidevs/g2_aspnet/tree/main/Views/Enrollments) |
| Notas | [GradesController](https://github.com/certidevs/g2_aspnet/blob/main/Controllers/GradesController.cs) | [Views/Grades](https://github.com/certidevs/g2_aspnet/tree/main/Views/Grades) |
| Asistencias | [AttendancesController](https://github.com/certidevs/g2_aspnet/blob/main/Controllers/AttendancesController.cs) | [Views/Attendances](https://github.com/certidevs/g2_aspnet/tree/main/Views/Attendances) |

## Interfaz compartida

La navegación, el acceso al perfil y el selector de tema están en [Views/Shared/_Layout.cshtml](https://github.com/certidevs/g2_aspnet/blob/main/Views/Shared/_Layout.cshtml). Los recursos locales están en [wwwroot](https://github.com/certidevs/g2_aspnet/tree/main/wwwroot).

En aulas y lecciones están activas las consultas de listado y detalle. Las acciones de crear, editar y borrar permanecen dentro de bloques comentados; tenemos que completarlas y activar sus vistas.

Los permisos actuales se detallan en [Autenticación y roles](https://github.com/certidevs/g2_aspnet/wiki/Autenticacion-y-roles).

---

[Inicio](https://github.com/certidevs/g2_aspnet/wiki/Home) · [Documentación](https://github.com/certidevs/g2_aspnet/wiki/Home#documentación) · [Repositorio](https://github.com/certidevs/g2_aspnet) · [Comunicar un error](https://github.com/certidevs/g2_aspnet/issues)
