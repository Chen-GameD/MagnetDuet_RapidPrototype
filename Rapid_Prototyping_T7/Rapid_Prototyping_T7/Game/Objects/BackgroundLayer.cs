using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Rapid_Prototyping_T7.Game.Interfaces;

namespace Rapid_Prototyping_T7.Game.Objects
{
    class BackgroundLayer : GameObject
    {
        private int layer;

        public BackgroundLayer(int in_layer)
        {
            layer = in_layer;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var scale = 1f;
            var rotation = 0f;
            var origin = new Vector2(0, 0);
            var depth = layer;
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
            position = new Vector2(0, 0);
        }

        public override void LoadContent(ContentManager content)
        {
            sprite = content.Load<Texture2D>("Backgrounds/Layer" + layer + "_0");
        }

        public override void Update(GameTime gameTime)
        {}
    }
}
