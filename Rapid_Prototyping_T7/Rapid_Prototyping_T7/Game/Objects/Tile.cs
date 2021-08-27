using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Rapid_Prototyping_T7.Game.Interfaces;
using Rapid_Prototyping_T7.Game.Constants;

namespace Rapid_Prototyping_T7.Game.Objects
{

    enum TileCollision
    {
        Passable = 0,      //can pass
        Impassable = 1,    //can not pass
    }

    class Tile : GameObject
    {
        public Texture2D Texture;
        public TileCollision Collision;

        public const int Width = 40;
        public const int Height = 32;

        public static readonly Vector2 Size = new Vector2(Width, Height);

        public Tile(Texture2D texture, TileCollision collision)
        {
            Texture = texture;
            Collision = collision;
        }

        public override void Initialize() 
        { }

        public override void LoadContent(ContentManager content)
        { }

        public override void Update(GameTime gameTime)
        { }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var scale = 1f;
            var rotation = 0f;
            var origin = new Vector2(sprite.Width / 2, sprite.Height / 2);
            var depth = (int)Layers.level;
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

    }
}
