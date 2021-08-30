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
        const float BLOCK_SIZE_WIDTH = 100f;
        const float BLOCK_SIZE_HEIGHT = 100f;

        public int levelIndex = -1;
        public int numberOfLevels = 3;
        
        private Random random = new Random(354668); // Arbitrary, but constant seed


        public Level LoadNextLevel()
        {
            // move to the next level
            levelIndex = (levelIndex + 1) % numberOfLevels;

            // Load the level.
            string levelPath = string.Format("Content/Levels/{0}.txt", levelIndex);
            Level level = loadTilesFromFile(levelPath);
            return level;
        }

        private Level loadTilesFromFile(string filename)
        {
            Level level = new Level();

            int max_width = -1;
            var fileStream = TitleContainer.OpenStream(filename);
            using (StreamReader reader = new StreamReader(fileStream))
            {
                string line = reader.ReadLine();
                max_width = Math.Max(line.Length, max_width);

                while (line != null)
                {
                    lines.Add(line);
                    if (line.Length != width)
                        throw new Exception(String.Format("The length of line {0} is different from all preceeding lines.", lines.Count));
                    line = reader.ReadLine();
                }
            }

            // Allocate the tile grid.
            tiles = new Tile[width, lines.Count];

            // Loop over every tile position,
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    // to load each tile.
                    char tileType = lines[y][x];
                    tiles[x, y] = LoadTile(tileType, x, y);
                }
            }

            return level;
        }

        private string pickRandTileSprite(string tileBaseName, int numVarieties)
        {
            var randInt = random.Next(0, numVarieties);
            return tileBaseName + randInt.ToString();
        }

        private void makeTile(Level level, char tileType, Vector2 file_position)
        {
            var block_size = new Vector2(BLOCK_SIZE_WIDTH, BLOCK_SIZE_HEIGHT);
            var block_position = new Vector2(file_position.X * BLOCK_SIZE_WIDTH, file_position.Y * BLOCK_SIZE_HEIGHT);
            string image;
            switch (tileType)
            {
                // Blank space
                case '.':
                    return;
                // Impassable block
                case '#':
                    image = pickRandTileSprite("BlockA", 7);
                    break;
                // Passable block
                case ':':
                    image = pickRandTileSprite("BlockB", 2);
                    break;
                // Unknown tile type character
                default:
                    throw new NotSupportedException(String.Format("Unsupported tile type character '{0}' at position {1}, {2}.", tileType, file_position.X, file_position.Y));
            }
            var block = new Tile(block_position, block_size, image);
            level.Collection.Add(block);
        }
    }
}
