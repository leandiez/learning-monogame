namespace MyFirstGameLibrary.Graphics;
// Representa a una textura y expone sus parametros para que puedan ser modificados y gestionados en una instancia propia.
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
public class Sprite
{
    public TextureRegion Region {get; set;}
    //Propiedades de la Textura aplicadas al Sprite
    public Color Color {get; set;} = Color.White;
    public float Rotation { get; set; } = 0.0f;
    public Vector2 Scale {get; set;} = Vector2.One;
    public Vector2 Origin {get; set;} = Vector2.Zero;
    public float LayerDepth { get; set; } = 0;
    public SpriteEffects Effects {get; set;} = SpriteEffects.None;
    // Getters del tamaño Sprite
    public float Width => Region.Width * Scale.X;
    public float Height => Region.Height * Scale.Y;
    
    public Sprite(){}

    public Sprite(TextureRegion region)
    {
        Region = region;
    }
    //Pone el Origen del Sprite en el centro del mismo
    public void CenterOrigin()
    {
        Origin = new Vector2(Region.Width, Region.Height) * 0.5f;
    }

    public void Draw(SpriteBatch sprBtch, Vector2 pos)
    {
        Region.Draw(sprBtch, pos, Rotation, Origin, Scale, Color, Effects, LayerDepth);
    }
}