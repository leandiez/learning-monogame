using Microsoft.Xna.Framework.Media;
using MyFirstGameLibrary;
using DungeonSlimeGame.Scenes;

namespace DungeonSlimeGame {
    public class Game2 : Core {
        // The background theme song.
        private Song _themeSong;

        public Game2() : base("Dungeon Slime", 1280, 720, false) {
        }

        protected override void Initialize() {
            base.Initialize();

            // Start playing the background music.
            Audio.PlaySong(_themeSong);

            // Start the game with the title scene.
            ChangeScene(new TitleScene());
        }

        protected override void LoadContent() {
            // Load the background theme music.
            _themeSong = Content.Load<Song>("audio/theme");
        }
    }
}