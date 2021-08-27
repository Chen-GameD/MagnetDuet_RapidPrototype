using Microsoft.Xna.Framework;
using Rapid_Prototyping_T7.Game.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rapid_Prototyping_T7.Game
{
    class Camera
    {
        public Matrix Transform
        {
            get;
            private set;
        }

        public void Follow(Player player)
        {
            var position = Matrix.CreateTranslation(
                -player.Position.X - (player.Rectangle.Width / 2),
                -player.Position.Y - (player.Rectangle.Height / 2),
                0);
            var offset =  Matrix.CreateTranslation(
                Game1.ScreenWidth / 2,
                Game1.ScreenHeight / 2,
                0);

            Transform = position * offset;
        }
    }
}
