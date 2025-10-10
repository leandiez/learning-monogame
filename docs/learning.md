# Conceptos aprendidos durante la creacion de este proyecto

[Volver al Readme.md](../README.md)

## Content Pipeline

En Monogame todos los recursos que no sean codigo (imagenes, audio, efectos, datos de juego, etc) se les llama Contenido. 
Este contenido debe ser procesado por 2 componentes clave, el Importador y el Procesador. Ambos se ejecutan en tiempo de compilacion del juego (Build Time).
El Importador se encarga de convertir el formato de origen del archivo a un objeto del DOM de Monogame (ejemplo mallas, vertices, materiales, texturas) que luego puede usarse tanto por un procesador como por otras clases custom que lo puedan requerir en Build Time.
El Procesador por otra parte, toma un contenido especifico que ya ha sido importado y lo convierte en un objeto XBN (binario) para poder ser usado en tiempo de ejecucion (Runtime).
Ambos componentes pueden ser extendidos para soportar nuevos formatos o cambiar la informacion que se trae cada tipo de contenido, usando las librerias "Microsoft.Xna.Framework.Content.Pipeline"

Luego, en Runtime, se puede usar el contenido generado usando la llamada "ContentManager.Load" y el objeto devuelto por el resto de las librerias del Framework de Monogame ("Microsoft.Xna.Framework")

Para desarrollar un formato propio o nuevo, se debe importar la libreria custom en el editor de MGCB, de momento no encontre mayor documentacion al respecto, sino mas bien que los Standard suelen ser suficientes.

## Uso de librerias

Como el Framework es basado en XNA y funciona totalmente sobre .NET y C#, es importante tener en cuenta la modularizacion y el separar las capas usadas en paquetes, o librerias DLL.

Para esto, existe un template llamado Game Library, el cual genera un proyecto nuevo compilable en una .DLL la cual luego puede referenciarse desde otros proyectos o incluso distribuirse mediante paquetes por NuGet.