/* LASER BIKES
 * 
 * Inspired by game created in 3D Buzz XNA Extreme 101 class
 * 
 * By: Ernest Huntley
 */

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LaserBikes.Classes
{
    class Actor
    {
        public static List<Actor> Actors;

        public Vector2 Position;
        public Vector2 Origin;

        static Actor()
        {
            Actors = new List<Actor>();
        }

        public Actor()
        {
            Actors.Add(this);
        }

        public virtual void Update(GameTime gameTime) { }

        public virtual void Draw(SpriteBatch spriteBatch) { }
    }
}
