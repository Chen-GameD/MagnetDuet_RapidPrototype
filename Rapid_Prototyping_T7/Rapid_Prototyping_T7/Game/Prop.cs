using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rapid_Prototyping_T7.Game
{
    enum PropType
    {
        Battery = 0,      
        Star = 1,    
    }

    class Prop
    {
        private Texture2D texture;
        private Vector2 origin;
        private PropType type;

        public readonly int PointValue = 30;

        private Vector2 basePosition;
        private float bounce;

        public Level Level
        {
            get { return level; }
        }
        Level level;

        public PropType Type
        {
            get { return type; }
        }

        public Vector2 Position
        {
            get
            {
                return basePosition + new Vector2(0.0f, bounce);
            }
        }

        public Circle BoundingCircle
        {
            get
            {
                return new Circle(Position, Tile.Width / 3.0f);
            }
        }

        public Prop(Level level, Vector2 position, PropType type)
        {
            this.level = level;
            this.basePosition = position;
            this.type = type;

            LoadContent();
        }

        public void LoadContent()
        {
            switch(Type)
            {
                case PropType.Battery:
                    texture = Level.Content.Load<Texture2D>("Sprites/Battery");
                    break;
                case PropType.Star:
                    texture = Level.Content.Load<Texture2D>("Sprites/Star");
                    break;
            }
            
            origin = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);
        }

        public void Update(GameTime gameTime)
        {
            // Bounce control constants
            const float BounceHeight = 0.18f;
            const float BounceRate = 3.0f;
            const float BounceSync = -0.75f;

            // Bounce along a sine curve over time.
            // Include the X coordinate so that neighboring gems bounce in a nice wave pattern.            
            double t = gameTime.TotalGameTime.TotalSeconds * BounceRate + Position.X * BounceSync;
            bounce = (float)Math.Sin(t) * BounceHeight * texture.Height;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, Color.White, 0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);

            var rotation = 0f;
            var depth = 0f;// (float)Layer.items;
            float scale = 2.0f;
            spriteBatch.Draw(texture,
                Position,
                null,
                Color.White,
                rotation,
                origin,
                scale,
                SpriteEffects.None,
                depth
                );
        }
    }
}
