using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Rapid_Prototyping_T7.Game.Objects;

namespace Rapid_Prototyping_T7.Game
{
    public static class Jump
    {
        public static float max_speed_vertical_up = 200f;
        public static float max_speed_vertical_down = 3000f;
        public static float max_repulsion = 150f;
        public static float repulse_force = 500000f * 10;
        public static float attract_force = 250f;
        public static float acceleration_gravity = 100f;
        public static float distance_decay_exponant = 1.1f;

        public static float battery_duration = 0f;
        public static float battery_getCollected = 5f;
        public static float super_jump_force_multiplyer = 1.5f;

        public static float jump_stregnth_max = 1f;
        public static float jump_stregnth_current = 0f;
        public static float jump_stregnth_decay = 0.00000000001f;

        public static float GetVerticalVelocityChange(GameTime gameTime, float distance)
        {
            Vector2 velocity = new Vector2(0, 0);

            var kstate = Keyboard.GetState();
            if (kstate.IsKeyDown(Keys.Space))
            {
                var repulsion = -1 * repulse_force / MathF.Pow(distance, distance_decay_exponant) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (battery_duration > 0)
                {
                    repulsion *= super_jump_force_multiplyer;
                    battery_duration -= 1 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (battery_duration < 0f)
                        battery_duration = 0f;
                }
                else
                {
                    repulsion *= jump_stregnth_current;
                }
                velocity.Y += MathF.Max(repulsion, -max_repulsion);
            }
            else
            {
                velocity.Y += attract_force / MathF.Pow(distance, distance_decay_exponant) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            velocity.Y += acceleration_gravity;
            return velocity.Y;
        }

    }
}
