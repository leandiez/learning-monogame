using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyFirstGameLibrary;
using MyFirstGameLibrary.Scenes;
using MyFirstGameLibrary.Graphics;
using MyFirstGameLibrary.Inputs;
using DungeonSlimeGame.Actors;
using DungeonSlimeGame.UI;
using Gum.DataTypes;
using Gum.Wireframe;
using Gum.Managers;
using MonoGameGum;
using Gum.Forms.Controls;
using MonoGameGum.GueDeriving;

namespace DungeonSlimeGame.Scenes;

public class GameScene : Scene {
    private Slime _slimePlayer = new Slime(Vector2.Zero, new Vector2(5,0));
    private Bat _batEnemy = new Bat(Vector2.Zero, AssignRandomBatVelocity());

    // Defines the tilemap to draw.
    private Tilemap _tilemap;
    // Defines the bounds of the room that the slime and bat are contained within.
    private Rectangle _roomBounds;
    // The sound effect to play when the bat bounces off the edge of the screen.
    private SoundEffect _bounceSoundEffect;
    // The sound effect to play when the slime eats a bat.
    private SoundEffect _collectSoundEffect;
    // The SpriteFont Description used to draw text
    private SpriteFont _font;
    // Tracks the players score.
    private int _score;
    // Defines the position to draw the score text at.
    private Vector2 _scoreTextPosition;
    // Defines the origin used when drawing the score text.
    private Vector2 _scoreTextOrigin;
    
    // UI elements
    private Panel _pausePanel;
    private AnimatedButton _resumeButton;
    private SoundEffect _uiSoundEffect;
    // Reference to the texture atlas that we can pass to UI elements when they
    // are created.
    private TextureAtlas _atlas;


