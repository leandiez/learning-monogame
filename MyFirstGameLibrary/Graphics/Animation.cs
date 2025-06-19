namespace MyFirstGameLibrary.Graphics;
// Referencia a los frames usados para la animacion y el tiempo entre pasaje en milisegundos
// Para graficar en pantalla ver AnimatedSprite
using System;
using System.Collections.Generic;
// Buscar significado de esta sintaxis. Creo que es para inicializar valores de construccion sin tener que poner un bloque especifico.
public class Animation(List<TextureRegion> frames, TimeSpan delay)
{
    public List<TextureRegion> Frames {get;set;} = frames;
    public TimeSpan Delay {get;set;} = delay;
    //El constructor vacio, inicializa al AnimatedSprite con los parametros definidos
    public Animation() : this(new List<TextureRegion>(), TimeSpan.FromMilliseconds(30)) {}
}