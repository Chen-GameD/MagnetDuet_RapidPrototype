using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rapid_Prototyping_T7.Game
{

    enum TileCollision
    {
        Passable = 0,      //can pass
        Impassable = 1,    //can not pass
    }

    struct Tile
    {
        public Texture2D Texture;
        public TileCollision Collision;

        public const int Width = 32;
        public const int Height = 32;

        public static readonly Vector2 Size = new Vector2(Width, Height);

        public Tile(Texture2D texture, TileCollision collision)
        {
            Texture = texture;
            Collision = collision;
        }
    }
}
