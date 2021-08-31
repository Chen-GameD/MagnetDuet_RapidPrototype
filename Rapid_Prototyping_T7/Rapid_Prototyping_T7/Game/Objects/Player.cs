using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Rapid_Prototyping_T7.Game.Objects
{
    class Player : GameObject
    {
        public Vector2 velocity;
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public Level Level
        {
            get { return level; }
        }
        Level level;

        private Shadow shadow;
        public void SetShadow(Shadow in_shadow)
        {
            shadow = in_shadow;
        }

        private float acceleration_horizontal = 2500f;
        private float speed_decay_horizontal = 0.95f;
        private float max_speed_horizontal = 250f;
        private float min_speed_horizontal = 35f;

        public float max_speed_vertical_up = 2000f;
        public float max_speed_vertical_down = 3000f;
        public float max_repulsion = 2500f;
        public float repulse_force = 2500000f;
        public float attract_force = 1f;
        public float acceleration_gravity = 10f;

        private Vector2 previous_position;
        public float scale = 0.1f;

        public bool IsOnGround
        {
            get { return isOnGround; }
        }
        bool isOnGround;

        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, (int)(sprite.Width * scale), (int)(sprite.Height * scale));
            }
        }
        private Rectangle localBounds;

        public Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - (sprite.Width * scale / 2)) + localBounds.X;
                int top = (int)Math.Round(Position.Y - sprite.Height * scale) + localBounds.Y;

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }


        public Player(Level level, Vector2 in_position)
        {
            this.level = level;

            LoadContent();

            Reset(in_position);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var rotation = 0f;
            var origin = new Vector2(sprite.Width / 2, sprite.Height);
            var depth = 0;
            spriteBatch.Draw(sprite,
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

        public override void Initialize()
        {
            //position = new Vector2(0, 0);
            //velocity = new Vector2(0, 0);
        }

        public override void LoadContent()
        {
            sprite = level.Content.Load<Texture2D>("Sprites/Player/Silhouette-Stick-Figure");
            int width = (int)(sprite.Width * scale);
            int left = ((int)(sprite.Width * scale) - width) / 2;
            int height = (int)(sprite.Height * scale);
            int top = (int)(sprite.Height * scale) - height;
            localBounds = new Rectangle(left, top, width, height);
        }

        public override void Update(GameTime gameTime)
        {
            previous_position = position;

            var kstate = Keyboard.GetState();

            // Get player input
            if (kstate.IsKeyDown(Keys.Left) || kstate.IsKeyDown(Keys.A))
            {
                velocity.X -= acceleration_horizontal * (float)gameTime.ElapsedGameTime.TotalSeconds;
                velocity.X = MathF.Max(velocity.X, -max_speed_horizontal);
            }
            else if (kstate.IsKeyDown(Keys.Right) || kstate.IsKeyDown(Keys.D))
            {
                velocity.X += acceleration_horizontal * (float)gameTime.ElapsedGameTime.TotalSeconds;
                velocity.X = MathF.Min(velocity.X, max_speed_horizontal);
            }
            else
            {
                velocity.X *= speed_decay_horizontal;
                if (MathF.Abs(velocity.X) < min_speed_horizontal)
                {
                    velocity.X = 0.0f;
                }
            }
            var distance_to_shadow = Vector2.Distance(position, shadow.Position);
            if (kstate.IsKeyDown(Keys.Space))
            {
                var repulsion = -1 * repulse_force / MathF.Pow(distance_to_shadow, 1.5f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                velocity.Y += MathF.Max(repulsion, -max_repulsion);
            }
            else
            {
                velocity.Y += attract_force / MathF.Pow(distance_to_shadow, 1.5f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            velocity.Y += acceleration_gravity;
            if (velocity.Y > 0)
            {
                velocity.Y = MathF.Min(velocity.Y, max_speed_vertical_down);
            }
            else
            {
                velocity.Y = MathF.Max(velocity.Y, -max_speed_vertical_up);
            }

            // Move
            position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

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

            // Reset flag to search for ground collision.
            isOnGround = false;

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
                                velocity.X = 0f;
                                if (movement.X > 0)
                                {
                                    position.X -= intersection.Width;
                                }
                                else
                                {
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

        public void Reset(Vector2 in_position)
        {
            position = in_position;
            previous_position = in_position;
            velocity = Vector2.Zero;
        }

        //Position is the Center bottom of the sprite. So it should relocalize for the left-top corner.
        /*private Vector2 RelocalizePosition(Vector2 pos)
        {
            var posX = pos.X - (Rectangle.Width * 0.25f) / 2;
            var posY = pos.Y - Rectangle.Height * 0.25f;
            return new Vector2(posX, posY);
        }*/
    }
}
