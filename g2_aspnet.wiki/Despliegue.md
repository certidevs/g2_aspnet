# Despliegue

Desde la raíz del repositorio:

```powershell
# Construye la imagen local.
docker build -t g2-aspnet .

# Inicia el contenedor y publica el puerto de la aplicación.
docker run --rm -p 10000:10000 g2-aspnet
```

Abre `http://localhost:10000`. El contenedor usa la variable `PORT` o, si no existe, el puerto `10000`.

El Dockerfile no configura volúmenes para conservar la base de datos y los avatares. Añadir un volumen tampoco resolvería el borrado de SQLite en cada arranque: antes habría que modificar su inicialización. Antes de usarlo con datos reales tenemos que completar los permisos, la persistencia y la configuración del entorno.

## Antes de publicar

- Retirar el borrado automático de la base de datos.
- Completar la autorización de los módulos.
- Revisar el registro y retirar las cuentas de demostración del entorno público.
- Preparar la conservación de SQLite y los avatares.
- Revisar HTTPS y la configuración del alojamiento.

El [Dockerfile](https://github.com/certidevs/g2_aspnet/blob/main/Dockerfile) prepara el contenedor; no configura por sí solo el dominio ni el alojamiento. Consulta [Configuración](https://github.com/certidevs/g2_aspnet/wiki/Configuracion) y [Testing y CI](https://github.com/certidevs/g2_aspnet/wiki/Testing-y-CI).

---

[Inicio](https://github.com/certidevs/g2_aspnet/wiki/Home) · [Documentación](https://github.com/certidevs/g2_aspnet/wiki/Home#documentación) · [Repositorio](https://github.com/certidevs/g2_aspnet) · [Comunicar un error](https://github.com/certidevs/g2_aspnet/issues)
