# Moogle!

![](moogle.png)

> Proyecto de Programación I.
> Facultad de Matemática y Computación - Universidad de La Habana.
> Cursos 2021, 2022.

Moogle! es un buscador simple que le permite al usuario encontrar en una base de documentos los más relevantes para un input dado.

## Ejecutando el proyecto

Lo primero que tendrás que hacer para poder trabajar en este proyecto es instalar .NET Core 6.0 (lo que a esta altura imaginamos que no sea un problema, ¿verdad?). Luego, solo te debes parar en la carpeta del proyecto y ejecutar en la terminal de Linux:

```bash
make dev
```

Si estás en Windows, debes poder hacer lo mismo desde la terminal del WSL (Windows Subsystem for Linux). Si no tienes WSL ni posibilidad de instalarlo, deberías considerar seriamente instalar Linux, pero si de todas formas te empeñas en desarrollar el proyecto en Windows, el comando *ultimate* para ejecutar la aplicación es (desde la carpeta raíz del proyecto):

```bash
dotnet watch run --project MoogleServer
```

Una vez iniciado el servidor, deberá mostrarse en la consola algo como:

```bash
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
```

Entrar en el navegador a la dirección:
```bash
http://localhost:5000
```

## Cómo buscar?



