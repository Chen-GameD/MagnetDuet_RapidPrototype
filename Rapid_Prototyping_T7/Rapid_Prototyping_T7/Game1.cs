using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Rapid_Prototyping_T7.Game;
using Rapid_Prototyping_T7.Game.Objects;
using Rapid_Prototyping_T7.Game.GameLogic;
using Microsoft.Xna.Framework.Content;


namespace Rapid_Prototyping_T7
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        Vector2 baseScreenSize = new Vector2(800, 480);

        BackgroundContainer background;
        //LevelMaker levelMaker;
        //Level level;

        private Player player;
        private Shadow shadow;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.IsFullScreen = false;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            background = new BackgroundContainer();
            //levelMaker = new LevelMaker();
            //level = levelMaker.LoadNextLevel();

            player = new Player();
            shadow = new Shadow(player);
    }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            background.Initialize();
            //level.Initialize();
            player.Initialize();
            shadow.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            ContentManager content = new ContentManager(Services, "Content");

            background.LoadContent(content);
            //level.LoadContent(content);
            player.LoadContent(content);
            shadow.LoadContent(content);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            
            base.Update(gameTime);
            background.Update(gameTime);
            //level.Update(gameTime);
            player.Update(gameTime);
            shadow.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null);

            base.Draw(gameTime);
            background.Draw(gameTime, _spriteBatch);
            //level.Draw(gameTime, _spriteBatch);
            player.Draw(gameTime, _spriteBatch);
            shadow.Draw(gameTime, _spriteBatch);

            _spriteBatch.End();
        }
    }
}
