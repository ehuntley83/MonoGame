/* Grid.cs
 * 
 * This class defines and manages the game grid, that is, the area in which the block pieces are
 * contained.
 * 
 * (c) 2014 E. Huntley
 * Code is free to reuse and redistribute
 */

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoTetris.Classes.Blocks;

namespace MonoTetris
{
    // Static because we can only have one grid
    internal static class Grid
    {
        #region Private Fields
        private const int GRID_BLOCKS_X = 15;                                               // Number of horizontal squares
        private const int GRID_BLOCKS_Y = 25;                                               // Number of vertical squares
        private const int GRID_PIXEL_WIDTH = GRID_BLOCKS_X * Block.BLOCK_OFFSET;            // These must be a multiple of Block.BLOCK_OFFSET
        private const int GRID_PIXEL_HEIGHT = GRID_BLOCKS_Y * Block.BLOCK_OFFSET;           //    to fit blocks nicely
        private const int GRID_X_COORD = (int)((MonoTetris.SCREEN_WIDTH / 2) - (GRID_PIXEL_WIDTH / 2));    // Centers the grid rectangle horizontally
        private const int GRID_Y_COORD = (int)((MonoTetris.SCREEN_HEIGHT / 2) - (GRID_PIXEL_HEIGHT / 2));  // Centers the grid rectangle vertically
        private const int NEXT_PIECE_X_COORD = GRID_X_COORD - Block.BLOCK_OFFSET * 7;                       // X screen coordinate for the "Next Piece" square
        private const int NEXT_PIECE_Y_COORD = GRID_Y_COORD + Block.BLOCK_OFFSET;                           // Y screen coordinate for the "Next Piece" square
        private const int NEXT_PIECE_PIXEL_WIDTH = Block.BLOCK_OFFSET * 6;                  // Width (and height, since it's a square) of the "Next Piece" square
        #endregion

        #region Public Fields
        public static int[,] TheGrid;
        public static int EMPTY_SPACE = 0;
        public static int OCCUPIED_SPACE = -1;
        public static bool DebuggingInfo = false;
        #endregion

        #region Public Properties
        /// <summary>
        /// The upper-left corner of the grid in screen coordinates
        /// </summary>
        public static Vector2 GridDrawOffset
        {
            get
            {
                return new Vector2(GRID_X_COORD, GRID_Y_COORD);
            }
        }

        /// <summary>
        /// Number of blocks in the grid's width
        /// </summary>
        public static int GridBlocksWidth
        {
            get
            {
                return GRID_BLOCKS_X;
            }
        }

        /// <summary>
        /// Number of blocks in the grid's height
        /// </summary>
        public static int GridBlocksHeight
        {
            get
            {
                return GRID_BLOCKS_Y;
            }
        }
        #endregion

        /// <summary>
        /// Bow to the all-mighty C'tor!
        /// </summary>
        static Grid()
        {
            TheGrid = new int[GRID_BLOCKS_Y, GRID_BLOCKS_X];
            Initialize();
        }

        /// <summary>
        /// Clears the grid
        /// </summary>
        public static void Initialize()
        {
            for (int i = 0; i < GRID_BLOCKS_Y; i++)
            {
                for (int j = 0; j < GRID_BLOCKS_X; j++)
                {
                    TheGrid[i, j] = EMPTY_SPACE;
                }
            }
        }

