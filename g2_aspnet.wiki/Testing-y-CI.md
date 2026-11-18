# Testing y CI

El proyecto de pruebas contiene una prueba unitaria, `Failure_DoesNotReportASuccessfulUpload`, que comprueba el resultado de una subida fallida mediante `ImageUploadResult`.

De momento solo tenemos esta prueba unitaria. Nos falta cubrir los permisos, los controladores, los formularios y las reglas del dominio académico.

El [workflow de GitHub Actions](https://github.com/certidevs/g2_aspnet/blob/main/.github/workflows/build-and-test.yml) está configurado para ejecutarse en los envíos a `main`, las solicitudes de incorporación hacia `main` y la ejecución manual. Restaura dependencias, compila en `Release` y ejecuta las pruebas en Ubuntu.

Para comprobar el resultado de cada ejecución, abre la pestaña [Actions](https://github.com/certidevs/g2_aspnet/actions).

## Comprobación local

```powershell
# Restaura, compila y ejecuta las pruebas.
dotnet restore G2AspNet.sln
dotnet build G2AspNet.sln --configuration Release --no-restore
dotnet test G2AspNet.sln --configuration Release --no-build
```

Antes de dar una funcionalidad por terminada, comprobamos también el formulario y los accesos con los distintos perfiles.

---

[Inicio](https://github.com/certidevs/g2_aspnet/wiki/Home) · [Documentación](https://github.com/certidevs/g2_aspnet/wiki/Home#documentación) · [Repositorio](https://github.com/certidevs/g2_aspnet) · [Comunicar un error](https://github.com/certidevs/g2_aspnet/issues)
