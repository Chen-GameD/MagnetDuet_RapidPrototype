using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Rapid_Prototyping_T7.Game;
using Rapid_Prototyping_T7.Game.Objects;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Rapid_Prototyping_T7.Constants;

namespace Rapid_Prototyping_T7
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Camera _camera;

        // Meta-level game state.
        private int levelIndex = -1;
        private Level level;

        private SpriteFont hudFont;
        private Texture2D batteryHud;

        //Game State
        private Texture2D winOverlay;
        private Texture2D diedOverlay;

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
            _graphics.PreferredBackBufferWidth = Constants.Constants.ScreenWidth;
            _graphics.PreferredBackBufferHeight = Constants.Constants.ScreenHeight;
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

            // Load Font
            hudFont = Content.Load<SpriteFont>("Fonts/Hud");

            //Load Overlay
            winOverlay = Content.Load<Texture2D>("Overlays/you_win");
            diedOverlay = Content.Load<Texture2D>("Overlays/you_died");

            // TODO: use this.Content to load your game content here
            LoadNextLevel();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            HandleInput(gameTime);

            // TODO: Add your update logic here
            level.Update(gameTime);
            _camera.Follow(level.Player, level.Shadow);
            base.Update(gameTime);
            //player.Update(gameTime);
            //shadow.Update(gameTime);
        }

        private void HandleInput(GameTime gameTime)
        {
            // Exit the game when back is pressed.
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            bool resetPressed =
                Keyboard.GetState().IsKeyDown(Keys.R);
            resetPressed = resetPressed || (Keyboard.GetState().IsKeyDown(Keys.Space) && !level.Player.IsAlive);

            // Perform the appropriate action to advance the game and
            // to get the player back to playing.
            if (resetPressed)
            {
                level.StartNewLife();

                //if (!level.Player.IsAlive)
                //{
                    //level.StartNewLife();
                //}
            }
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
            //_spriteBatch.Begin();

            base.Draw(gameTime);
            
            level.Draw(gameTime, _spriteBatch, new Vector2(_camera.Transform.M41, _camera.Transform.M42));
            DrawHud(_camera.Transform, level.Player.battery_duration);
            //_spriteBatch.Draw(createCircleText(50), level.Player.Position, Color.White);
            //player.Draw(gameTime, _spriteBatch);
            //shadow.Draw(gameTime, _spriteBatch);

            //DrawCollisionLine(_graphics, level.Player.BoundingRectangle, _spriteBatch, new Vector2(level.Player.Sprite.Width / 2, level.Player.Sprite.Height));

            _spriteBatch.End();
        }

        private void DrawHud(Matrix transform, float batter_state)
        {
            //Rectangle titleSafeArea = GraphicsDevice.Viewport.TitleSafeArea;
            Vector2 hudLocation = new Vector2(-transform.M41, -transform.M42);
            //Vector2 center = new Vector2(titleSafeArea.X + titleSafeArea.Width / 2.0f,
            //                             titleSafeArea.Y + titleSafeArea.Height / 2.0f);

            Vector2 center = new Vector2(Constants.Constants.ScreenWidth / 2, Constants.Constants.ScreenHeight / 2);

            // Draw score
            string scoreString = "SCORE: " + level.Score.ToString();
            DrawShadowedString(hudFont, scoreString, hudLocation, Color.Yellow);

            // Draw Battery
            float scoreHeight = hudFont.MeasureString(scoreString).Y;
            string batteryString = "BatteryState:" + batter_state;
            DrawShadowedString(hudFont, batteryString, hudLocation + new Vector2(0, scoreHeight * 1.2f), Color.Yellow);

            //Draw Reset Tips
            DrawShadowedString(hudFont, "Press 'R' to reset game", hudLocation + new Vector2(0, scoreHeight * 2.4f), Color.Yellow);

            // Determine the status overlay message to show.
            Texture2D status = null;

            
            if (!level.Player.IsAlive)
            {
                status = diedOverlay;
            }
            if (level.Player.IsAlive && level.ReachedExit)
            {
                status = winOverlay;
            }

            if (status != null)
            {
                // Draw status message.
                Vector2 statusSize = new Vector2(status.Width, status.Height);
                Vector2 statusPosition = new Vector2(center.X - (statusSize.X / 2) - transform.M41, center.Y - (statusSize.Y / 2) - transform.M42);
                _spriteBatch.Draw(status, statusPosition, Color.White);
            }
        }

        private void DrawShadowedString(SpriteFont font, string value, Vector2 position, Color color)
        {
            _spriteBatch.DrawString(font, value, position + new Vector2(1.0f, 1.0f), Color.Black);
            _spriteBatch.DrawString(font, value, position, color);
        }

        public void DrawCollisionLine(GraphicsDeviceManager mag, Rectangle rec, SpriteBatch spr, Vector2 origin)
        {
            DrawCollision col = new DrawCollision(mag, rec);
            col.Update(rec, origin);
            col.Draw(spr);
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
