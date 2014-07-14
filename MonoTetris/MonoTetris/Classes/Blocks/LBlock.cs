/* LBlock.cs
 * 
 * Defines and implements a "L" shaped block
 * 
 * (c) 2014 E. Huntley
 * Code is free to reuse and redistribute
 */

using Microsoft.Xna.Framework;

namespace MonoTetris.Classes.Blocks
{
    class LBlock : Block
    {
        public LBlock()
        {
            this.shape = BlockStates.LBlock1;
            this.color = Color.Orange;
            this.rotation = 1;
            this.availRotations = 4;
            this.position = new Vector2(Grid.GridBlocksWidth / 2 - 2, 0);
        }

        public override void Rotate()
        {
            this.rotation += 1;

            if (this.rotation > this.availRotations)
                this.rotation = 1;

            if (this.rotation == 1)
                this.shape = BlockStates.LBlock1;
            else if (this.rotation == 2)
                this.shape = BlockStates.LBlock2;
            else if (this.rotation == 3)
                this.shape = BlockStates.LBlock3;
            else if (this.rotation == 4)
                this.shape = BlockStates.LBlock4;
        }
    }
}
