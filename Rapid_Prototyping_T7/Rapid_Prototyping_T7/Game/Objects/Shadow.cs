using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Rapid_Prototyping_T7.Game.Objects
{
    class Shadow : GameObject
    {
        private Player player;

        public Vector2 velocity;
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        private Vector2 previous_position;

        public Rectangle BoundingRectangle
        {
            get
            {
                int width = (int)(sprite.Width * player.scale);
                int height = (int)(sprite.Height * player.scale);
                int left = (int)Math.Round(Position.X - (width / 2));
                int top = (int)Math.Round(Position.Y - (height / 2));

                return new Rectangle(left, top, width, height);
            }
        }

        public Level Level
        {
            get { return level; }
        }
        Level level;

        public Shadow(Level level, Vector2 in_position, Player in_player)
        {
            this.level = level;
            player = in_player;
            Initialize();
            LoadContent();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var rotation = 0f;
            var origin = new Vector2(sprite.Width / 2, sprite.Height / 2);
            var depth = 0;
            spriteBatch.Draw(sprite,
                position,
                null,
                Color.Black,
                rotation,
                origin,
                player.scale,
                SpriteEffects.FlipVertically,
                depth
                );
        }

        public override void Initialize()
        {
            position.X = player.Position.X;
            position.Y = player.Position.Y + 250;
        }

        public override void LoadContent()
        {
            sprite = player.Level.Content.Load<Texture2D>("Sprites/Player/Silhouette-Stick-Figure");
        }

        public override void Update(GameTime gameTime)
        {
            previous_position = position;
            var kstate = Keyboard.GetState();
            var distance_to_player = Vector2.Distance(position, player.Position);
            if (kstate.IsKeyDown(Keys.Space))
            {
                var repulsion = -1 * player.repulse_force / MathF.Pow(distance_to_player, player.distance_decay_exponant) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                velocity.Y -= MathF.Max(repulsion, -player.max_repulsion);
            }
            else
            {
                velocity.Y -= player.attract_force / MathF.Pow(distance_to_player, player.distance_decay_exponant) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            velocity.Y -= player.acceleration_gravity;
            if (velocity.Y > 0)
            {
                velocity.Y = MathF.Min(velocity.Y, player.max_speed_vertical_up);
            }
            else
            {
                velocity.Y = MathF.Max(velocity.Y, -player.max_speed_vertical_down);
            }

            // Move
            position.Y += velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;
            position.X = player.Position.X;

            HandleCollisions();
        }

        private void HandleCollisions()
        {
            // Get the player's bounding rectangle and find neighboring tiles.
            Rectangle bounds = BoundingRectangle;
            int leftTile = (int)Math.Floor((float)bounds.Left / Tile.Width);
            int rightTile = (int)Math.Ceiling(((float)bounds.Right / Tile.Width)) - 1;
            int topTile = (int)Math.Floor((float)bounds.Top / Tile.Height);
            int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / Tile.Height)) - 1;

            // For each potentially colliding tile,
            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    // If this tile is collidable,
                    TileCollision collision = Level.GetCollision(x, y);
                    if (collision != TileCollision.Passable)
                    {
                        // Determine collision depth (with direction) and magnitude.
                        Rectangle tileBounds = Level.GetBounds(x, y);
                        if (bounds.Intersects(tileBounds) && collision == TileCollision.Impassable)
                        {
                            Rectangle intersection;
                            Rectangle.Intersect(ref bounds, ref tileBounds, out intersection);
                            Vector2 movement = position - previous_position;
                            if (intersection.Height > intersection.Width)
                            {
                                // Horizontal collision
                                player.velocity.X = 0f;
                                if (movement.X > 0)
                                {
                                    player.position.X -= intersection.Width;
                                    position.X -= intersection.Width;
                                }
                                else
                                {
                                    player.position.X += intersection.Width;
                                    position.X += intersection.Width;
                                }
                            }
                            else
                            {
                                // Vertical collision
                                velocity.Y = 0f;
                                if (movement.Y > 0)
                                {
                                    position.Y -= intersection.Height;
                                }
                                else
                                {
                                    position.Y += intersection.Height;
                                }

                            }

                            // Perform further collisions with the new bounds.
                            bounds = BoundingRectangle;

                        }
                    }
                }
            }
        }
    }
}
