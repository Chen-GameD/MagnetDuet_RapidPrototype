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
        public Vector2 size; // (width, height)
        private string image;

        public Tile(Vector2 in_position, Vector2 in_size, string sprite_image)
        {
            position = in_position;
            size = in_size;
        }

        public Tile(Rectangle rectangle, string sprite_image)
        {
            var x = rectangle.X + (rectangle.Width/2);
            var y = rectangle.Y + (rectangle.Height/2);
            position = new Vector2(x, y);

            size = new Vector2(rectangle.Width, rectangle.Height);
        }

        public override void Initialize() 
        { }

        public override void LoadContent(ContentManager content)
        {
            sprite = content.Load<Texture2D>("Sprites/Tiles/" + image); 
        }

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
