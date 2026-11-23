# Autenticación y roles

Usamos ASP.NET Core Identity con cookies y los roles `Admin`, `Teacher` y `Student`. La configuración está en [Program.cs](https://github.com/certidevs/g2_aspnet/blob/main/Program.cs), y los nombres de los roles en [RoleNames.cs](https://github.com/certidevs/g2_aspnet/blob/main/Models/RoleNames.cs).

## Cuentas y sesión

El acceso admite nombre de usuario o correo. Las cuentas tienen un estado activo que también se comprueba al validar la sesión. La configuración establece contraseñas de al menos ocho caracteres, con un dígito y un carácter especial, y bloqueo de diez minutos tras cinco intentos fallidos.

El registro público asigna `Teacher`; tenemos pendiente revisar este comportamiento antes de abrir el registro para un uso real.

## Permisos

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

## Formularios

Tenemos un filtro global `AutoValidateAntiforgeryTokenAttribute` para las operaciones que modifican datos. Este filtro protege frente a peticiones falsificadas, pero los permisos de cada acción se tienen que configurar aparte.

Consulta los flujos disponibles en la [guía funcional](https://github.com/certidevs/g2_aspnet/wiki/Guia-funcional).

---

[Inicio](https://github.com/certidevs/g2_aspnet/wiki/Home) · [Documentación](https://github.com/certidevs/g2_aspnet/wiki/Home#documentación) · [Repositorio](https://github.com/certidevs/g2_aspnet) · [Comunicar un error](https://github.com/certidevs/g2_aspnet/issues)
