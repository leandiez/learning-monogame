using System;
using System.Diagnostics;
using DungeonSlimeGame.Actors;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyFirstGameLibrary;
using MyFirstGameLibrary.Primitives;
using MyFirstGameLibrary.Graphics;
using MyFirstGameLibrary.Inputs;

namespace DungeonSlimeGame {
    public class Game2 : Core {
        // Actors - OffTutorial
        private Slime _slimePlayer = new Slime(Vector2.Zero, new Vector2(5,0));
        private Bat _batEnemy = new Bat(Vector2.Zero, AssignRandomBatVelocity());
        
        // Colliders
        private Rectangle _roomBounds;
        
        // Defines the tilemap to draw.
        private Tilemap _tilemap;
        public Game2() : base("Dungeon Slime", 1280, 720, false) { }

        protected override void Initialize() {
            base.Initialize();
            Rectangle screenBounds = GraphicsDevice.PresentationParameters.Bounds;
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
        }
        
        protected override void LoadContent() {
            // Carga de texturas a partir del XML con una descripcion del ATLAS.
            // Se pasa el ContentManager del juego para que cargue en memoria la textura principal
            // El resto seran TextureRegions los cuales se renderizan mediante un Sprite.
            TextureAtlas atlas = TextureAtlas.FromFile(Content, "images/atlas-definition.xml");
            _slimePlayer.Animation = atlas.CreateAnimatedSprite("slime-animation");
            _slimePlayer.Animation.Scale = new Vector2(5.0f, 5.0f);
            _batEnemy.Animation = atlas.CreateAnimatedSprite("bat-animation");
            _batEnemy.Animation.Scale = new Vector2(5.0f, 5.0f);
            
            _tilemap = Tilemap.FromFile(Content,"images/tilemap-definition.xml");
            _tilemap.Scale = new Vector2(4.0f, 4.0f);
            
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime) {
            Console.WriteLine(_slimePlayer.Collider.Location.ToString());
            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
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
            }
            
            // INTERSECTION COLLIDER
            if (_slimePlayer.Collider.Intersects(_batEnemy.Collider))
            {
                // Divide the width  and height of the screen into equal columns and
                // rows based on the width and height of the bat.
                int totalColumns = GraphicsDevice.PresentationParameters.BackBufferWidth / (int)_batEnemy.Animation.Width;
                int totalRows = GraphicsDevice.PresentationParameters.BackBufferHeight / (int)_batEnemy.Animation.Height;

                // Choose a random row and column based on the total number of each
                int column = Random.Shared.Next(0, totalColumns);
                int row = Random.Shared.Next(0, totalRows);

                // Change the bat position by setting the x and y values equal to
                // the column and row multiplied by the width and height.
                _batEnemy.Position = new Vector2(column * _batEnemy.Animation.Width, row * _batEnemy.Animation.Height);

                // Assign a new random velocity to the bat
                AssignRandomBatVelocity();
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _tilemap.Draw(SpriteBatch);
            _slimePlayer.Draw(SpriteBatch);
            _batEnemy.Draw(SpriteBatch);

            SpriteBatch.End();

            base.Draw(gameTime);
        }
        
        //---------------------- My Methods ----------------------
        // Mueve la posicion del SLIME segun la tecla presionada
        // TODO: Estos Checks deberian ir en una clase aparte para gestionar los Inputs que soporta el Juego. Candidato InputManager
        private void CheckKeyboardInput() {
            float speed = _slimePlayer.Velocity.Length();
            if (Input.Keyboard.IsKeyDown(Keys.Space)) speed *= 1.5f;
            if(Input.Keyboard.IsKeyDown(Keys.W) || Input.Keyboard.IsKeyDown(Keys.Up)) _slimePlayer.Position -= new Vector2(0.0f, speed);
            if(Input.Keyboard.IsKeyDown(Keys.S) || Input.Keyboard.IsKeyDown(Keys.Down)) _slimePlayer.Position += new Vector2(0.0f, speed);
            if(Input.Keyboard.IsKeyDown(Keys.A) || Input.Keyboard.IsKeyDown(Keys.Left)) _slimePlayer.Position -= new Vector2(speed, 0.0f);
            if(Input.Keyboard.IsKeyDown(Keys.D) || Input.Keyboard.IsKeyDown(Keys.Right)) _slimePlayer.Position += new Vector2(speed, 0.0f);
        }
        // Mueve la posicion del SLIME segun el boton. Adicionalmente se prueba la vibracion al correr.
        private void CheckGamepadInput() {
            GamePadInfo gpState = Input.GamePads[0]; // Necesito saber que Joystick voy a leer
            float speed = _slimePlayer.Velocity.Length();
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

    }
}