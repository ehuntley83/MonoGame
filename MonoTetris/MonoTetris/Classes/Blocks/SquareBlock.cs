/* SquareBlock.cs
 * 
 * Defines and implements a square shaped block
 * 
 * (c) 2014 E. Huntley
 * Code is free to reuse and redistribute
 */

using Microsoft.Xna.Framework;

namespace MonoTetris.Classes.Blocks
{
    class SquareBlock : Block
    {
        public SquareBlock()
        {
            this.shape = BlockStates.SquareBlock;
            this.color = Color.SlateBlue;
            this.rotation = 1;
            this.availRotations = 1;
            this.position = new Vector2(Grid.GridBlocksWidth / 2 - 1, -1);
        }

        public override void Rotate()
        {
            // This block doesn't really rotate, does it?
            // So we'll just return
            return;
        }
    }
}
