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
        private Camera _camera;
        Vector2 baseScreenSize = new Vector2(800, 480);
        public static int ScreenWidth = 1600;
        public static int ScreenHeight = 800;

        // Meta-level game state.
        private int levelIndex = -1;
        private Level level;

        private const int numberOfLevels = 3;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.IsFullScreen = false;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
        }

            

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = ScreenWidth;
            _graphics.PreferredBackBufferHeight = ScreenHeight;
            var vp = new Viewport();
            vp.X = vp.Y = 800;
            vp.Width = 800;
            vp.Height = 800;
            _graphics.GraphicsDevice.Viewport = vp;
            _graphics.ApplyChanges();
            _camera = new Camera();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            ContentManager content = new ContentManager(Services, "Content");

            // TODO: use this.Content to load your game content here
            LoadNextLevel();
            //player.LoadContent(content);
            //shadow.LoadContent(content);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            level.Update(gameTime);
            _camera.Follow(level.Player);
            base.Update(gameTime);
            //player.Update(gameTime);
            //shadow.Update(gameTime);
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

            _spriteBatch.Begin(transformMatrix: _camera.Transform);

            base.Draw(gameTime);
            level.Draw(gameTime, _spriteBatch);
            _spriteBatch.Draw(createCircleText(50), level.Player.Position, Color.White);
            //player.Draw(gameTime, _spriteBatch);
            //shadow.Draw(gameTime, _spriteBatch);

            _spriteBatch.End();
        }

        Texture2D createCircleText(int radius)
        {
            Texture2D texture = new Texture2D(GraphicsDevice, radius, radius);
            Color[] colorData = new Color[radius * radius];

            float diam = radius / 2f;
            float diamsq = diam * diam;

            for (int x = 0; x < radius; x++)
            {
                for (int y = 0; y < radius; y++)
                {
                    int index = x * radius + y;
                    Vector2 pos = new Vector2(x - diam, y - diam);
                    if (pos.LengthSquared() <= diamsq)
                    {
                        colorData[index] = Color.White;
                    }
                    else
                    {
                        colorData[index] = Color.Transparent;
                    }
                }
            }

            texture.SetData(colorData);
            return texture;
        }
    }
}
