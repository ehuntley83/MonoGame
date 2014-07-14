/* MonoTetris.cs
 * 
 * Main game class that drives the functionality of the game
 * 
 * (c) 2014 E. Huntley
 * Code is free to reuse and redistribute
 * 
 * TETRIS WAS CREATED BY Alexey Pajitnov! THANKS TO HIM FOR INSPIRATION!
 * I CLAIM NO CREATIVE CREDIT FOR THIS GAME OR ITS BASE GAMEPLAY DESIGN,
 * ONLY THE CODING AND TECHNICAL IMPLEMENTATION.
 */

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoTetris.Classes.Blocks;

namespace MonoTetris
{
    // Game states
    public enum GameState
    {
        Begin,
        Playing,
        Paused,
        GameOver
    }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MonoTetris : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont arialFont;
        Texture2D texture;
        Texture2D pixel;

        public const int SCREEN_WIDTH = 800;
        public const int SCREEN_HEIGHT = 600;
        public static bool TriggerNewBlock = false;
        public static GameState State;
        public static long PlayerScore;

        Random random;
        KeyboardState lastKeyboardState;
        GamePadState lastGamePadState;

        Block curBlock;
        Block nextBlock;
        Vector2 nextStartingPosition;
        readonly Vector2 nextBlockDrawVector = new Vector2(-6f, 2f);

