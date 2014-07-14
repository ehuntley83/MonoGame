/* IBlock.cs
 * 
 * Defines and implements an "I" shaped block
 * 
 * (c) 2014 E. Huntley
 * Code is free to reuse and redistribute
 */

using Microsoft.Xna.Framework;

namespace MonoTetris.Classes.Blocks
{
    class IBlock : Block
    {
        public IBlock()
        {
            this.shape = BlockStates.IBlock1;
            this.color = Color.Red;
            this.rotation = 1;
            this.availRotations = 2;
            this.position = new Vector2(Grid.GridBlocksWidth / 2 - 2, 0);
        }

        public override void Rotate()
        {
            this.rotation += 1;

            if (this.rotation > this.availRotations)
                this.rotation = 1;

            if (this.rotation == 1)
                this.shape = BlockStates.IBlock1;
            else if (this.rotation == 2)
                this.shape = BlockStates.IBlock2;
        }
    }
}
