using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Rapid_Prototyping_T7.Game.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Rapid_Prototyping_T7.Game
{
    public class Level : IDisposable
    {
        private Tile[,] tiles;
        private Texture2D[] layers;

        private const int EntityLayer = 0;

        private List<Prop> props = new List<Prop>();
        private List<Prop> propsStore = new List<Prop>();

        private SoundEffect exitReachedSound;

        // Level game state.
        private Random random = new Random(354668); // Arbitrary, but constant seed

        public ContentManager Content
        {
            get { return content; }
        }
        ContentManager content;

        public Player Player
        {
            get { return player; }
        }
        Player player;
        private Vector2 player_start; //player_start point

        public Shadow Shadow
        {
            get { return shadow; }
        }
        Shadow shadow;
        private Vector2 shadow_start;

        public bool ReachedExit
        {
            get { return reachedExit; }
        }
        bool reachedExit;

        public int Score
        {
            get { return score; }
        }
        int score;

        private Point exit = InvalidPosition;
        private static readonly Point InvalidPosition = new Point(-1, -1);

        public Level(IServiceProvider serviceProvider, Stream fileStream, int levelIndex)
        {
            content = new ContentManager(serviceProvider, "Content");

            player = new Player(this, new Vector2(0, 0));
            shadow = new Shadow(this, new Vector2(0, 0), player);
            player.SetShadow(shadow);
            player_start = Vector2.Zero;
            shadow_start = Vector2.Zero;


            LoadTiles(fileStream);

            layers = new Texture2D[1];
            for (int i = 0; i < layers.Length; i++)
            {
                int segmentIndex = levelIndex;
                layers[i] = Content.Load<Texture2D>("Backgrounds/Layer" + i + "_" + segmentIndex);
            }

            // Load sounds.
            exitReachedSound = Content.Load<SoundEffect>("Music/Win");
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

            if (Player == null)
                throw new NotSupportedException("A level must have a starting point.");
        }

        private Tile LoadTile(char tileType, int x, int y)
        {
            switch (tileType)
            {
                // Blank space
                case '.':
                    return new Tile(null, TileCollision.Passable);

                // Platform brick
                case '#':
                    return LoadVarietyTile("grid0", 1, TileCollision.Impassable);

                // Battery Prop
                case '@':
                    return LoadPropTile(x, y, PropType.Battery);

                // Star Prop
                case '*':
                    return LoadPropTile(x, y, PropType.Star);

                // Electric field
                case '%':
                    return LoadVarietyTile("ElectroField0", 1, TileCollision.ElectronicField);

                //Blue moving platform
                case '$':
                    return new Tile(null, TileCollision.Passable);
                    //return LoadVarietyTile("BlockB", 2, TileCollision.Passable);

                // Final Platform
                case '&':
                    return LoadExitTile(x, y);

                // Player 1 player_start point
                case '1':
                    return LoadStartTile(x, y, 1);

                case '2':
                    return LoadStartTile(x, y, 2);

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

        private Tile LoadPropTile(int x, int y, PropType type)
        {
            Point position = GetBounds(x, y).Center;
            props.Add(new Prop(this, new Vector2(position.X, position.Y), type));
            propsStore.Add(new Prop(this, new Vector2(position.X, position.Y), type));

            return new Tile(null, TileCollision.Passable);
        }

        private Tile LoadExitTile(int x, int y)
        {
            if (exit != InvalidPosition)
                throw new NotSupportedException("A level may only have one exit.");

            exit = GetBounds(x, y).Center;

            return LoadTile("Exit", TileCollision.Passable);
        }

        private Tile LoadStartTile(int x, int y, int player_number)
        {
            if (player_number == 1)
            {
                if (player_start != Vector2.Zero)
                    throw new NotSupportedException("A level may only have one starting point.");

                player_start = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
                player.position = player_start;
            }
            else
            {
                if (shadow_start != Vector2.Zero)
                    throw new NotSupportedException("A level may only have one starting point for the shadow.");
                shadow_start = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
                shadow.position = shadow_start;
            }
            return new Tile(null, TileCollision.Passable);
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


        public void Update(GameTime gameTime)
        {
            UpdateProp(gameTime);
            if (player.IsAlive && !ReachedExit)
            {
                player.Update(gameTime);
                shadow.Update(gameTime);
                if (player.BoundingRectangle.Contains(exit) || shadow.BoundingRectangle.Contains(exit))
                {
                    OnExitReached();
                }
            }
        }

        private void UpdateProp(GameTime gameTime)
        {
            for (int i = 0; i < props.Count; i++)
            {
                Prop prop = props[i];

                prop.Update(gameTime);

                if (prop.BoundingCircle.Intersects(Player.BoundingRectangle) || prop.BoundingCircle.Intersects(Shadow.BoundingRectangle))
                {
                    props.RemoveAt(i--);
                    OnPropCollected(prop, Player);
                }
            }
        }

        private void OnPropCollected(Prop prop, Player collectedBy)
        {
            switch (prop.Type)
            {
                case PropType.Battery:
                    //To do(Get some ability)
                    Jump.battery_duration = Jump.battery_getCollected;
                    break;
                case PropType.Star:
                    score += prop.PointValue;
                    break;
            }

            prop.OnCollected(collectedBy);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 camera_pos)
        {
            Vector2 layerPos = -camera_pos;
            for (int i = 0; i <= EntityLayer; i++)
            {
                spriteBatch.Draw(layers[i], layerPos, Color.White);
            }

            DrawTiles(spriteBatch);

            foreach (Prop prop in props)
            {
                prop.Draw(gameTime, spriteBatch);
            }

            for (int i = EntityLayer + 1; i < layers.Length; ++i)
            {
                spriteBatch.Draw(layers[i], Vector2.Zero, Color.White);
            }

            player.Draw(gameTime, spriteBatch);
            shadow.Draw(gameTime, spriteBatch);

            Trace.WriteLine(Score);

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
                        float scale = 2.0f;
                        var rotation = 0f;
                        var origin = new Vector2(0,0);
                        var depth = 0;// (float)Layer.level;
                        spriteBatch.Draw(texture,
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
        }

        private void OnExitReached()
        {
            reachedExit = true;
            exitReachedSound.Play();
        }

        public void StartNewLife()
        {
            reachedExit = false;
            Player.Reset(player_start);
            shadow.Reset(shadow_start);
            ReloadProp();
        }

        private void ReloadProp()
        {
            props = new List<Prop>();
            foreach(Prop prop in propsStore)
            {
                props.Add(prop);
            }
        }
    }
}