    public override void Initialize() {
        base.Initialize();
        InitializeUI();
        Core.ExitOnEscape = false;
        Rectangle screenBounds = Core.GraphicsDevice.PresentationParameters.Bounds;
        _roomBounds = new Rectangle(
            (int)_tilemap.TileWidth,
            (int)_tilemap.TileHeight,
            screenBounds.Width - (int)_tilemap.TileWidth * 2,
            screenBounds.Height - (int)_tilemap.TileHeight * 2
        );
        // Initial slime position will be the center tile of the tile map.
        // Initial bat position will be in the top left corner of the room
        int centerRow = _tilemap.Rows / 2;
        int centerColumn = _tilemap.Columns / 2;
        _slimePlayer.Position = new Vector2(centerColumn * _tilemap.TileWidth, centerRow * _tilemap.TileHeight);
        _batEnemy.Position = new Vector2(_roomBounds.Left, _roomBounds.Top);
        // Set the position of the score text to align to the left edge of the
        // room bounds, and to vertically be at the center of the first tile.
        _scoreTextPosition = new Vector2(_roomBounds.Left, _tilemap.TileHeight * 0.5f);

        // Set the origin of the text so it is left-centered.
        float scoreTextYOrigin = _font.MeasureString("Score").Y * 0.5f;
        _scoreTextOrigin = new Vector2(0, scoreTextYOrigin);

    }
    public override void LoadContent() {
        // Carga de texturas a partir del XML con una descripcion del ATLAS.
        // Se pasa el ContentManager del juego para que cargue en memoria la textura principal
        // El resto seran TextureRegions los cuales se renderizan mediante un Sprite.
        _atlas = TextureAtlas.FromFile(Core.Content, "images/atlas-definition.xml");
        _slimePlayer.Animation = _atlas.CreateAnimatedSprite("slime-animation");
        _slimePlayer.Animation.Scale = new Vector2(5.0f, 5.0f);
        _batEnemy.Animation = _atlas.CreateAnimatedSprite("bat-animation");
        _batEnemy.Animation.Scale = new Vector2(5.0f, 5.0f);
        // Carga de las texturas del TILEMAP desde una descripcion en XML
        // TODO Ver como se puede implementar esto mismo usando Tiled u otra herramienta externa
        _tilemap = Tilemap.FromFile(Content,"images/tilemap-definition.xml");
        _tilemap.Scale = new Vector2(4.0f, 4.0f);
            
        // Carga archivos de sonido en su referencia
        _bounceSoundEffect = Content.Load<SoundEffect>("audio/bounce");
        _collectSoundEffect = Content.Load<SoundEffect>("audio/collect");
        _uiSoundEffect = Core.Content.Load<SoundEffect>("audio/ui");
        // Load the font
        _font = Content.Load<SpriteFont>("fonts/04B_30");
        base.LoadContent();
    }
        public override void Update(GameTime gameTime) {
            #if DEBUG
            Console.WriteLine(_slimePlayer.Collider.Location.ToString());
            #endif
            // UI and condition of pause
            GumService.Default.Update(gameTime);
            if (_pausePanel.IsVisible)
            {
                return;
            }
            // Verifica que botones se estan pulsando y cambia la posicion del SLIME
            CheckKeyboardInput();
            CheckGamepadInput();
            // Actualiza el frame a mostrar de cada animacion y su collider
            _slimePlayer.Update(gameTime);
            _batEnemy.Update(gameTime);
            
            // Colliders. Mover a otra funcion despues
            // SLIME COLLIDERS
            // Use distance based checks to determine if the slime is within the
            // bounds of the game screen, and if it is outside that screen edge,
            // move it back inside.
            if (_slimePlayer.Collider.Left < _roomBounds.Left)
            {
                _slimePlayer.Position = new Vector2(_roomBounds.Left , _slimePlayer.Position.Y);
            }
            else if (_slimePlayer.Collider.Right > _roomBounds.Right)
            {
                _slimePlayer.Position = new Vector2(_roomBounds.Right - _slimePlayer.Animation.Width , _slimePlayer.Position.Y);
            }

            if (_slimePlayer.Collider.Top < _roomBounds.Top)
            {
                _slimePlayer.Position = new Vector2(_slimePlayer.Position.X, _roomBounds.Top);
            }
            else if (_slimePlayer.Collider.Bottom > _roomBounds.Bottom)
            {
                _slimePlayer.Position = new Vector2(_slimePlayer.Position.X, _roomBounds.Bottom - _slimePlayer.Animation.Height);
            }
            // BAT COLLIDERS
            // Variable para la normal
            Vector2 normal = Vector2.Zero;
            Vector2 newPosition = _batEnemy.Position;
            // Use distance based checks to determine if the bat is within the
            // bounds of the game screen, and if it is outside that screen edge,
            // reflect it about the screen edge normal.
            if (_batEnemy.Collider.Left < _roomBounds.Left)
            {
                normal.X = Vector2.UnitX.X;
                newPosition.X = _roomBounds.Left;
            }
            else if (_batEnemy.Collider.Right > _roomBounds.Right)
            {
                normal.X = -Vector2.UnitX.X;
                newPosition.X = _roomBounds.Right - _batEnemy.Animation.Width;
            }

            if (_batEnemy.Collider.Top < _roomBounds.Top)
            {
                normal.Y = Vector2.UnitY.Y;
                newPosition.Y = _roomBounds.Top;
            }
            else if (_batEnemy.Collider.Bottom > _roomBounds.Bottom)
            {
                normal.Y = -Vector2.UnitY.Y;
                newPosition.Y = _roomBounds.Bottom - _batEnemy.Animation.Height;
            }
            _batEnemy.Position = newPosition;

            // If the normal is anything but Vector2.Zero, this means the bat had
            // moved outside the screen edge so we should reflect it about the
            // normal.
            if (normal != Vector2.Zero)
            {
                _batEnemy.Velocity = Vector2.Reflect(_batEnemy.Velocity, normal);
                Core.Audio.PlaySoundEffect(_bounceSoundEffect);
            }
            
            // INTERSECTION COLLIDER
            if (_slimePlayer.Collider.Intersects(_batEnemy.Collider))
            {
                // Divide the width  and height of the screen into equal columns and
                // rows based on the width and height of the bat.
                int totalColumns = Core.GraphicsDevice.PresentationParameters.BackBufferWidth / (int)_batEnemy.Animation.Width;
                int totalRows = Core.GraphicsDevice.PresentationParameters.BackBufferHeight / (int)_batEnemy.Animation.Height;

                // Choose a random row and column based on the total number of each
                int column = Random.Shared.Next(0, totalColumns);
                int row = Random.Shared.Next(0, totalRows);

                // Change the bat position by setting the x and y values equal to
                // the column and row multiplied by the width and height.
                _batEnemy.Position = new Vector2(column * _batEnemy.Animation.Width, row * _batEnemy.Animation.Height);

                // Assign a new random velocity to the bat
                _batEnemy.Velocity = AssignRandomBatVelocity();
                Core.Audio.PlaySoundEffect(_collectSoundEffect);
                // Increase the player's score.
                _score += 100;
            }
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime) {
            Core.GraphicsDevice.Clear(Color.CornflowerBlue);

            Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _tilemap.Draw(Core.SpriteBatch);
            _slimePlayer.Draw(Core.SpriteBatch);
            _batEnemy.Draw(Core.SpriteBatch);
            // Draw the score
            Core.SpriteBatch.DrawString(
                _font,                      // spriteFont
                $"Score: {_score}",    // text
                _scoreTextPosition,         // position
                Color.White,                // color
                0.0f,               // rotation
                _scoreTextOrigin,           // origin
                1.0f,                 // scale
                SpriteEffects.None,         // effects
                0.0f              // layerDepth
            );

            Core.SpriteBatch.End();
            // Draw the Gum UI
            GumService.Default.Draw();
            base.Draw(gameTime);
        }
        //---------------------- My Methods ----------------------
        // Mueve la posicion del SLIME segun la tecla presionada
        // TODO: Estos Checks deberian ir en una clase aparte para gestionar los Inputs que soporta el Juego. Candidato InputManager
        private void CheckKeyboardInput() {
            float speed = _slimePlayer.Velocity.Length();
            // If the escape key is pressed, pause the game
            if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Escape))
            {
                PauseGame();
            }
            if (Core.Input.Keyboard.IsKeyDown(Keys.Space)) speed *= 1.5f;
            if(Core.Input.Keyboard.IsKeyDown(Keys.W) || Core.Input.Keyboard.IsKeyDown(Keys.Up)) _slimePlayer.Position -= new Vector2(0.0f, speed);
            if(Core.Input.Keyboard.IsKeyDown(Keys.S) || Core.Input.Keyboard.IsKeyDown(Keys.Down)) _slimePlayer.Position += new Vector2(0.0f, speed);
            if(Core.Input.Keyboard.IsKeyDown(Keys.A) || Core.Input.Keyboard.IsKeyDown(Keys.Left)) _slimePlayer.Position -= new Vector2(speed, 0.0f);
            if(Core.Input.Keyboard.IsKeyDown(Keys.D) || Core.Input.Keyboard.IsKeyDown(Keys.Right)) _slimePlayer.Position += new Vector2(speed, 0.0f);
            
            // AUDIO CONTROLS
            if (Core.Input.Keyboard.WasKeyJustPressed(Keys.M))
            {
                Core.Audio.ToggleMute();
            }
            // Button +
            if (Core.Input.Keyboard.WasKeyJustPressed(Keys.OemPlus))
            {
                Core.Audio.SongVolume += 0.1f;
                Core.Audio.SoundEffectVolume += 0.1f;
            }
            // Button -
            if (Core.Input.Keyboard.WasKeyJustPressed(Keys.OemMinus))
            {
                Core.Audio.SongVolume -= 0.1f;
                Core.Audio.SoundEffectVolume -= 0.1f;
            }
        }
        // Mueve la posicion del SLIME segun el boton. Adicionalmente se prueba la vibracion al correr.
        private void CheckGamepadInput() {
            GamePadInfo gpState = Core.Input.GamePads[0]; // Necesito saber que Joystick voy a leer
            float speed = _slimePlayer.Velocity.Length();
            // If the start button is pressed, pause the game
            if (gpState.WasButtonJustPressed(Buttons.Start))
            {
                PauseGame();
            }
            if (gpState.IsButtonDown(Buttons.A)) {
                speed *= 1.5f;
                GamePad.SetVibration(PlayerIndex.One, 1.0f, 1.0f);
            }
            else  GamePad.SetVibration(PlayerIndex.One, 0, 0);
            if (gpState.LeftThumbStick != Vector2.Zero) { // Lee el analogico Izquierdo y si se esta moviendo actua
                _slimePlayer.Position += new Vector2(gpState.LeftThumbStick.X * speed, gpState.LeftThumbStick.Y * speed * -1.0f);
            }
            else {
                // Movimiento por el D-PAD
                if (gpState.IsButtonDown(Buttons.DPadUp)) _slimePlayer.Position -= new Vector2(0.0f, speed);
                if (gpState.IsButtonDown(Buttons.DPadDown)) _slimePlayer.Position += new Vector2(0.0f, speed);
                if (gpState.IsButtonDown(Buttons.DPadLeft)) _slimePlayer.Position -= new Vector2(speed, 0.0f);
                if (gpState.IsButtonDown(Buttons.DPadRight)) _slimePlayer.Position += new Vector2(speed, 0.0f);
            }
        }
        // Le da una nueva velocidad (con direccion) al murcielago. Es estatico para poder inicializar el murcielago.
        private static Vector2 AssignRandomBatVelocity() {
            // Generate a random angle.
            float angle = (float)(Random.Shared.NextDouble() * Math.PI * 2);

            // Convert angle to a direction vector.
            float x = (float)Math.Cos(angle);
            float y = (float)Math.Sin(angle);
            Vector2 direction = new Vector2(x, y);

            // Multiply the direction vector by the movement speed.
            return (direction * Random.Shared.Next(5, 10));
        }
        private void PauseGame()
        {
            // Make the pause panel UI element visible.
            _pausePanel.IsVisible = true;
            // Set the resume button to have focus
            _resumeButton.IsFocused = true;
        }
        
        private void CreatePausePanel()
        {
            _pausePanel = new Panel();
            _pausePanel.Anchor(Anchor.Center);
            _pausePanel.Visual.WidthUnits = DimensionUnitType.Absolute;
            _pausePanel.Visual.HeightUnits = DimensionUnitType.Absolute;
            _pausePanel.Visual.Height = 70;
            _pausePanel.Visual.Width = 264;
            _pausePanel.IsVisible = false;
            _pausePanel.AddToRoot();

            TextureRegion backgroundRegion = _atlas.GetRegion("panel-background");

            NineSliceRuntime background = new NineSliceRuntime();
            background.Dock(Dock.Fill);
            background.Texture = backgroundRegion.Texture;
            background.TextureAddress = TextureAddress.Custom;
            background.TextureHeight = backgroundRegion.Height;
            background.TextureLeft = backgroundRegion.Region.Left;
            background.TextureTop = backgroundRegion.Region.Top;
            background.TextureWidth = backgroundRegion.Width;
            _pausePanel.AddChild(background);

            TextRuntime textInstance = new TextRuntime();
            textInstance.Text = "PAUSED";
            textInstance.CustomFontFile = @"fonts/04b_30.fnt";
            textInstance.UseCustomFont = true;
            textInstance.FontScale = 0.5f;
            textInstance.X = 10f;
            textInstance.Y = 10f;
            _pausePanel.AddChild(textInstance);

            _resumeButton = new AnimatedButton(_atlas);
            _resumeButton.Text = "RESUME";
            _resumeButton.Anchor(Anchor.BottomLeft);
            _resumeButton.Visual.X = 9f;
            _resumeButton.Visual.Y = -9f;
            _resumeButton.Visual.Width = 80;
            _resumeButton.Click += HandleResumeButtonClicked;
            _pausePanel.AddChild(_resumeButton);

            AnimatedButton quitButton = new AnimatedButton(_atlas);
            quitButton.Text = "QUIT";
            quitButton.Anchor(Anchor.BottomRight);
            quitButton.Visual.X = -9f;
            quitButton.Visual.Y = -9f;
            quitButton.Width = 80;
            quitButton.Click += HandleQuitButtonClicked;

            _pausePanel.AddChild(quitButton);
        }
        private void HandleResumeButtonClicked(object sender, EventArgs e)
        {
            // A UI interaction occurred, play the sound effect
            Core.Audio.PlaySoundEffect(_uiSoundEffect);
            // Make the pause panel invisible to resume the game.
            _pausePanel.IsVisible = false;
        }
        private void HandleQuitButtonClicked(object sender, EventArgs e)
        {
            // A UI interaction occurred, play the sound effect
            Core.Audio.PlaySoundEffect(_uiSoundEffect);

            // Go back to the title scene.
            Core.ChangeScene(new TitleScene());
        }
        private void InitializeUI()
        {
            GumService.Default.Root.Children.Clear();
            CreatePausePanel();
        }
}