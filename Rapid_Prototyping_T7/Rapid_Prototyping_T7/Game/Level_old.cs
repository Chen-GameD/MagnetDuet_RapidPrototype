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
    class Level_old : GameObjectContainer
    {
        private Tile[,] tiles;
        private Texture2D[] layers;

        private const int EntityLayer = 2;

        // Level game state.
        private Random random = new Random(354668); // Arbitrary, but constant seed

        public ContentManager Content
        {
            get { return content; }
        }
        ContentManager content;

        public Level_old(IServiceProvider serviceProvider, Stream fileStream, int levelIndex)
        {
            content = new ContentManager(serviceProvider, "Content");

            LoadTiles(fileStream);

            layers = new Texture2D[3];
            for (int i = 0; i < layers.Length; i++)
            {
                int segmentIndex = levelIndex;
                layers[i] = Content.Load<Texture2D>("Backgrounds/Layer" + i + "_" + segmentIndex);
            }

        }

        private void LoadTiles(Stream fileStream)
        {
            // Load the level and ensure all of the lines are the same length.
            int width;
            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader(fileStream))
            {
                string line = reader.ReadLine();
                width = line.Length;
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
        }

        private Tile LoadTile(char tileType, int x, int y)
        {
            switch (tileType)
            {
                // Blank space
                case '.':
                    return new Tile(null, TileCollision.Passable);

                // Impassable block
                case '#':
                    return LoadVarietyTile("BlockA", 7, TileCollision.Impassable);

                // Passable block
                case ':':
                    return LoadVarietyTile("BlockB", 2, TileCollision.Passable);

                // Unknown tile type character
                default:
                    throw new NotSupportedException(String.Format("Unsupported tile type character '{0}' at position {1}, {2}.", tileType, x, y));
            }
        }

        private Tile LoadTile(string tileName, TileCollision collision)
        {
            return new Tile(Content.Load<Texture2D>("Sprites/Tiles/" + tileName), collision);
        }

        private Tile LoadVarietyTile(string tileName, int variationCount, TileCollision collision)
        {
            int index = random.Next(variationCount);
            return LoadTile(tileName + index, collision);
        }

        public void Dispose()
        {
            Content.Unload();
        }

        public TileCollision GetCollision(int x, int y)
        {
            // Prevent escaping past the level ends.
            if (x < 0 || x >= Width)
                return TileCollision.Impassable;
            // Allow jumping past the level top and falling through the bottom.
            if (y < 0 || y >= Height)
                return TileCollision.Passable;

            return tiles[x, y].Collision;
        }

        public Rectangle GetBounds(int x, int y)
        {
            return new Rectangle(x * Tile.Width, y * Tile.Height, Tile.Width, Tile.Height);
        }

        /// <summary>
        /// Width of level measured in tiles.
        /// </summary>
        public int Width
        {
            get { return tiles.GetLength(0); }
        }

        /// <summary>
        /// Height of the level measured in tiles.
        /// </summary>
        public int Height
        {
            get { return tiles.GetLength(1); }
        }


        public void Update()
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i <= EntityLayer; i++)
            {
                spriteBatch.Draw(layers[i], Vector2.Zero, Color.White);
            }

            DrawTiles(spriteBatch);

            for (int i = EntityLayer + 1; i < layers.Length; ++i)
            {
                spriteBatch.Draw(layers[i], Vector2.Zero, Color.White);
            }
                
        }

        private void DrawTiles(SpriteBatch spriteBatch)
        {
            for (int y = 0; y < Height; ++y)
            {
                // For each tile position
                for (int x = 0; x < Width; ++x)
                {
                    // If there is a visible tile in that position
                    Texture2D texture = tiles[x, y].Texture;
                    if (texture != null)
                    {
                        // Draw it in screen space.
                        Vector2 position = new Vector2(x, y) * Tile.Size;
                        spriteBatch.Draw(texture, position, Color.White);
                    }
                }
            }
        }
    }
}
