using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyFirstGameLibrary.Graphics;
using MyFirstGameLibrary.Primitives;
namespace DungeonSlimeGame.Actors;

public class Bat(Vector2 position, Vector2 velocity) {
    public Vector2  Position { get; set; } = position;
    public Vector2  Velocity { get; set; } = velocity;
    public AnimatedSprite Animation { get; set; }
    private AnimatedSprite _animation;
    public Circle Collider {get; private set;}
    
    public void Update(GameTime gameTime) {
        Animation.Update(gameTime);
        Position += Velocity; // El murcielago siempre se mueve y lo controlo usando la velocidad.
        Collider =  new Circle((int)(Position.X + Animation.Width * 0.5f),
            (int)(Position.Y + Animation.Height * 0.5f),
            (int)(Animation.Width * 0.5f));
    }
    
    public void Draw(SpriteBatch sprBtch) {
        Animation.Draw(sprBtch, Position);
    }
}