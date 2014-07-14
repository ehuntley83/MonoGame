/* LASER BIKES
 * 
 * Inspired by game created in 3D Buzz XNA Extreme 101 class
 * 
 * By: Ernest Huntley
 */

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LaserBikes.Classes
{
    struct Direction
    {
        public static Vector2 Up { get { return new Vector2(0f, -1f); } }
        public static Vector2 Down { get { return new Vector2(0f, 1f); } }
        public static Vector2 Left { get { return new Vector2(-1f, 0f); } }
        public static Vector2 Right { get { return new Vector2(1f, 0f); } }
    }

    class Bike : Actor
    {
        private Vector2 direction;
        private Vector2 desiredDirection;
        public PlayerIndex PlayerIndex;

        private double timeRemaining;
        private double moveInterval;
        private bool isAlive;

        public bool IsAlive
        {
            get { return this.isAlive; }
        }

        public double MoveInterval
        {
            get { return this.moveInterval; }
            set
            {
                this.moveInterval = value;
                if (this.timeRemaining > this.moveInterval)
                    this.timeRemaining = this.moveInterval;
            }
        }

        public Bike(PlayerIndex playerIndex, Vector2 position, Vector2 direction)
        {
            this.isAlive = true;
            this.PlayerIndex = playerIndex;
            this.Position = position;
            this.direction = direction;
            this.desiredDirection = direction;

            this.moveInterval = LaserBikes.BikeMoveInterval;
            this.Origin = LaserBikes.BikeOrigin;
        }

        public override void Update(GameTime gameTime)
        {
            if (this.moveInterval < LaserBikes.BikeStopThreshold)
            {
                this.timeRemaining -= gameTime.ElapsedGameTime.TotalSeconds;

                if (this.timeRemaining <= 0)
                {
                    this.Move();
                    this.timeRemaining = this.moveInterval;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 drawPosition;
            Vector2 startPosition = this.Position * LaserBikes.GRID_BLOCK_SIZE;
            Vector2 endPosition = (this.Position + this.direction) * LaserBikes.GRID_BLOCK_SIZE;

            // interpolate between the block spaces in order to smooth out movement between them
            // this way we don't "jump" from grid block to grid block
            if (this.timeRemaining > 0 && this.moveInterval > 0)
            {
                float percent = 1f - (float)(this.timeRemaining / this.moveInterval);
                drawPosition = Vector2.Lerp(startPosition, endPosition, percent);
            }
            else
            {
                drawPosition = startPosition;
            }

            drawPosition += new Vector2(LaserBikes.GRID_BLOCK_SIZE / 2);     // this offsets the draw so that the wall emits from the middle of the bike, not the side

            float rotation = (float)Math.Atan2(this.direction.Y, this.direction.X) + (MathHelper.Pi / 2);
            Color tailColor = LaserBikes.PlayerColors[(int)this.PlayerIndex];

            spriteBatch.Draw(LaserBikes.BikeTexture, drawPosition, null, Color.White, rotation, this.Origin, 1f, SpriteEffects.None, 0);
            spriteBatch.Draw(LaserBikes.TailTexture, drawPosition, null, tailColor, rotation, this.Origin, 1f, SpriteEffects.None, 0);
        }

        private void Move()
        {
            this.Position += this.direction;

            int x = (int)this.Position.X;
            int y = (int)this.Position.Y;

            // make sure bike is within the grid before attempting to access the segments
            if (x > -1 && y > -1 && x < LaserBikes.GRID_WIDTH && y < LaserBikes.GRID_HEIGHT)
            {
                // if current segment is filled, we have collided with a wall
                if (Wall.Segments[x, y].Filled)
                    DestroyBike();

                Wall.Segments[x, y].Filled = true;
                Wall.Segments[x, y].PlayerIndex = this.PlayerIndex;

                // if the current direction is != to desiredDirection we are turning
                if (this.direction != this.desiredDirection)
                {
                    if ((this.direction == Direction.Left && this.desiredDirection == Direction.Down) ||
                        (this.direction == Direction.Up && this.desiredDirection == Direction.Right))
                        Wall.Segments[x, y].TextureIndex = 2;
                    else if ((this.direction == Direction.Right && this.desiredDirection == Direction.Down) ||
                             (this.direction == Direction.Up && this.desiredDirection == Direction.Left))
                        Wall.Segments[x, y].TextureIndex = 3;
                    else if ((this.direction == Direction.Right && this.desiredDirection == Direction.Up) ||
                             (this.direction == Direction.Down && this.desiredDirection == Direction.Left))
                        Wall.Segments[x, y].TextureIndex = 4;
                    else if ((this.direction == Direction.Left && this.desiredDirection == Direction.Up) ||
                             (this.direction == Direction.Down && this.desiredDirection == Direction.Right))
                        Wall.Segments[x, y].TextureIndex = 5;

                    this.direction = this.desiredDirection;
                }
                else    // not turning, make sure we draw horizontal or vertical based on current direction
                {
                    if (this.direction.X != 0f)
                        Wall.Segments[x, y].TextureIndex = 0;
                    else
                        Wall.Segments[x, y].TextureIndex = 1;
                }
            }
            else   // if the bike is not within the grid, then it is technically outside the screen & should be destroyed
            {
                DestroyBike();
            }
        }

        public void ChangeDirection(Vector2 desiredDirection)
        {
            if (desiredDirection != this.direction * -1)
                this.desiredDirection = desiredDirection;
        }

        private void DestroyBike()
        {
            this.isAlive = false;
            LaserBikes.CollideWall((int)this.Position.X, (int)this.Position.Y);
        }
    }
}
