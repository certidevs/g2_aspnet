# Guía funcional

| Área | Funciones presentes en el código |
| --- | --- |
| Cuentas | Registro, inicio de sesión por usuario o correo, cierre de sesión y página de acceso denegado. |
| Perfil | Consulta y edición de datos personales, avatar y cambio de contraseña. |
| Usuarios | Administración de cuentas, búsqueda, creación, edición, asignación de rol, activación y eliminación. |
| Centros | Listado, búsqueda por nombre, filtro por ciudad, detalle, creación, edición y eliminación. |
| Cursos | Listado con búsqueda, detalle, creación, edición y eliminación; selección de centro y profesor. |
| Aulas | Consulta del listado y del detalle. Las acciones de creación, edición y eliminación están comentadas. |
| Lecciones | Consulta del listado y del detalle, con curso, aula y asistencias. Las acciones de creación, edición y eliminación están comentadas. |
| Matrículas | Solicitud de matrícula, consulta, edición y eliminación con comprobaciones de propietario y administrador. |
| Calificaciones | Consulta, creación, edición y eliminación; selección de matrícula mediante un desplegable. |
| Asistencias | Listado con búsqueda, detalle, creación, edición y eliminación; comprobación de duplicados por lección y matrícula. |
| Interfaz | Navegación adaptable y tema claro u oscuro. |

Tenemos pendientes las altas, modificaciones y bajas de aulas y lecciones. El código de ejemplo sigue comentado.

## Perfiles y permisos

Los roles definidos son `Admin`, `Teacher` y `Student`. El registro público asigna actualmente el rol `Teacher` a las nuevas cuentas.

| Área | Comportamiento actual |
| --- | --- |
| Administración de usuarios | Requiere el rol `Admin`. |
| Perfil | Requiere iniciar sesión. |
| Listado de matrículas | `Admin` ve todas; las demás cuentas ven aquellas cuyo `StudentId` coincide con su identificador. Esto también afecta a `Teacher`. |
| Detalle de una matrícula | `Admin` y `Teacher` pueden consultar cualquier matrícula; las demás cuentas, solo la propia. |
| Creación de matrícula | Cualquier cuenta autenticada puede crear una matrícula para sí misma. |
| Edición y eliminación de matrícula | El propietario o `Admin`. Al editar, solo `Admin` puede cambiar `IsPaid`. |
| Consulta de notas | `Admin` y `Teacher` ven todas; el resto de cuentas autenticadas ve sus propias notas. |
| Creación, edición y eliminación de notas | Requieren `Admin` o `Teacher`. |
| Centros, cursos y asistencias | Sus controladores no exigen autenticación ni un rol. Las operaciones de escritura presentes también carecen de esa restricción. |
| Aulas y lecciones | Las consultas activas no exigen autenticación. |

No existe una política general que obligue a iniciar sesión en toda la aplicación. La protección antifalsificación de formularios no sustituye a los permisos. El acceso del profesorado a notas y detalles de matrículas tampoco se limita a sus cursos.

Código relacionado: [controladores](https://github.com/certidevs/g2_aspnet/tree/main/Controllers) y [configuración del pipeline](https://github.com/certidevs/g2_aspnet/blob/main/Program.cs).

## Trabajo pendiente

- Completar las operaciones de aulas y lecciones.
- Cerrar los permisos de centros, cursos y asistencias.
- Revisar el rol asignado en el registro público.
- Limitar el acceso del profesorado a los cursos que corresponda.
- Ampliar las pruebas de matrículas, notas y asistencias.

Consulta [Autenticación y roles](https://github.com/certidevs/g2_aspnet/wiki/Autenticacion-y-roles).

---

[Inicio](https://github.com/certidevs/g2_aspnet/wiki/Home) · [Documentación](https://github.com/certidevs/g2_aspnet/wiki/Home#documentación) · [Repositorio](https://github.com/certidevs/g2_aspnet) · [Comunicar un error](https://github.com/certidevs/g2_aspnet/issues)
