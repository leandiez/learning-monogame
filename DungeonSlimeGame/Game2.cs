using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyFirstGameLibrary;
using MyFirstGameLibrary.Graphics;
using MyFirstGameLibrary.Inputs;

namespace DungeonSlimeGame {
    public class Game2 : Core {
        // Graficos
        private Texture2D _monogameLogo;
        private AnimatedSprite _slime;
        private AnimatedSprite _bat;
        private Vector2 _screenCenterPosition;
        private Rectangle _iconRect, _wordmarkRect;
        
        // Inputs
        private Vector2 _slimePosition;
        private const float MOVEMENT_SPEED = 5.0f;
        public Game2() : base("Dungeon Slime", 1280, 720, false) { }

        protected override void Initialize() {
            base.Initialize();
            
            _screenCenterPosition = new Vector2(
                GraphicsDevice.Viewport.Width * 0.5f,
                GraphicsDevice.Viewport.Height * 0.5f
            );
            // Rectangles para dibujar una parte de la textura con el LOGO
            _iconRect = new Rectangle(0, 0, 128, 128);
            _wordmarkRect = new Rectangle(150, 34, 458, 58);
            /* Chequeo de controles
            for (int i = 0; i < 16; i++) {
                Console.WriteLine(GamePad.GetState(i).IsConnected);
            }
        */
        }

        protected override void LoadContent() {
            _monogameLogo = Content.Load<Texture2D>("images/logo");
            
            // Carga de texturas a partir del XML con una descripcion del ATLAS.
            // Se pasa el ContentManager del juego para que cargue en memoria la textura principal
            // El resto seran TextureRegions los cuales se renderizan mediante un Sprite.
            TextureAtlas atlas = TextureAtlas.FromFile(Content, "images/atlas-definition.xml");
            _slime = atlas.CreateAnimatedSprite("slime-animation");
            _slime.Scale = new Vector2(5.0f, 5.0f);
            _bat = atlas.CreateAnimatedSprite("bat-animation");
            _bat.Scale = new Vector2(5.0f, 5.0f);
            
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime) {
            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            // Actualiza el frame a mostrar de cada animacion
            _slime.Update(gameTime);
            _bat.Update(gameTime);

            // Verifica que botones se estan pulsando y cambia la posicion del SLIME
            CheckKeyboardInput();
            CheckGamepadInput();
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _slime.Draw(SpriteBatch, Vector2.Zero);
            _bat.Draw(SpriteBatch, _slimePosition);
            SpriteBatch.Draw(
                _monogameLogo,                              // Texture to draw
                _screenCenterPosition,                      // Position to draw at
                _iconRect,                                  // Source rectangle (null means the whole texture)
                Color.White,                                // Color to tint the texture
                0f,                                         // Rotation angle in radians
                new Vector2(
                    _iconRect.Width * 0.5f,
                    _iconRect.Height * 0.5f
                ),                                          // Origin point for rotation and scaling
                1f,                                         // Scale factor
                SpriteEffects.None,                         // Flip effects
                0f                                          // Layer depth
            );

            SpriteBatch.Draw(
                _monogameLogo,                              // Texture to draw
                _screenCenterPosition + new Vector2(0, 200),                      // Position to draw at
                _wordmarkRect,                                  // Source rectangle (null means the whole texture)
                Color.White,                                // Color to tint the texture
                0f,                                         // Rotation angle in radians
                new Vector2(
                    _wordmarkRect.Width * 0.5f,
                    _wordmarkRect.Height * 0.5f
                ),                                          // Origin point for rotation and scaling
                1f,                                         // Scale factor
                SpriteEffects.None,                         // Flip effects
                0f                                          // Layer depth
            );

            SpriteBatch.End();

            base.Draw(gameTime);
        }
        
        //---------------------- My Methods ----------------------
        // Mueve la posicion del SLIME segun la tecla presionada
        private void CheckKeyboardInput() {
            float speed = MOVEMENT_SPEED;
            if (Input.Keyboard.IsKeyDown(Keys.Space)) speed *= 1.5f;
            if(Input.Keyboard.IsKeyDown(Keys.W) || Input.Keyboard.IsKeyDown(Keys.Up)) _slimePosition.Y -= speed;
            if(Input.Keyboard.IsKeyDown(Keys.S) || Input.Keyboard.IsKeyDown(Keys.Down)) _slimePosition.Y += speed;
            if(Input.Keyboard.IsKeyDown(Keys.A) || Input.Keyboard.IsKeyDown(Keys.Left)) _slimePosition.X -= speed;
            if(Input.Keyboard.IsKeyDown(Keys.D) || Input.Keyboard.IsKeyDown(Keys.Right)) _slimePosition.X += speed;
        }
        // Mueve la posicion del SLIME segun el boton. Adicionalmente se prueba la vibracion al correr.
        private void CheckGamepadInput() {
            GamePadInfo gpState = Input.GamePads[0]; // Necesito saber que Joystick voy a leer
            float speed = MOVEMENT_SPEED;
            if (gpState.IsButtonDown(Buttons.A)) {
                speed *= 1.5f;
                GamePad.SetVibration(PlayerIndex.One, 1.0f, 1.0f);
            }
            else  GamePad.SetVibration(PlayerIndex.One, 0, 0);
            if (gpState.LeftThumbStick != Vector2.Zero) { // Lee el analogico Izquierdo y si se esta moviendo actua
                _slimePosition.X += gpState.LeftThumbStick.X * speed;
                _slimePosition.Y -= gpState.LeftThumbStick.Y * speed;
            }
            else {
                // Movimiento por el D-PAD
                if (gpState.IsButtonDown(Buttons.DPadUp)) _slimePosition.Y -= speed;
                if (gpState.IsButtonDown(Buttons.DPadDown)) _slimePosition.Y += speed;
                if (gpState.IsButtonDown(Buttons.DPadLeft)) _slimePosition.X -= speed;
                if (gpState.IsButtonDown(Buttons.DPadRight)) _slimePosition.X += speed;
            }
        }
    }
}