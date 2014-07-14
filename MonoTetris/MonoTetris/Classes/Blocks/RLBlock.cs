/* RLBlock.cs
 * 
 * Defines and implements a reversed "L" shaped block
 * 
 * (c) 2014 E. Huntley
 * Code is free to reuse and redistribute
 */

using Microsoft.Xna.Framework;

namespace MonoTetris.Classes.Blocks
{
    class RLBlock : Block
    {
        public RLBlock()
        {
            this.shape = BlockStates.RLBlock1;
            this.color = Color.Yellow;
            this.rotation = 1;
            this.availRotations = 4;
            this.position = new Vector2(Grid.GridBlocksWidth / 2 - 1, 0);
        }

        public override void Rotate()
        {
            this.rotation += 1;

            if (this.rotation > this.availRotations)
                this.rotation = 1;

            if (this.rotation == 1)
                this.shape = BlockStates.RLBlock1;
            else if (this.rotation == 2)
                this.shape = BlockStates.RLBlock2;
            else if (this.rotation == 3)
                this.shape = BlockStates.RLBlock3;
            else if (this.rotation == 4)
                this.shape = BlockStates.RLBlock4;
        }
    }
}
