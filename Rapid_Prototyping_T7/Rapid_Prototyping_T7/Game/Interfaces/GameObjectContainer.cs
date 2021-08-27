using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Rapid_Prototyping_T7.Game.Interfaces
{
    abstract class GameObjectContainer
    {
        protected List<GameObject> object_collection;
        public List<GameObject> Collection
        {
            get { return object_collection; }
            set { object_collection = value; }
        }

        public void Initialize()
        {
            foreach(GameObject obj in object_collection)
            {
                obj.Initialize();
            }
        }

        public void LoadContent(ContentManager content)
        {
            foreach (GameObject obj in object_collection)
            {
                obj.LoadContent(content);
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (GameObject obj in object_collection)
            {
                obj.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (GameObject obj in object_collection)
            {
                obj.Draw(gameTime, spriteBatch);
            }
        }
    }
}