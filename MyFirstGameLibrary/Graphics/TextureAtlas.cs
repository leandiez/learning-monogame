using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MyFirstGameLibrary.Graphics;
// Representa un conjunto de TextureRegions que forman parte de una unica Textura de MonoGame.
// Permite crear multiples regions de una sola vez usando un XML

public class TextureAtlas
{
    private Dictionary<string, TextureRegion> _regions;
    private Dictionary<string, Animation> _animations;
    public Texture2D Texture { get; set; }

    public TextureAtlas()
    {
        _regions = new Dictionary<string, TextureRegion>();
        _animations = new Dictionary<string, Animation>();
    }

    public TextureAtlas(Texture2D texture) : this()
    {
        Texture = texture;
    }

    public void AddRegion(string name, int x, int y, int width, int height)
    {
        TextureRegion reg = new TextureRegion(Texture, x, y, width, height);
        _regions.Add(name, reg);
    }

    public TextureRegion GetRegion(string name)
    {
        return _regions[name];
    }
    public bool RemoveRegion(string name)
    {
        return _regions.Remove(name);
    }

    public void Clear()
    {
        _regions.Clear();
    }

    public Sprite CreateSprite(string regionName)
    {
        TextureRegion region = GetRegion(regionName);
        return new Sprite(region);
    }

    public AnimatedSprite CreateAnimatedSprite(string animationName)
    {
        Animation anim = GetAnimation(animationName);
        return new AnimatedSprite(anim);
    }

    public void AddAnimation(string animationName, Animation animation)
    {
        _animations.Add(animationName, animation);
    }
    public Animation GetAnimation(string animationName)
    {
        return _animations[animationName];
    }
    public bool RemoveAnimation(string animationName)
    {
        return _animations.Remove(animationName);
    }

    public static TextureAtlas FromFile(ContentManager content, string fileName)
    {
        TextureAtlas atlas = new TextureAtlas();

        string filePath = Path.Combine(content.RootDirectory, fileName);

        using (Stream stream = TitleContainer.OpenStream(filePath))
        {
            using (XmlReader reader = XmlReader.Create(stream))
            {
                XDocument doc = XDocument.Load(reader);
                XElement root = doc.Root;

                string texturePath = root.Element("Texture").Value;
                atlas.Texture = content.Load<Texture2D>(texturePath);

                var regions = root.Element("Regions")?.Elements("Region");
                
                // Crea un Atlas usando data desde un XML. Ejemplo del XML a leer:
                // ReSharper disable once InvalidXmlDocComment
                /**
                <?xml version = "1.0" encoding="utf-8"?>
                <TextureAtlas>
                    <Texture>images/atlas</Texture>
                    <Regions>
                        <Region name = "slime" x="0" y="0" width="20" height="20" />
                        <Region name = "bat" x="20" y="0" width="20" height="20" />
                    </Regions>
                </TextureAtlas>
                **/


                if (regions != null)
                {
                    foreach (var region in regions)
                    {
                        string name = region.Attribute("name")?.Value;
                        int x = int.Parse(region.Attribute("x")?.Value ?? "0");
                        int y = int.Parse(region.Attribute("y")?.Value ?? "0");
                        int width = int.Parse(region.Attribute("width")?.Value ?? "0");
                        int height = int.Parse(region.Attribute("height")?.Value ?? "0");

                        if (!string.IsNullOrEmpty(name))
                        {
                            atlas.AddRegion(name, x, y, width, height);
                        }
                    }
                }
                // The <Animations> element contains individual <Animation> elements, each one describing
                // a different animation within the atlas.
                //
                // Example:
                // <Animations>
                //      <Animation name="animation" delay="100">
                //          <Frame region="spriteOne" />
                //          <Frame region="spriteTwo" />
                //      </Animation>
                // </Animations>
                //
                // So we retrieve all of the <Animation> elements then loop through each one
                // and generate a new Animation instance from it and add it to this atlas.
                var animationElements = root.Element("Animations").Elements("Animation");

                if (animationElements != null)
                {
                    foreach (var animationElement in animationElements)
                    {
                        string name = animationElement.Attribute("name")?.Value;
                        float delayInMilliseconds = float.Parse(animationElement.Attribute("delay")?.Value ?? "0");
                        TimeSpan delay = TimeSpan.FromMilliseconds(delayInMilliseconds);

                        List<TextureRegion> frames = new List<TextureRegion>();

                        var frameElements = animationElement.Elements("Frame");

                        if (frameElements != null)
                        {
                            foreach (var frameElement in frameElements)
                            {
                                string regionName = frameElement.Attribute("region").Value;
                                TextureRegion region = atlas.GetRegion(regionName);
                                frames.Add(region);
                            }
                        }

                        Animation animation = new Animation(frames, delay);
                        atlas.AddAnimation(name, animation);
                    }
                }
                return atlas;
            }
        }
    }


}
