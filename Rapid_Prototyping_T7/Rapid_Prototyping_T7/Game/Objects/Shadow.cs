using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Rapid_Prototyping_T7.Game.Objects
{
    class Shadow : GameObject
    {
        private Player player;

        public Shadow(Player in_player)
        {
            player = in_player;
            Initialize();
            LoadContent();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var scale = 1f;
            var rotation = 0f;
            var origin = new Vector2(sprite.Width / 2, sprite.Height / 2);
            var depth = 0;
            spriteBatch.Draw(sprite,
                position,
                null,
                Color.Black,
                rotation,
                origin,
                scale,
                SpriteEffects.FlipVertically,
                depth
                );
        }

        public override void Initialize()
        {
            position.X = player.Position.X;
            position.Y = player.Position.Y + 250;
        }

        public override void LoadContent()
        {
            sprite = player.Level.Content.Load<Texture2D>("Sprites/Player/Idle");
        }

        public override void Update(GameTime gameTime)
        {
            position.X = player.Position.X;
        }
    }
}
