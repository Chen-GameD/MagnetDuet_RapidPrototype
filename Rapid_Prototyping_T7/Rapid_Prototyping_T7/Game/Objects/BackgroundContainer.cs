using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Rapid_Prototyping_T7.Game.Interfaces;

namespace Rapid_Prototyping_T7.Game.Objects
{
    class BackgroundContainer : GameObjectContainer
    {
        public BackgroundContainer()
        {
            object_collection = new List<GameObject>();
            for (int i = 0; i < 3; i++)
            {
                object_collection.Add(new BackgroundLayer(i));
            }
        }
    }
}
