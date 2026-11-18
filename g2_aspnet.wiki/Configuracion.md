# Configuración

Los valores predeterminados están en [appsettings.json](https://github.com/certidevs/g2_aspnet/blob/main/appsettings.json).

| Clave | Valor predeterminado o función |
| --- | --- |
| `ConnectionStrings:DefaultConnection` | `Data Source=App_Data/g2.db`. Conexión SQLite. |
| `SeedData:Enabled` | `true`. Carga roles, cuentas y datos de demostración. |
| `FileUploads:Images:MaxFileSizeBytes` | `5242880`, equivalente a 5 MiB. |
| `FileUploads:Images:AllowedExtensions` | `.jpg`, `.jpeg`, `.png`, `.gif` y `.webp`. |
| `Logging:LogLevel` | Niveles del registro de mensajes. |
| `AllowedHosts` | `*`. |

Las claves pueden sobrescribirse mediante variables de entorno utilizando `__` entre niveles, por ejemplo `SeedData__Enabled`.

**Desactivar `SeedData:Enabled` no evita el borrado de la base de datos.** Únicamente omite la carga posterior de datos de ejemplo, incluidos los roles y las cuentas.

Consulta también [Base de datos y migraciones](https://github.com/certidevs/g2_aspnet/wiki/Base-de-datos-y-migraciones) y [Despliegue](https://github.com/certidevs/g2_aspnet/wiki/Despliegue).

---

[Inicio](https://github.com/certidevs/g2_aspnet/wiki/Home) · [Documentación](https://github.com/certidevs/g2_aspnet/wiki/Home#documentación) · [Repositorio](https://github.com/certidevs/g2_aspnet) · [Comunicar un error](https://github.com/certidevs/g2_aspnet/issues)
