using System;
using Microsoft.Xna.Framework;

namespace MyFirstGameLibrary.Graphics;
// Representa un Sprite con multiples regiones las cuales se reproducen secuencialmente.
public class AnimatedSprite : Sprite
{
    private int _currentFrame;
    private TimeSpan _elapsed;
    private Animation _animation;
    //Getter y Setter de animacion
    public Animation Animation
    {
        get { return _animation; }
        // Al cambiar o setear la animacion el sprite toma el primer frame
        set
        {
            _animation = value;
            Region = _animation.Frames[0];
        }
    }
    
    public AnimatedSprite(){} //No necesita llamar a Base, ya que el Sprite define sus propiedades default sin pasar por constructor

    public AnimatedSprite(Animation animation)
    {
        Animation = animation;
    }
    
    /// <summary>
    /// Updates this animated sprite.
    /// </summary>
    /// <param name="gameTime">A snapshot of the game timing values provided by the framework.</param>
    public void Update(GameTime gameTime)
    {
        _elapsed += gameTime.ElapsedGameTime;

        if (_elapsed >= _animation.Delay)
        {
            _elapsed -= _animation.Delay;
            _currentFrame++;

            if (_currentFrame >= _animation.Frames.Count)
            {
                _currentFrame = 0;
            }
            Region = _animation.Frames[_currentFrame];
        }
    }

}