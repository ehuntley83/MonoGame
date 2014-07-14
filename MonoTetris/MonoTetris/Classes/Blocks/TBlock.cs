/* TBlock.cs
 * 
 * Defines and implements a "T" shaped block
 * 
 * (c) 2014 E. Huntley
 * Code is free to reuse and redistribute
 */

using Microsoft.Xna.Framework;

namespace MonoTetris.Classes.Blocks
{
    class TBlock : Block
    {
        public TBlock()
        {
            this.shape = BlockStates.TBlock1;
            this.color = Color.LightGray;
            this.rotation = 1;
            this.availRotations = 4;
            this.position = new Vector2(Grid.GridBlocksWidth / 2 - 2, -1);
        }

        public override void Rotate()
        {
            this.rotation += 1;

            if (this.rotation > this.availRotations)
                this.rotation = 1;

            if (this.rotation == 1)
                this.shape = BlockStates.TBlock1;
            else if (this.rotation == 2)
                this.shape = BlockStates.TBlock2;
            else if (this.rotation == 3)
                this.shape = BlockStates.TBlock3;
            else if (this.rotation == 4)
                this.shape = BlockStates.TBlock4;
        }
    }
}
