# Moogle!

![](moogle.png)

> Proyecto de Programación I.
> Facultad de Matemática y Computación - Universidad de La Habana.
> Cursos 2021, 2022.

Moogle! es un buscador simple que le permite al usuario encontrar en una base de documentos los más relevantes para un input dado.

## Ejecutando el proyecto
1. Entrar a la carpeta `script`.
2. Abrirla desde el terminal
3. Ejecutar el siguiente comando
```bash
./proyecto.sh run
```

que es lo mismo internamente que ejecutar desde la raíz del proyecto el comando:

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

En la barra de búsqueda escriba las palabras que quiere buscar (note que puede buscar no solo palabras,
sino una frase en sí) y presione el botón 🔍 Search

## Qué me mostrará el buscador una vez realizada mi consulta

Se le mostrará una lista de documentos que contengan las palabras que aparecen en su búsqueda.
      Si escribe una consulta vacía el buscador se lo informará.
      Se mostrarán primero los documentos que contienen palabras más raras (aparecen en la consulta pero aparecen en menos documentos).
      Las palabras que son demasiado comunes (aparecen en todos los documentos) no serán relevantes
      y por tanto ignoradas por la búsqueda

## Funcionalidades opcionales para la búsqueda

Sugerencias:
Si no se encuentra ningún resultado para su búsqueda se mostrará una sugerencia de lo que debería escribir en su consulta para tener resultados. Esta sugerencia está basada en palabras ya contenidas en los textos que sean lo más parecidas a la consulta inicial.
Lucirá de la manera siguiente:
```bash
🤔 ¿Quisite decir "sugerencia"?
```

Puede introducir los siguientes operadores para la búsqueda:
```bash
!palabra
```
Los documentos que contengan las palabras que tengan un signo ! delante, no serán sugeridos

```bash
^palabra
```
Solo podrán ser sugeridos documentos que contengan las palabras que tengan un signo ^ delante

```bash
*palabra
```
Cualquier cantidad de * delante de una palabra aumenta la relevancia de esta para la búsqueda

```bash
palabra1~palabra2
```
Para dos o más palabras unidas por ~, entre más cerca estén esas palabras en un documento más relevante será este

Para usar los operadores en una consulta es importante respetar las siguientes reglas:
      Los operadores `!` `^` `*` deben estar pegados a la palabra de lo contrario la búsqueda se realizará pero estos no tendrán efecto
      No deben aparecer operadores intercalados. Por ejemplo: `!^` o `*^`
      Todos los operadores, menos `~` pueden repetirse. Si lo repetimos, este será ignorado

## Nota:
No se recomienda usar símbolos extraños que no sean considerados letras o números en la búsqueda, puede que no obtenga los resultados esperados
