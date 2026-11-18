# Problemas frecuentes

| Problema | Comprobación |
| --- | --- |
| El SDK no es compatible | Comprobar `dotnet --version` y los requisitos de `global.json`. |
| Se han perdido los cambios de datos | Es el comportamiento actual de `DbInitializer` al reiniciar. |
| No aparecen los datos de ejemplo | Revisar `SeedData:Enabled` y las variables de entorno que lo sobrescriben. |
| No aparecen opciones para crear aulas o lecciones | Sus acciones y formularios de ejemplo están desactivados. |
| No se puede borrar un registro | Revisar las relaciones y los avisos de historial asociado. |
| Un profesor no ve matrículas en el listado | El listado filtra por propietario para cualquier rol distinto de `Admin`. |
| El README y la aplicación discrepan | Algunos apartados del README siguen describiendo el esqueleto inicial; revisar el controlador y la vista correspondientes. |

Si el problema continúa, abre una [issue](https://github.com/certidevs/g2_aspnet/issues) indicando la pantalla, los pasos, el resultado esperado y el error. No incluyas contraseñas ni datos personales.

---

[Inicio](https://github.com/certidevs/g2_aspnet/wiki/Home) · [Documentación](https://github.com/certidevs/g2_aspnet/wiki/Home#documentación) · [Repositorio](https://github.com/certidevs/g2_aspnet) · [Comunicar un error](https://github.com/certidevs/g2_aspnet/issues)
