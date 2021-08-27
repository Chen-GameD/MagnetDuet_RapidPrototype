using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Rapid_Prototyping_T7.Game;
using Rapid_Prototyping_T7.Game.Objects;
using Rapid_Prototyping_T7.Game.GameLogic;

namespace Rapid_Prototyping_T7.Game.GameLogic
{
    class LevelMaker
    {
        public int levelIndex = -1;
        public int numberOfLevels = 3;

        public Level LoadNextLevel()
        {
            // move to the next level
            levelIndex = (levelIndex + 1) % numberOfLevels;

            // Load the level.
            Level level = new Level();
            string levelPath = string.Format("Content/Levels/{0}.txt", levelIndex);
            using (Stream fileStream = TitleContainer.OpenStream(levelPath))
            {
                
            }
            return level;
        }
    }
}
