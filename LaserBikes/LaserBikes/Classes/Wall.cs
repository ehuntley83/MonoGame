/* LASER BIKES
 * 
 * Inspired by game created in 3D Buzz XNA Extreme 101 class
 * 
 * By: Ernest Huntley
 */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LaserBikes.Classes
{
    struct WallSegment
    {
        public bool Filled;
        public byte TextureIndex;
        public PlayerIndex PlayerIndex;
    }

    // Declared as static because we are not creating multiple Wall objects throughout the game;
    // this is merely management class for the wall segments placed as the bikes move
    static class Wall
    {
        public static WallSegment[,] Segments;

        public static void Reset()
        {
            Segments = new WallSegment[LaserBikes.GRID_WIDTH, LaserBikes.GRID_HEIGHT];
        }

        public static Rectangle GetPointBounds(int x, int y)
        {
            return new Rectangle(x * LaserBikes.GRID_BLOCK_SIZE,
                                 y * LaserBikes.GRID_BLOCK_SIZE,
                                 LaserBikes.GRID_BLOCK_SIZE,
                                 LaserBikes.GRID_BLOCK_SIZE);
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            for (int x = 0; x < LaserBikes.GRID_WIDTH; x++)
            {
                for (int y = 0; y < LaserBikes.GRID_HEIGHT; y++)
                {
                    if (Segments[x, y].Filled)
                    {
                        Texture2D wallTexture = LaserBikes.WallTextures[Segments[x, y].TextureIndex];
                        Color wallColor = LaserBikes.PlayerColors[(int)Segments[x, y].PlayerIndex];

                        spriteBatch.Draw(wallTexture, GetPointBounds(x, y), wallColor);
                    }
                }
            }
        }
    }
}
