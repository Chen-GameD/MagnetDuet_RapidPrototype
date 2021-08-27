using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Rapid_Prototyping_T7.Game.Objects
{
    class Player : GameObject
    {    
        private Vector2 velocity;
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public Level Level
        {
            get { return level; }
        }
        Level level;

        private float acceleration = 2500;
        private float speed_decay = 0.95f;
        private float max_speed =  250f;
        private float min_speed = 35f;

        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, sprite.Width, sprite.Height);
            }
        }


        public Player(Level level, Vector2 position)
        {
            this.level = level;

            LoadContent();

            Reset(position);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
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

        public override void Initialize()
        {
            //position = new Vector2(0, 0);
            //velocity = new Vector2(0, 0);
        }

        public override void LoadContent()
        {
            sprite = level.Content.Load<Texture2D>("Sprites/Player/Silhouette-Stick-Figure");
        }

        public override void Update(GameTime gameTime)
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
                if (MathF.Abs(velocity.X) < min_speed)
                { 
                    velocity.X = 0.0f; 
                } 
            }

            position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

        }

        public void Reset(Vector2 position)
        {
            Position = position;
            Velocity = Vector2.Zero;
        }
    }
}
