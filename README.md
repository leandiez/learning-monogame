# Monogame Learning

### Proyecto en 2D para practicar y aprender programacion usando el framework Monogame.

Este proyecto fue creado siguiendo el tutorial ["Construyendo juegos 2D con Monogame"](https://docs.monogame.net/articles/tutorials/building_2d_games/index.html),
y esta adaptado al Español, intentando sumar mi propia experiencia al codigo del mismo, esperando que pueda serle de utilidad a alguien mas.

La idea es realizar tanto la implementacion del juego, como el despliegue en distintas plataformas (Windows, Linux y MacOS) y la subida al sitio de [Itch.io](https://itch.io), completando un circuito de desarrollo completo.

El juego desarrollado, es un clon del clasico juego de Snake (o la Viborita como le llamamos en mi pais), con algunas variantes para entender ciertas mecanicas del Framework.
Se compone de:
- Una pantalla de Titulo con un fondo animado.
- Una pantalla de configuracion de Opciones (no persistentes).
- Un mapa 2D hecho con una grilla (Tileset + Tilemaps).
- 2 Sprites que representan al Slime (viborita), y al Murcielago (la comida), que son los que interactuan para dar forma al juego.
- Un gestor de escenas para manejar el contenido en pantalla.
- Gestor de musica y efectos de sonido.
- Gestor de entrada con soporte a Teclado, Mouse y Joysticks.

Algunas cosas a mejorar o que podrian implementarse como EXTRA del tutorial original:
- Persistir el puntaje y las configuraciones tras cerrar el juego.
- Añadir niveles y transiciones entre escenas.
- Modificar la musica y graficos originales, o bien agregar un mecanismo de skins para intercambiarlas.
- Añadir efectos usando Shaders existentes. (Si sale modificar o crear alguno mucho mejor).

## Primeros pasos

Para poner algo de contexto, Monogame es un Framework de .NET, creado como sucesor OpenSource de la libreria deprecada por Microsoft, XNA.
Si bien comparte las mismas funciones basicas, Monogame le ha sumado el soporte multiplataforma, y varias utilidades para la gestion de Contenido, convirtiendola lentamente en un Framework nuevo.
Puede encontrarse mas informacion en el siguiente link: https://docs.monogame.net/articles/index.html

### Prerequisitos

Para poder configurar el entorno de desarrollo, y descargar Monogame, es necesario tener los siguientes puntos preparados.
Cabe destacar que si bien intento mantener los pasos lo mas general posible, trabajo sobre Windows, con lo cual para configurar otros entornos sugiero seguir los tutoriales de Monogame. 

[Tutorial Ubuntu](https://docs.monogame.net/articles/getting_started/1_setting_up_your_os_for_development_ubuntu.html?tabs=android)

[Tutorial MacOS](https://docs.monogame.net/articles/getting_started/1_setting_up_your_os_for_development_macos.html?tabs=android)

Pasos a seguir:

- Instalar .NET SDK 8.0 o superior
- Instalar los Templates de Proyectos de Monogame usando el siguiente comando:


    dotnet new install MonoGame.Templates.CSharp

- Instalar un editor de codigo. Los soportados por Monogame son VS Code y Visual Studio. El editor Riders tiene soporte de la comunidad. Otros editores pueden usarse pero a responsabilidad del usuario.
- En caso de elegir VS Code, Visual Studio o Riders, instalar los plugins de Monogame correspondientes para mejorar las funciones de debug y del gestor de Contenido.

Con estas dependencias instaladas, se puede proceder a clonar el repositorio y comenzar a modificar o testear el juego.

### Como crear nuevos proyectos de Monogame

Para inicializar un proyecto de Monogame, pueden utilizarse las plantillas instaladas anteriormente con el comando

    dotnet new nombreDelTemplate -n nombreDelProyecto

Los templates existentes se pueden listar con el siguiente comando:

    dotnet new list --tag MonoGame


## Estructura del proyecto

El Juego esta compuesto por 2 proyectos de .NET 

### MyFirstGameLibrary
Contiene la logica "compartida" del juego, esta implementado con el Template de Shared Library, y esta pensado para implementar elementos reutilizables.

Esta dividido en los siguientes modulos:

- Audio: funciones para cargar y reproducir Musica y Efectos de Sonido (SFX)
- Graphics: funciones para carga de texturas y algunas estructuras basicas como Sprites, Animaciones y Tilemaps.
- Inputs: funciones para llamar a la API de XNA y obtener el estado de las entradas. Soporta Teclado, Mouse y Joysticks.
- Primitives: son estructuras (mayormente structs) para representar objetos basicos. Actualmente solo implementa un Circulo como forma geometrica, ya que XNA solo maneja Rectangulos.
- Scenes: objeto para representar una escena. De momento se gestiona con la clase Core.cs
- Core.cs: objeto base para cargar la libreria. Implementa la inicializacion de Monogame, y la carga de objetos basicos como escenas y gestor de Inputs.

### DungeonSlimeGame
Implementa la logica del juego, y cada componente del mismo.

El proyecto arranca con la clase **Game2.cs** que hereda de Core.cs y realiza el proceso de inicializar la escena y cargar los objetos necesarios para la misma.

La carpeta **Actors** contiene la logica del Murcielago y el Slime.

La carpeta **Content** contiene los assets graficos, de sonido y datos que se utilizan en el codigo del juego. Es una carpeta que esta gestionada por el Content Manager de Monogame y por ello debe usarse su editor para añadir o modificar assets. 
Mas informacion puede encontrarse en [este](docs/learning.md) documento hecho por mi, o bien en la propia [documentacion](https://docs.monogame.net/articles/getting_started/content_pipeline/why_content_pipeline.html) de Monogame.

La carpeta Scenes, contiene la implementacion de las escenas propiamente del juego. Realizan la carga de assets, y ejecutan el ciclo de Update y Draw a cada frame.

La carpeta UI, implementa la interfaz de usuario (menus, botones, carteles, etc.) utilizando la libreria GumUI, que es recomendada en el tutorial para facilitar la gestion de esta funcionalidad.

La carpeta Utils, contiene logica de soporte para facilitar o mejorar lo implementado. 
Actualmente solo incluye la clase **GameActions.cs** la cual usando el patron Comando, permite abstraer las acciones del juego contra los distintos botones de entrada, permitiendo modificar los mismos sin afectar al codigo que monitorea las acciones del usuario (moverse, confirmar, cancelar).  
Una buena referencia respecto al patron comando puede encontrarse en [este libro gratuito](https://gameprogrammingpatterns.com/command.html) de Robert Nystrom.


## Compilacion

El proyecto esta generado utilizando el template DesktopGL, con lo cual pueden crearse versiones para Windows, Linux y
MacOS. Para la automatizacion de este proceso se creo un Workflow de GitHub el cual produce los 3 paquetes y los 
publica como Releases.

El proceso manual incluye los siguientes pasos:

- Buildear el proyecto principal segun la plataforma adecuada (Windows, Linux, MAC)
- Empaquetar los archivos compilados en un instalable para cada plataforma.
- - Si es Windows o Linux solo se deben configurar permisos de ejecucion y Zippear la carpeta
- - Si es MacOS, se deben generar una serie de archivos y estructura para poder instalar la aplicacion como DMG.

Para mayor detalle puede consultarse en el capitulo 25 del Tutorial 2D de Monogame: 

https://docs.monogame.net/articles/tutorials/building_2d_games/25_packaging_game/index.html?tabs=windows

## Problemas conocidos

### Compilacion de Shaders en ArchLinux
Para compilar el contenido con Efectos, se debe contemplar una configuracion particular en Linux, la cual requiere 
de Wine y una version de dotnet y FXCSS particular. 
Hay un script en el sitio de Monogame con el detalle de la configuracion, sin embargo la ultima version de Wine (10.20) 
parece fallar con la libreria "icu.dll" e "icuuc.dll".

De momento deshabilitando estas librerias usando winecfg, deberia ser suficiente para resolver el problema.

## Licencia

Este proyecto se encuentra licenciado bajo GPLv3, puedes ver el texto original de la misma
en el archivo [LICENSE.md](LICENSE.md)

En caso de necesitar una traduccion, utilizar el siguiente enlace: https://lslspanish.github.io/translation_GPLv3_to_spanish/
