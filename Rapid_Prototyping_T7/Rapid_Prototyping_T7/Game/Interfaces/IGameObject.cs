using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Rapid_Prototyping_T7.Game
{
    interface IGameObject
    {
        public void Initialize();

        public void LoadContent(ContentManager content);

        public void Update(GameTime gameTime);

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
