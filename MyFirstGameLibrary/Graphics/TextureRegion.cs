using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyFirstGameLibrary.Graphics;
// Representa una region dentro de una Textura cargada por el gestor de contenido de MonoGame
public class TextureRegion
{
    public Texture2D Texture { get; }
    public Rectangle Region { get; }
    public int Width => Region.Width;
    public int Height => Region.Height;

    public TextureRegion(Texture2D texture, Rectangle region)
    {
        Texture = texture;
        Region = region;
    }

    // Sobrecarga para crear un TextureRegion con coordenadas y dimensiones específicas.
    public TextureRegion(Texture2D texture, int x, int y, int width, int height) : this(texture, new Rectangle(x, y, width, height)) { }

    public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color)
    {
        Draw(spriteBatch, position, 0.0f, Vector2.Zero, Vector2.One, color, SpriteEffects.None, 0.0f);
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position, float rotation, Vector2 origin, float scale, Color color, SpriteEffects flip, float layerDepth)
    {
        Draw(spriteBatch, position, rotation, origin, new Vector2(scale, scale), color, flip, layerDepth);
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position, float rotation, Vector2 origin, Vector2 scale, Color color, SpriteEffects effects, float layerDepth)
    {
        spriteBatch.Draw(Texture, position, Region, color, rotation, origin, scale, effects, layerDepth);
    }
}