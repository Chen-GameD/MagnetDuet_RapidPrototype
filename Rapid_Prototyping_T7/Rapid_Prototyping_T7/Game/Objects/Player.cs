using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;

namespace Rapid_Prototyping_T7.Game.Objects
{
    public class Player : GameObject
    {
        //Animations
        private Animation idleAnimation;
        private Animation walkAnimation;
        private AnimationPlayer anim_sprite;
        private SpriteEffects flip = SpriteEffects.None;

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

        public bool IsAlive
        {
            get { return isAlive; }
        }
        bool isAlive;

        private Shadow shadow;
        public void SetShadow(Shadow in_shadow)
        {
            shadow = in_shadow; 
        }

        private static float acceleration_horizontal = 2500f * 2;
        private static float speed_decay_horizontal = 0.95f;
        private static float max_speed_horizontal = 250f * 2;
        private static float min_speed_horizontal = 35f * 2;

        public Vector2 previous_position;
        public float scale = .32f;

        public bool IsOnGround
        {
            get { return isOnGround; }
        }
        bool isOnGround;

        public Rectangle BoundingRectangle
        {
            get
            {
                int width = (int)(anim_sprite.Animation.FrameWidth * scale);
                int height = (int)(anim_sprite.Animation.FrameWidth * scale);
                int left = (int)Math.Round(Position.X - (width / 2));
                int top = (int)Math.Round(Position.Y - height);

                return new Rectangle(left, top, width, height);
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
            //var rotation = 0f;
            //var origin = new Vector2(sprite.Width / 2, sprite.Height / 2);
            //var depth = 0;
            //spriteBatch.Draw(sprite,
            //    position,
            //    null,
            //    Color.White,
            //    rotation,
            //    origin,
            //    scale,
            //    SpriteEffects.None,
            //    depth
            //    );

            if (Velocity.X < 0)
                flip = SpriteEffects.FlipHorizontally;
            else if (Velocity.X > 0)
                flip = SpriteEffects.None;

            anim_sprite.Draw(gameTime, spriteBatch, Position, flip, scale);
        }

        public override void Initialize()
        {
        }

        public override void LoadContent()
        {
            sprite = level.Content.Load<Texture2D>("Sprites/Player/JohnnyGreenHead");
            //Load animated textures
            idleAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/CharacterIdle"), 0.1f, true);
            walkAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/CharacterWalk"), 0.1f, true);
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

            if (isAlive && isOnGround)
            {
                Trace.WriteLine(isOnGround);
                if (Math.Abs(Velocity.X) - 0.02f > 0)
                {
                    anim_sprite.PlayAnimation(walkAnimation);
                }
                else
                {
                    anim_sprite.PlayAnimation(idleAnimation);
                }
            }

            var distance = Vector2.Distance(position, shadow.position);
            velocity.Y += Jump.GetVerticalVelocityChange(gameTime, distance);
            if (velocity.Y > 0)
            {
                velocity.Y = MathF.Min(velocity.Y, Jump.max_speed_vertical_down);
            }
            else
            {
                velocity.Y = MathF.Max(velocity.Y, -Jump.max_speed_vertical_up);
            }

            Trace.WriteLine(velocity.Y);

            // Move
            position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            HandleCollisions();

            if (this.BoundingRectangle.Intersects(shadow.BoundingRectangle))
            {
                OnKilled();
            }
        }

        private void HandleCollisions()
        {
            // Get the player's bounding rectangle and find neighboring tiles.
            Rectangle bounds = BoundingRectangle;
            int leftTile = (int)Math.Floor((float)bounds.Left / Tile.Width);
            int rightTile = (int)Math.Ceiling(((float)bounds.Right / Tile.Width)) - 1;
            int topTile = (int)Math.Floor((float)bounds.Top / Tile.Height);
            int bottomTile = (int)Math.Ceiling((((float)bounds.Bottom) / Tile.Height)) - 1;

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
                                isOnGround = true;
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

            Trace.WriteLine(isOnGround);
        }

        public void OnKilled()
        {
            isAlive = false;
        }

        public void Reset(Vector2 in_position)
        {
            position = in_position;
            previous_position = in_position;
            velocity = Vector2.Zero;
            isAlive = true;
            anim_sprite.PlayAnimation(idleAnimation);
        }

    }
}
