using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DungeonSlimeGame;
// JUEGO 1 - Dibuja una textura y la mueve con las flechas. ESC para salir
public class Game1 : Game {
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _redBallTexture;
    Vector2 _ballPosition;
    float _ballSpeed;
    int _deadZone;

    // Constructor, hereda de Game
    public Game1() {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }
    // Logica a correr antes de iniciar el loop del juego
    protected override void Initialize() {
        _ballPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2,
                           _graphics.PreferredBackBufferHeight / 2);
        _ballSpeed = 200f;
        _deadZone = 4096;
        base.Initialize();
    }
    // Carga el contenido del juego en memoria
    protected override void LoadContent() {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _redBallTexture = Content.Load<Texture2D>("images/logo");
    }
    // Loop de logica del juego, corre cada frame y segun configuracion del constructor
    protected override void Update(GameTime gameTime) {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();
        // Actualizo posicion de la bola segun direcciones del teclado.
        float updatedBallSpeed = _ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        var kstate = Keyboard.GetState();
        if (kstate.IsKeyDown(Keys.Up)) _ballPosition.Y -= updatedBallSpeed;
        if (kstate.IsKeyDown(Keys.Down)) _ballPosition.Y += updatedBallSpeed;
        if (kstate.IsKeyDown(Keys.Left)) _ballPosition.X -= updatedBallSpeed;
        if (kstate.IsKeyDown(Keys.Right)) _ballPosition.X += updatedBallSpeed;

        // Idem anterior pero con Joystick
        if (Joystick.LastConnectedIndex == 0) {
            JoystickState jstate = Joystick.GetState((int)PlayerIndex.One);
            if (jstate.Axes[1] < -_deadZone) _ballPosition.Y -= updatedBallSpeed;
            if (jstate.Axes[1] > _deadZone) _ballPosition.Y += updatedBallSpeed;
            if (jstate.Axes[0] < -_deadZone) _ballPosition.X -= updatedBallSpeed;
            if (jstate.Axes[0] > _deadZone) _ballPosition.X += updatedBallSpeed;
        }
        base.Update(gameTime);
    }
    // Actualiza lo que se ve en pantalla
    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.DarkBlue);
        _spriteBatch.Begin();
        _spriteBatch.Draw(_redBallTexture, _ballPosition, null, Color.White, 0f, new Vector2(_redBallTexture.Width / 2, _redBallTexture.Height / 2), Vector2.One, SpriteEffects.None, 0f);
        _spriteBatch.Draw(_redBallTexture, _ballPosition, Color.White);
        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
