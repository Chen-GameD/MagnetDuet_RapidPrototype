using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Rapid_Prototyping_T7.Game.Objects
{
    public class Shadow : GameObject
    {
        private Animation idleAnimation;
        private Animation walkAnimation;
        private Animation jumpAnimation;
        private AnimationPlayer anim_sprite;
        private SpriteEffects flip = SpriteEffects.None;

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
                int width = (int)(anim_sprite.Animation.FrameWidth * player.scale);
                int height = (int)(anim_sprite.Animation.FrameWidth * player.scale);
                int left = (int)Math.Round(Position.X - (width / 2));
                int top = (int)Math.Round(Position.Y - (height));

                return new Rectangle(left, top, width, height);
            }
        }

        public Level Level
        {
            get { return level; }
        }
        Level level;

        public bool IsOnGround
        {
            get { return isOnGround; }
        }
        bool isOnGround;

        public Shadow(Level level, Vector2 in_position, Player in_player)
        {
            this.level = level;
            player = in_player;
            Initialize();
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
            //    Color.Black,
            //    rotation,
            //    origin,
            //    player.scale,
            //    SpriteEffects.FlipVertically,
            //    depth
            //    );

            if (player.Velocity.X < 0)
                flip = SpriteEffects.FlipHorizontally;
            else if (player.Velocity.X > 0)
                flip = SpriteEffects.None;

            anim_sprite.Draw(gameTime, spriteBatch, Position, flip, player.scale);
        }

        public override void Initialize()
        {
            position.X = player.Position.X;
            position.Y = player.Position.Y + 250;
        }

        public override void LoadContent()
        {
            sprite = player.Level.Content.Load<Texture2D>("Sprites/Player/JohnnyGreenHead");

            idleAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/FlippedCharacterIdle"), 0.1f, true);
            walkAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/FlippedCharacterWalk"), 0.1f, true);
            jumpAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/FlippedCharacterIdle"), 0.1f, true);
        }

        public override void Update(GameTime gameTime)
        {
            previous_position = position;

            var distance = Vector2.Distance(player.previous_position, position);
            velocity.Y -= Jump.GetVerticalVelocityChange(gameTime, distance);
            if (velocity.Y < 0)
            {
                velocity.Y = MathF.Max(velocity.Y, -Jump.max_speed_vertical_down);
            }
            else
            {
                velocity.Y = MathF.Min(velocity.Y, Jump.max_speed_vertical_up);
            }

            if (player.IsAlive && isOnGround)
            {
                if (Math.Abs(player.Velocity.X) - 0.02f > 0)
                {
                    anim_sprite.PlayAnimation(walkAnimation);
                }
                else
                {
                    anim_sprite.PlayAnimation(idleAnimation);
                }
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
        }

        public void Reset(Vector2 in_position)
        {
            position = in_position;
            previous_position = in_position;
            velocity = Vector2.Zero;
            anim_sprite.PlayAnimation(idleAnimation);
        }
    }
}