        /// <summary>
        /// Draws the grid outline
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch to draw with</param>
        /// <param name="outlineTexture">Texture to use for the grid's outline</param>
        /// <param name="blockTexture">Texure to use for the blocks</param>
        /// <param name="font">Temporary: font for debugging</param>
        public static void Draw(SpriteBatch spriteBatch, Texture2D outlineTexture, Texture2D blockTexture, SpriteFont font)
        {
            Vector2 gridDrawVector = new Vector2();

            // Draws the rectangle that outlines the grid
            // It is actually a one rectangle with another rectangle within it that is one pixel smaller in each dimension giving
            //    the illusion of an outline
            // The first draw is the "outer" rectangle which is drawn slightly offset so that the "inner" rectangle will be the correct size
            //    to fit the block pieces
            spriteBatch.Draw(outlineTexture, new Rectangle(GRID_X_COORD - 1, GRID_Y_COORD - 1, GRID_PIXEL_WIDTH + 2, GRID_PIXEL_HEIGHT + 2), Color.Red);
            spriteBatch.Draw(outlineTexture, new Rectangle(GRID_X_COORD, GRID_Y_COORD, GRID_PIXEL_WIDTH, GRID_PIXEL_HEIGHT), Color.Black);

            // Draws the rectangle that the "Next Piece" is displayed in
            spriteBatch.Draw(outlineTexture, new Rectangle(NEXT_PIECE_X_COORD - 1, NEXT_PIECE_Y_COORD - 1, NEXT_PIECE_PIXEL_WIDTH + 2, NEXT_PIECE_PIXEL_WIDTH + 2), Color.Red);
            spriteBatch.Draw(outlineTexture, new Rectangle(NEXT_PIECE_X_COORD, NEXT_PIECE_Y_COORD, NEXT_PIECE_PIXEL_WIDTH, NEXT_PIECE_PIXEL_WIDTH), Color.Black);

            // Reads the entire grid and draws a square for each block that is filled
            // This process is very similar to the drawing done in Block.cs
            for (int i = 0; i < GRID_BLOCKS_Y; i++)
            {
                for (int j = 0; j < GRID_BLOCKS_X; j++)
                {
                    if (TheGrid[i, j] == OCCUPIED_SPACE)
                    {
                        gridDrawVector.X = j * Block.BLOCK_OFFSET;  // Here is our adjustment into screen coordinates
                        gridDrawVector.Y = i * Block.BLOCK_OFFSET;  // ---'
                        spriteBatch.Draw(blockTexture, gridDrawVector + Grid.GridDrawOffset, Color.White);
                    }
                }
            }

            // Draws the grid array if debugging info has been selected
            if (DebuggingInfo)
            {
                String textGrid = "";
                for (int i = 0; i < GRID_BLOCKS_Y; i++)
                {
                    for (int j = 0; j < GRID_BLOCKS_X; j++)
                    {
                        textGrid += TheGrid[i, j].ToString() + " ";
                    }
                    textGrid += "\n";
                }
                spriteBatch.DrawString(font, textGrid, Vector2.Zero, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
            }
        }

        /// <summary>
        /// Updates the grid elements based upon the position of the current block
        /// </summary>
        /// <param name="block">The current block being played</param>
        public static void Update(Block block)
        {
            int startX = (int)block.Position.X;
            int startY = (int)block.Position.Y;
            int endX = startX + 4;
            int endY = startY + 4;

            for (int i1 = startY, i2 = 0; i1 < endY; i1++, i2++)
            {
                for (int j1 = startX, j2 = 0; j1 < endX; j1++, j2++)
                {
                    // Make sure we don't go beyond the bounds of the Grid array
                    if (i1 >= 0 && i1 < GRID_BLOCKS_Y && j1 >= 0 && j1 < GRID_BLOCKS_X)
                        // Now make sure we don't reset a square that is already occupied by a set block
                        if (TheGrid[i1, j1] != OCCUPIED_SPACE)
                            TheGrid[i1, j1] = block.Shape[i2, j2] + 1;
                }
            }

            // This allows us to "zero out" the spaces that were previously occupied by the block, but aren't any longer
            // It is why we set the grid value to 2 (block.Shape[,] + 1) in the previous loop
            for (int i = 0; i < GRID_BLOCKS_Y; i++)
            {
                for (int j = 0; j < GRID_BLOCKS_X; j++)
                {
                    if (TheGrid[i, j] > 0)
                        TheGrid[i, j] -= 1;
                }
            }

            // Check for a game-over state
            if (block.Position.Y <= 0)
                if (GameOver(block))
                    MonoTetris.State = GameState.GameOver;

            CheckForFullRows();
        }

        /// <summary>
        /// Places the selected block permanently into the Grid
        /// </summary>
        /// <param name="block">The block to set</param>
        public static void SetBlock(Block block)
        {
            int startX = (int)block.Position.X;
            int startY = (int)block.Position.Y;
            int endX = startX + 4;
            int endY = startY + 4;

            for (int i1 = startY, i2 = 0; i1 < endY; i1++, i2++)
            {
                for (int j1 = startX, j2 = 0; j1 < endX; j1++, j2++)
                {
                    // Make sure we don't go beyond the bounds of the Grid array
                    if (i1 >= 0 && i1 < GRID_BLOCKS_Y && j1 >= 0 && j1 < GRID_BLOCKS_X)
                        if (block.Shape[i2, j2] == 1)
                            TheGrid[i1, j1] = OCCUPIED_SPACE;
                }
            }
        }

        /// <summary>
        /// Checks for and eliminates fully occupied rows
        /// </summary>
        private static void CheckForFullRows()
        {
            // This is a simple check of the total numeric value of each row
            // Since -1 denotes an occupied space, if the row total = -GRID_BLOCKS_X, we have a full row
            int rowTotal = 0;
            // Create a temporary grid which we will then copy into our grid
            int[,] tempGrid = new int[GRID_BLOCKS_Y, GRID_BLOCKS_X];

            for (int i = 0; i < GRID_BLOCKS_Y; i++)
            {
                for (int j = 0; j < GRID_BLOCKS_X; j++)
                {
                    rowTotal += TheGrid[i, j];
                }

                // Do we have a full row?
                if (rowTotal == -GRID_BLOCKS_X)
                {
                    // Cycle through grid spaces and eliminate full ones in this row by copying the cells from
                    //    immediately above each block position down one row
                    for (int y = 0; y <= i; y++)
                    {
                        for (int x = 0; x < GRID_BLOCKS_X; x++)
                        {
                            // If this is the first row, zero it out
                            if (y == 0)
                            {
                                tempGrid[y, x] = EMPTY_SPACE;
                            }
                            else
                            {
                                tempGrid[y, x] = TheGrid[(y - 1), x];
                            }
                        }
                    }
                    // Copy the remaining rows (if any, below the eliminated line)
                    for (int y = i + 1; y < GRID_BLOCKS_Y; y++)
                    {
                        for (int x = 0; x < GRID_BLOCKS_X; x++)
                        {
                            tempGrid[y, x] = TheGrid[y, x];
                        }
                    }

                    TheGrid = tempGrid;
                    MonoTetris.PlayerScore += 100;
                    return;
                }

                // Reset each time through
                rowTotal = 0;
            }
        }

        /// <summary>
        /// Checks if we have reached a game-over state
        /// </summary>
        /// <param name="block">The active block</param>
        /// <returns></returns>
        private static bool GameOver(Block block)
        {
            // To check for game-over, we check each square of the block. If it's at it's top position (which is when
            //    this method is called) and intersects an occupied space, we have failed
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (block.Shape[j, i] == 1)
                    {
                        if ((int)block.Position.Y + j > 0 && (int)block.Position.X + i > 0 && (int)block.Position.X + i < Grid.GridBlocksWidth)
                        {
                            if (TheGrid[(int)block.Position.Y + j, (int)block.Position.X + i] == OCCUPIED_SPACE)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}
