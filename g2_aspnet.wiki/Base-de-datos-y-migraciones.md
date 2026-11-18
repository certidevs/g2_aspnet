# Base de datos y migraciones

Usamos SQLite con la conexión definida en [appsettings.json](https://github.com/certidevs/g2_aspnet/blob/main/appsettings.json). El archivo predeterminado es `App_Data/g2.db`.

## Qué ocurre al arrancar

1. Se crea la carpeta `App_Data` si hace falta.
2. Se ejecuta `DbInitializer.InitializeAsync`.
3. `EnsureDeleted()` elimina la base configurada y `EnsureCreated()` vuelve a crear las tablas.
4. Si `SeedData:Enabled` está activado, se cargan los roles y los datos de ejemplo.

**La base se borra también con `SeedData:Enabled` en `false`.** No apuntes la conexión a una base que necesites conservar.

## Cambios de esquema

En este proyecto no aplicamos migraciones de EF Core. Para probar un cambio en las entidades, volvemos a arrancar la aplicación y se genera el esquema. Los datos introducidos a mano se pierden; los ejemplos que necesitamos para clase están en [DbInitializer.cs](https://github.com/certidevs/g2_aspnet/blob/main/Data/DbInitializer.cs).

## Persistencia pendiente

Antes de trabajar con datos reales tenemos que retirar el borrado del arranque y preparar un mecanismo de actualización del esquema y copias de seguridad. Montar un volumen en Docker no evita el borrado actual.

Consulta las entidades y las reglas de borrado en [Modelo de datos](https://github.com/certidevs/g2_aspnet/wiki/Modelo-de-datos).

---

[Inicio](https://github.com/certidevs/g2_aspnet/wiki/Home) · [Documentación](https://github.com/certidevs/g2_aspnet/wiki/Home#documentación) · [Repositorio](https://github.com/certidevs/g2_aspnet) · [Comunicar un error](https://github.com/certidevs/g2_aspnet/issues)