        public MonoTetris() : base()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = SCREEN_WIDTH,
                PreferredBackBufferHeight = SCREEN_HEIGHT
            };
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            random = new Random();
            State = GameState.Begin;
            PlayerScore = 0;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            texture = Content.Load<Texture2D>(@"Textures\block");
            pixel = Content.Load<Texture2D>(@"Textures\pixel");

            arialFont = Content.Load<SpriteFont>("Font");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            // Allows the game to exit
            if (gamePadState.Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            switch (State)
            {
                case GameState.Begin:
                    if ((keyboardState.IsKeyDown(Keys.Enter) && lastKeyboardState.IsKeyUp(Keys.Enter)) ||
                        (gamePadState.IsButtonDown(Buttons.Start) && lastGamePadState.IsButtonUp(Buttons.Start)))
                    {
                        GenerateBlock(ref curBlock);
                        GenerateBlock(ref nextBlock);
                        nextStartingPosition = nextBlock.Position;
                        nextBlock.Position = nextBlockDrawVector;
                        State = GameState.Playing;
                    }
                    break;

                case GameState.Playing:
                    // Allow toggling of Grid debugging info
                    if ((keyboardState.IsKeyDown(Keys.D) && lastKeyboardState.IsKeyUp(Keys.D)) ||
                        (gamePadState.IsButtonDown(Buttons.Y) && lastGamePadState.IsButtonUp(Buttons.Y)))
                        Grid.DebuggingInfo = !Grid.DebuggingInfo;

                    Grid.Update(curBlock);
                    curBlock.Update(gameTime);

                    if (TriggerNewBlock)
                    {
                        Grid.SetBlock(curBlock);

                        curBlock = nextBlock;
                        curBlock.Position = nextStartingPosition;
                        GenerateBlock(ref nextBlock);
                        nextStartingPosition = nextBlock.Position;
                        nextBlock.Position = nextBlockDrawVector;

                        TriggerNewBlock = false;
                    }

                    // Allow for game pausing
                    if ((keyboardState.IsKeyDown(Keys.P) && lastKeyboardState.IsKeyUp(Keys.P)) ||
                        (gamePadState.IsButtonDown(Buttons.Start) && lastGamePadState.IsButtonUp(Buttons.Start)))
                        State = GameState.Paused;
                    break;

                case GameState.Paused:
                    if ((keyboardState.IsKeyDown(Keys.P) && lastKeyboardState.IsKeyUp(Keys.P)) ||
                        (gamePadState.IsButtonDown(Buttons.Start) && lastGamePadState.IsButtonUp(Buttons.Start)))
                        State = GameState.Playing;
                    break;

                case GameState.GameOver:
                    if (keyboardState.IsKeyDown(Keys.Enter) ||
                        gamePadState.IsButtonDown(Buttons.Start))
                    {
                        Grid.Initialize();
                        GenerateBlock(ref curBlock);
                        GenerateBlock(ref nextBlock);
                        nextStartingPosition = nextBlock.Position;
                        nextBlock.Position = nextBlockDrawVector;
                        PlayerScore = 0;
                        Block.currentDropSpeed = Block.STARTING_DROP_SPEED;
                        State = GameState.Playing;
                    }
                    break;
            }

            base.Update(gameTime);

            lastKeyboardState = Keyboard.GetState();
            lastGamePadState = GamePad.GetState(PlayerIndex.One);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DimGray);

            spriteBatch.Begin();

            switch (State)
            {
                // Technically, these could be placed within their own case blocks, but the only difference between the two is the Game Over message, so we
                //    will keep them within the same case block with a small conditional
                case GameState.Begin:
                case GameState.GameOver:
                    Grid.Draw(spriteBatch, pixel, texture, arialFont);
                    DrawTitleScoreAndControls();
                    // Draw the text twice to cause a drop-shadow effect
                    spriteBatch.DrawString(arialFont, "Press Enter to Begin", new Vector2(310, 299), Color.Black, 0f, Vector2.Zero, 0.85f, SpriteEffects.None, 0f);
                    spriteBatch.DrawString(arialFont, "Press Enter to Begin", new Vector2(311, 300), Color.White, 0f, Vector2.Zero, 0.85f, SpriteEffects.None, 0f);
                    if (State == GameState.GameOver)
                    {
                        // Draw the text twice to cause a drop-shadow effect
                        spriteBatch.DrawString(arialFont, "GAME OVER!", new Vector2(332, 279), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                        spriteBatch.DrawString(arialFont, "GAME OVER!", new Vector2(333, 280), Color.Red, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    }
                    break;

                // We do the same thing with these two states as we do the previous two
                case GameState.Playing:
                case GameState.Paused:
                    // Grid must be drawn BEFORE the block pieces so that they appear on top of it
                    Grid.Draw(spriteBatch, pixel, texture, arialFont);
                    curBlock.Draw(spriteBatch, texture);
                    nextBlock.Draw(spriteBatch, texture);
                    DrawTitleScoreAndControls();
                    if (State == GameState.Paused)
                    {
                        // Draw the text twice to cause a drop-shadow effect
                        spriteBatch.DrawString(arialFont, "Paused", new Vector2(599, 399), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                        spriteBatch.DrawString(arialFont, "Paused", new Vector2(600, 400), Color.Chartreuse, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    }
                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Generates a random block
        /// </summary>
        /// <param name="block">A reference to the Block object that will be affected</param>
        private void GenerateBlock(ref Block block)
        {
            switch (random.Next(0, 7))
            {
                case 0:
                    block = new IBlock();
                    break;
                case 1:
                    block = new LBlock();
                    break;
                case 2:
                    block = new RLBlock();
                    break;
                case 3:
                    block = new SBlock();
                    break;
                case 4:
                    block = new SquareBlock();
                    break;
                case 5:
                    block = new TBlock();
                    break;
                case 6:
                    block = new ZBlock();
                    break;
                default:
                    throw new ArgumentException("Bad block type!");
            }
        }

        /// <summary>
        /// Draws the title, player score and controls text
        /// </summary>
        private void DrawTitleScoreAndControls()
        {
            String title = "XNA Tetris clone";
            String subtitle = "By: Ernie Huntley";
            String controls = "Controls:\n" +
                              "Left / Right Arrow Keys - Move block\n" +
                              "Up Arrow Key - Rotate block\n" +
                              "Down Key - Drop block fast\n" +
                              "P Key - Pause / Unpause\n" +
                              "D Key - Grid debugging info\n" +
                              "Escape Key - Exit game";
            String next = "Next Piece";
            String credit = "(TETRIS originally created by Alexey Pajitnov)";
            String score = "Score:\n" +
                           PlayerScore.ToString();

            // Draw the main title & subtitle twice - Once in black and then in blue. This creates a drop-shadow effect
            spriteBatch.DrawString(arialFont, title, new Vector2(244, 14), Color.Black, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(arialFont, title, new Vector2(245, 15), Color.Blue, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(arialFont, subtitle, new Vector2(324, 54), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(arialFont, subtitle, new Vector2(325, 55), Color.Blue, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(arialFont, controls, new Vector2(530, 100), Color.White, 0f, Vector2.Zero, 0.65f, SpriteEffects.None, 0f);
            // Draw the score twice as well for the drop-shadow effect
            spriteBatch.DrawString(arialFont, score, new Vector2(9, 549), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(arialFont, score, new Vector2(10, 550), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(arialFont, credit, new Vector2(248, 550), Color.Maroon, 0f, Vector2.Zero, 0.7f, SpriteEffects.None, 0f);
            // Draw the next piece prompt
            spriteBatch.DrawString(arialFont, next, new Vector2(168, 98), Color.White, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0f);
        }
    }
}
