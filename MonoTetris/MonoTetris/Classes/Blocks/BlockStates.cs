/* BlockStates.cs
 * 
 * Defines the various states ("rotations") that the various blocks can be in
 * 
 * (c) 2014 E. Huntley
 * Code is free to reuse and redistribute
 */

namespace MonoTetris.Classes.Blocks
{
    static class BlockStates
    {
        /**********************************
         * I BLOCK
         **********************************/
        public static int[,] IBlock1 =
            new int[4, 4] {{0,0,1,0},
                           {0,0,1,0},
                           {0,0,1,0},
                           {0,0,1,0}};
        public static int[,] IBlock2 =
            new int[4, 4] {{0,0,0,0},
                           {1,1,1,1},
                           {0,0,0,0},
                           {0,0,0,0}};
        /**********************************
         * SQUARE BLOCK
         **********************************/
        public static int[,] SquareBlock =
            new int[4, 4] {{0,0,0,0},
                           {0,1,1,0},
                           {0,1,1,0},
                           {0,0,0,0}};
        /**********************************
         * T BLOCK
         **********************************/
        public static int[,] TBlock1 =
            new int[4, 4] {{0,0,0,0},
                           {0,0,1,0},
                           {0,1,1,1},
                           {0,0,0,0}};
        public static int[,] TBlock2 =
            new int[4, 4] {{0,0,0,0},
                           {0,0,1,0},
                           {0,0,1,1},
                           {0,0,1,0}};
        public static int[,] TBlock3 =
            new int[4, 4] {{0,0,0,0},
                           {0,0,0,0},
                           {0,1,1,1},
                           {0,0,1,0}};
        public static int[,] TBlock4 =
            new int[4, 4] {{0,0,0,0},
                           {0,0,1,0},
                           {0,1,1,0},
                           {0,0,1,0}};
        /**********************************
         * S BLOCK
         **********************************/
        public static int[,] SBlock1 =
            new int[4, 4] {{0,0,0,0},
                           {0,0,1,0},
                           {0,1,1,0},
                           {0,1,0,0}};
        public static int[,] SBlock2 =
            new int[4, 4] {{0,0,0,0},
                           {0,1,1,0},
                           {0,0,1,1},
                           {0,0,0,0}};
        /**********************************
         * Z BLOCK
         **********************************/
        public static int[,] ZBlock1 =
            new int[4, 4] {{0,0,0,0},
                           {0,1,0,0},
                           {0,1,1,0},
                           {0,0,1,0}};
        public static int[,] ZBlock2 =
            new int[4, 4] {{0,0,0,0},
                           {0,0,1,1},
                           {0,1,1,0},
                           {0,0,0,0}};
        /**********************************
         * L BLOCK
         **********************************/
        public static int[,] LBlock1 =
            new int[4, 4] {{0,1,0,0},
                           {0,1,0,0},
                           {0,1,1,0},
                           {0,0,0,0}};
        public static int[,] LBlock2 =
            new int[4, 4] {{0,0,0,0},
                           {1,1,1,0},
                           {1,0,0,0},
                           {0,0,0,0}};
        public static int[,] LBlock3 =
            new int[4, 4] {{1,1,0,0},
                           {0,1,0,0},
                           {0,1,0,0},
                           {0,0,0,0}};
        public static int[,] LBlock4 =
            new int[4, 4] {{0,0,1,0},
                           {1,1,1,0},
                           {0,0,0,0},
                           {0,0,0,0}};
        /**********************************
         * Reversed L BLOCK
         **********************************/
        public static int[,] RLBlock1 =
            new int[4, 4] {{0,1,0,0},
                           {0,1,0,0},
                           {1,1,0,0},
                           {0,0,0,0}};
        public static int[,] RLBlock2 =
            new int[4, 4] {{1,0,0,0},
                           {1,1,1,0},
                           {0,0,0,0},
                           {0,0,0,0}};
        public static int[,] RLBlock3 =
            new int[4, 4] {{0,1,1,0},
                           {0,1,0,0},
                           {0,1,0,0},
                           {0,0,0,0}};
        public static int[,] RLBlock4 =
            new int[4, 4] {{0,0,0,0},
                           {1,1,1,0},
                           {0,0,1,0},
                           {0,0,0,0}};
    }
}
