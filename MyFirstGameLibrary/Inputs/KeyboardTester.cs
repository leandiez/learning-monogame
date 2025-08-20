using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using Microsoft.Xna.Framework.Graphics;
// Clase util para probar cada tecla de un teclado a nivel Hardware y comprobar que sea la que se mapea en Keys
// TODO Ver de hacer una escena con este codigo o ver como reutilizarlo para que se pueda aprovechar en un modo DEBUG
public class KeyboardTester : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteFont _font;
    private KeyboardState _previousKeyboardState;
    private string _displayText = "Press keys...";

    public KeyboardTester()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _font = Content.Load<SpriteFont>("MyFont"); // Ensure you have a font asset named "Font"
        _previousKeyboardState = Keyboard.GetState();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        KeyboardState currentKeyboardState = Keyboard.GetState();
        Keys[] pressedKeys = currentKeyboardState.GetPressedKeys();
        _displayText = "";

        if (pressedKeys.Length > 0)
        {
            foreach (Keys key in pressedKeys)
            {
                _displayText += key.ToString() + " ";
            }
        }
        else
        {
            _displayText = "Press keys...";
        }

        _previousKeyboardState = currentKeyboardState;
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        SpriteBatch mySprBtch = new SpriteBatch(_graphics.GraphicsDevice);
        mySprBtch.Begin();
        mySprBtch.DrawString(_font, _displayText, new Vector2(10, 10), Color.White);
        mySprBtch.End();

        base.Draw(gameTime);
    }
}