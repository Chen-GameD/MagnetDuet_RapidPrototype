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
        public Texture2D Sprite
        {
            get { return sprite; }
            set { sprite = value; }
        }
        
        private Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        
        private Vector2 velocity;
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        private float acceleration = 20f;
        private float speed_decay = 0.95f;
        private float max_speed = 20f;
        private float min_speed = 10f;


        public Player()
        {

        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var scale = 0.25f;
            var rotation = 0f;
            var origin = new Vector2(sprite.Width / 2, sprite.Height / 2);
            var depth = 0;
            spriteBatch.Draw(sprite, 
                position, 
                null, 
                Color.White, 
                rotation,
                origin,
                scale, 
                SpriteEffects.None, 
                depth
                );
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
            var kstate = Keyboard.GetState();

            if (kstate.IsKeyDown(Keys.Left))
            {
                velocity.X -= acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
                velocity.X = MathF.Max(velocity.X, -max_speed);
            }
            else if (kstate.IsKeyDown(Keys.Right))
            {
                velocity.X += acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
                velocity.X = MathF.Min(velocity.X, max_speed);
            }
            else 
            { 
                velocity.X *= speed_decay;
                if (velocity.X < min_speed)
                { 
                    velocity.X = 0.0f; 
                } 
            }

            position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

        }
    }
}
