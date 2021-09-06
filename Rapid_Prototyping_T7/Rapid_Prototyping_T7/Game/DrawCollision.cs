using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rapid_Prototyping_T7.Game
{
    class DrawCollision
    {
        private GraphicsDeviceManager graphics;
        private Texture2D sprite;
        private Rectangle rectangle;
        private float angleLine_1;
        private float angleLine_2;
        private float angleLine_3;
        private float angleLine_4;
        private Rectangle line_1;
        private Rectangle line_2;
        private Rectangle line_3;
        private Rectangle line_4;

        public DrawCollision(GraphicsDeviceManager grap, Rectangle rectangle)
        {
            graphics = grap;
            Initialize();
        }

        public void Initialize()
        {
            sprite = new Texture2D(graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            sprite.SetData(new[] { Color.White });
        }

        public void LoadContent()
        {
            throw new NotImplementedException();
        }

        public void Update(Rectangle rec, Vector2 origin)
        {
            rectangle = rec;
            angleLine_1 = (float)Math.Atan2(0, rectangle.Width);
            angleLine_2 = (float)Math.Atan2(rectangle.Height, 0);
            angleLine_3 = (float)Math.Atan2(0, -rectangle.Width);
            angleLine_4 = (float)Math.Atan2(-rectangle.Height, 0);
            line_1 = new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, 1);
            line_2 = new Rectangle(rectangle.X + rectangle.Width, rectangle.Y, rectangle.Height, 1);
            line_3 = new Rectangle(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height, rectangle.Width, 1);
            line_4 = new Rectangle(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Height, 1);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprite, line_1, null, Color.White, angleLine_1, new Vector2(0, 0), SpriteEffects.None, 0);
            spriteBatch.Draw(sprite, line_2, null, Color.White, angleLine_2, new Vector2(0, 0), SpriteEffects.None, 0);
            spriteBatch.Draw(sprite, line_3, null, Color.White, angleLine_3, new Vector2(0, 0), SpriteEffects.None, 0);
            spriteBatch.Draw(sprite, line_4, null, Color.White, angleLine_4, new Vector2(0, 0), SpriteEffects.None, 0);
        }
    }
}
