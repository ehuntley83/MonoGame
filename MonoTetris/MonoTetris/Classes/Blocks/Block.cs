/* Block.cs
 * 
 * Abstract class from which the individual blocks are derived.
 * 
 * Each block has a shape which is at one of many states (defined and initialized from BlockStates.cs) and a unique color
 * The Update() and Draw() methods are common amongst the blocks
 * 
 * (c) 2014 E. Huntley
 * Code is free to reuse and redistribute
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoTetris.Classes.Blocks
{
    abstract class Block
    {
        protected int[,] shape;
        protected Color color;
        protected int rotation;                             // Which availabe rotation to use
        protected int availRotations;                       // Number of available rotations
        protected Vector2 position;                         // This is in grid space, not screen coordinates
        
        private KeyboardState lastKeyboardState;            // These will be used to make rotation nicer
        private GamePadState lastGamePadState;

        internal static double STARTING_DROP_SPEED = 0.70;        // The initial time interval (in seconds) that the block uses to move down the grid
        internal static double currentDropSpeed = STARTING_DROP_SPEED;  // This value will change throughout the game as player score increases
        private double dropTimeRemaining = STARTING_DROP_SPEED; // Time remaining until next downward movement
        private const double MAX_DROP_SPEED = 0.05;             // The maximum speed that the drop speed can reach
        private const double LEFT_RIGHT_SPEED = 0.10;           // The time interval (in seconds) used to manage holding the left/right keys down
        private double moveTimeRemaining = LEFT_RIGHT_SPEED;    // Time remaining until next left/right move if key is held

        public const int BLOCK_OFFSET = 16;                 // Distance between block squares

        #region Public Properties

        public Vector2 Position
        {
            get { return this.position; }
            set
            {
                this.position = value;
            }
        }

        public int[,] Shape
        {
            get { return this.shape; }
        }

        #endregion

        /// <summary>
        /// Draws the block
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch to draw with</param>
        /// <param name="texture">Texture to use</param>
        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            Color color;
            Vector2 drawVector = new Vector2();             // Considering the block's position is in grid spaces, we will adjust into screen coordinates

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (shape[j, i] == 0)                   // If the shape contains a zero here, make sure it's drawn transparent
                        color = new Color(0f, 0f, 0f, 0f);
                    else
                        color = this.color;

                    drawVector.X = this.position.X * BLOCK_OFFSET;  // Here is our adjustment into screen coordinates
                    drawVector.Y = this.position.Y * BLOCK_OFFSET;  // ---'
                    // The drawVector is adjusted additional BLOCK_OFFSETs in X and Y for each square in the block piece
                    spriteBatch.Draw(texture, drawVector + Grid.GridDrawOffset + new Vector2((float)i * BLOCK_OFFSET, (float)j * BLOCK_OFFSET), color);
                }
            }
        }

        /// <summary>
        /// Manages the movement and rotation of the blocks
        /// </summary>
        /// <param name="gameTime">GameTime object</param>
        public void Update(GameTime gameTime)
        {
            KeyboardState currentKeyboardState = Keyboard.GetState();
            GamePadState currentGamePadState = GamePad.GetState(PlayerIndex.One);

            // First we adjust the timeRemaining since the last Update call
            this.dropTimeRemaining -= gameTime.ElapsedGameTime.TotalSeconds;
            // If enough time has passed, move the block down and reset timeRemaining
            if (this.dropTimeRemaining <= 0)
            {
                this.position.Y += 1;
                this.dropTimeRemaining = currentDropSpeed;
                if (CheckBlockMovementVertical())
                {
                    MonoTetris.TriggerNewBlock = true;
                }
            }

            this.moveTimeRemaining -= gameTime.ElapsedGameTime.TotalSeconds;
            if ((currentKeyboardState.IsKeyDown(Keys.Left) ||
                currentGamePadState.IsButtonDown(Buttons.DPadLeft))
                && this.moveTimeRemaining <= 0)
            {
                this.position.X -= 1;
                this.moveTimeRemaining = LEFT_RIGHT_SPEED;
                CheckBlockMovementLeft();
            }

            if ((currentKeyboardState.IsKeyDown(Keys.Right) ||
                currentGamePadState.IsButtonDown(Buttons.DPadRight))
                && this.moveTimeRemaining <= 0)
            {
                this.position.X += 1;
                this.moveTimeRemaining = LEFT_RIGHT_SPEED;
                CheckBlockMovementRight();
            }

            if (currentKeyboardState.IsKeyDown(Keys.Down) ||
                currentGamePadState.IsButtonDown(Buttons.DPadDown))
            {
                this.position.Y += 1;
                CheckBlockMovementVertical();
            }

            if ((currentKeyboardState.IsKeyDown(Keys.Up) && lastKeyboardState.IsKeyUp(Keys.Up)) ||
                (currentGamePadState.IsButtonDown(Buttons.A) && lastGamePadState.IsButtonUp(Buttons.A)))
            {
                this.Rotate();
                // After rotating, we want to make sure we haven't moved into an invalid space
                CheckBlockMovementVertical();
                CheckBlockMovementLeft();
                CheckBlockMovementRight();
            }

            // Lastly, adjust the currentDropSpeed for every 30 secods of game time
            if ((currentDropSpeed >= MAX_DROP_SPEED) &&
                ((int)gameTime.TotalGameTime.TotalMilliseconds % 30000 == 0))
            {
                currentDropSpeed -= 0.05;
            }

            lastKeyboardState = currentKeyboardState;
            lastGamePadState = currentGamePadState;
        }

        /// <summary>
        /// Rotates the block
        /// </summary>
        public abstract void Rotate();

        /// <summary>
        /// Prevents the block from going off the left side of the grid
        /// </summary>
        /// <returns>True if a collision was found</returns>
        private bool CheckBlockMovementLeft()
        {
            // Prevent the block from going too far left
            // Begin checking from left-most position
            // As soon as one square is found that has gone too far, reset the position and return
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (shape[j, i] == 1)
                    {
                        // This first check is for block-on-block collisions
                        if ((int)this.position.Y + j > 0 && (int)this.position.Y + j < Grid.GridBlocksHeight &&
                            (int)this.position.X + i >= 0 && (int)this.position.X + i < Grid.GridBlocksWidth)
                        {
                            if (Grid.TheGrid[(int)this.position.Y + j, (int)this.position.X + i] == Grid.OCCUPIED_SPACE)
                            {
                                this.position.X += 1;
                                return true;
                            }
                        }
                        // This second check prevents blocks from going off the left side of the screen
                        if (this.position.X + i < 0)
                        {
                            this.position.X = 0 - i;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Prevents the block from going off the right side of the grid
        /// </summary>
        /// <returns>True if a collision was found</returns>
        private bool CheckBlockMovementRight()
        {
            // Prevent the block from going too far right
            // Begin checking from right-most position
            // As soon as one square is found that has gone too far, reset the position and return
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (shape[j, i] == 1)
                    {
                        // This first check is for block-on-block collisions
                        if ((int)this.position.Y + j > 0 && (int)this.position.Y + j < Grid.GridBlocksHeight &&
                            (int)this.position.X + i > 0 && (int)this.position.X + i < Grid.GridBlocksWidth)
                        {
                            if (Grid.TheGrid[(int)this.position.Y + j, (int)this.position.X + i] == Grid.OCCUPIED_SPACE)
                            {
                                this.position.X -= 1;
                                return true;
                            }
                        }
                        // This second check prevents blocks from going off the right side of the screen
                        if (this.position.X + i > Grid.GridBlocksWidth - 1)
                        {
                            this.position.X = Grid.GridBlocksWidth - i - 1;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks to see if the block hits the bottom of the grid or rests on another filled square
        /// </summary>
        /// <returns>True if a collision was detected</returns>
        private bool CheckBlockMovementVertical()
        {
            // As soon as one square is found that has gone off the bottom of the grid space or run into an occupied space,
            //   fix the position and generate a new block
            // For this check, we begin checking from the bottom row of the block
            for (int i = 0; i < 4; i++)
            {
                for (int j = 3; j >= 0; j--)
                {
                    if (shape[j, i] == 1)
                    {
                        // This first check is for block-on-block collisions
                        if ((int)this.position.Y + j > 0 && (int)this.position.Y + j < Grid.GridBlocksHeight &&
                            (int)this.position.X + i >= 0 && (int)this.position.X + i < Grid.GridBlocksWidth)
                        {
                            if (Grid.TheGrid[(int)this.position.Y + j, (int)this.position.X + i] == Grid.OCCUPIED_SPACE)
                            {
                                this.position.Y -= 1;
                                return true;
                            }
                        }
                        // This second check prevents blocks from going off the bottom of the screen
                        if (this.position.Y + j > Grid.GridBlocksHeight - 1)
                        {
                            this.position.Y = Grid.GridBlocksHeight - j - 1;
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
