using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Rapid_Prototyping_T7.Game.Objects;
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
        Animation propAnim;
        AnimationPlayer animPlayer;
        private PropType type;
        private float scale;
        private SoundEffect collectedSound;

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
                return new Circle(Position, Tile.Width * scale / 3.0f);
            }
        }

        public Prop(Level level, Vector2 position, PropType type)
        {
            this.level = level;
            this.basePosition = position;
            this.type = type;
            if (type == PropType.Battery)
            {
                scale = 2f;
            }
            else if (type == PropType.Star)
            {
                scale = 1.2f;
            }

            LoadContent();
        }

        public void LoadContent()
        {
            switch(Type)
            {
                case PropType.Battery:
                    propAnim = new Animation(Level.Content.Load<Texture2D>("Sprites/BatteryAnim"), 0.1f, true);
                    break;
                case PropType.Star:
                    propAnim = new Animation(Level.Content.Load<Texture2D>("Sprites/diamond"), 0.1f, true);
                    break;
            }
            collectedSound = Level.Content.Load<SoundEffect>("Music/Collected");
            animPlayer.PlayAnimation(propAnim);
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
            bounce = (float)Math.Sin(t) * BounceHeight * propAnim.FrameHeight;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            animPlayer.Draw(gameTime, spriteBatch, Position, SpriteEffects.None, scale);
        }

        public void OnCollected(Player collectedBy)
        {
            collectedSound.Play();
        }
    }
}
