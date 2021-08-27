using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Rapid_Prototyping_T7.Game;
using Rapid_Prototyping_T7.Game.Objects;
using System.IO;
using Microsoft.Xna.Framework.Content;


namespace Rapid_Prototyping_T7
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Vector2 baseScreenSize = new Vector2(800, 480);

        // Meta-level game state.
        private int levelIndex = -1;
        private Level level;

        private const int numberOfLevels = 3;

        private Player player;
        private Shadow shadow;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.IsFullScreen = false;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            player = new Player();
            shadow = new Shadow(player);
        }

            

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = 1600;
            _graphics.PreferredBackBufferHeight = 800;
            var vp = new Viewport();
            vp.X = vp.Y = 800;
            vp.Width = 800;
            vp.Height = 800;
            _graphics.GraphicsDevice.Viewport = vp;
            _graphics.ApplyChanges();
            base.Initialize();
            player.Initialize();
            shadow.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            ContentManager content = new ContentManager(Services, "Content");

            // TODO: use this.Content to load your game content here
            LoadNextLevel();
            player.LoadContent(content);
            shadow.LoadContent(content);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            level.Update(gameTime);
            base.Update(gameTime);
            player.Update(gameTime);
            shadow.Update(gameTime);
        }

        private void LoadNextLevel()
        {
            // move to the next level
            levelIndex = (levelIndex + 1) % numberOfLevels;

            // Unloads the content for the current level before loading the next one.
            if (level != null)
                level.Dispose();

            // Load the level.
            string levelPath = string.Format("Content/Levels/{0}.txt", levelIndex);
            using (Stream fileStream = TitleContainer.OpenStream(levelPath))
                level = new Level(Services, fileStream, levelIndex);
        }

        protected override void Draw(GameTime gameTime)
        {
            _graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null);

            base.Draw(gameTime); 
            level.Draw(gameTime, _spriteBatch);
            player.Draw(gameTime, _spriteBatch);
            shadow.Draw(gameTime, _spriteBatch);

            _spriteBatch.End();
        }
    }
}
