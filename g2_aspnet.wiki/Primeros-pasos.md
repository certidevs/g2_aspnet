# Primeros pasos

## Requisitos

- SDK de .NET 10. `global.json` establece `10.0.100`, permite versiones posteriores de la misma línea mediante `latestFeature` y excluye versiones preliminares.
- Git para descargar el repositorio.
- Visual Studio Code con C# Dev Kit, o un IDE compatible con .NET 10.
- Docker, únicamente si se quiere ejecutar mediante contenedor.

No es necesario instalar SQLite como servidor independiente.

Consulta la [guía de preparación del IDE](https://github.com/certidevs/g2_aspnet/blob/main/docs/IDE-SETUP.md).

## Instalación y ejecución

Ejecuta estos comandos desde una terminal. El último inicia la aplicación y recrea la base de datos.

```powershell
# Descarga el proyecto y entra en su carpeta.
git clone https://github.com/certidevs/g2_aspnet.git
cd g2_aspnet

# Restaura las dependencias y las herramientas locales.
dotnet restore G2AspNet.sln
dotnet tool restore

# Inicia el perfil HTTP de desarrollo.
dotnet run --project G2AspNet.csproj --launch-profile http
```

Abre `http://localhost:5108`. El perfil `https` configura `https://localhost:7022` y `http://localhost:5108`; puede requerir preparar el certificado HTTPS de desarrollo.

Para compilar y ejecutar las pruebas disponibles:

```powershell
# Compila la solución.
dotnet build G2AspNet.sln

# Ejecuta las pruebas de la solución.
dotnet test G2AspNet.sln
```

Los perfiles y puertos están en [launchSettings.json](https://github.com/certidevs/g2_aspnet/blob/main/Properties/launchSettings.json).

Para preparar el editor, consulta la [guía de IDE](https://github.com/certidevs/g2_aspnet/wiki/Guia-de-IDE). Si algo falla, revisa [Problemas frecuentes](https://github.com/certidevs/g2_aspnet/wiki/Problemas-frecuentes).

---

[Inicio](https://github.com/certidevs/g2_aspnet/wiki/Home) · [Documentación](https://github.com/certidevs/g2_aspnet/wiki/Home#documentación) · [Repositorio](https://github.com/certidevs/g2_aspnet) · [Comunicar un error](https://github.com/certidevs/g2_aspnet/issues)
