using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Rapid_Prototyping_T7.Game
{
    class Player : IGameObject
    {
        private Texture2D sprite;
        private Texture2D Sprite
        {
            get { return sprite; }
            set { sprite = value; }
        }
        
        private Vector2 position; 
        private Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        
        private Vector2 velocity;
        private Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public Player()
        {

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, position, Color.White);
        }

        public void Initialize()
        {
            position = new Vector2(0, 0);
            velocity = new Vector2(0, 0);
        }

        public void LoadContent(ContentManager content)
        {
            sprite = content.Load<Texture2D>("Sprites\\Player\\Silhouette-Stick-Figure");
        }

        public void Update(GameTime gameTime)
        {
            
        }
    }
}
