/* LASER BIKES
 * 
 * Inspired by game created in 3D Buzz XNA Extreme 101 class
 * 
 * Special thanks to the people at Microsoft for the particle system engine
 * 
 * By: Ernest Huntley
 */

// TODO: Increase game resolution
//          ^--- must be divisible by 8 in both directions to work with current grid system
// TODO: Implement a braking system without the 360 controller

using System;
using LaserBikes.Classes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LaserBikes
{
    enum GameState
    {
        Menu,
        Playing,
        Collision,
        Paused
    }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class LaserBikes : Game
    {
        enum MenuOption
        {
            Start,
            Quit
        }

        GraphicsDeviceManager graphics;
        SpriteFont spriteFont;

        // the particle system needs a SpriteBatch to draw particles
        public SpriteBatch SpriteBatch { get; private set; }

        public static Vector2 CollisionLocation = Vector2.Zero;
        private ExplosionParticleSystem explosion;
        private const float EXPLOSION_TIME = 2f;
        private float timeSinceCollision = 0f;

        // a random number generator that the whole game can share.
        private static Random random = new Random();
        public static Random Random
        {
            get { return random; }
        }

        public const int SCREEN_WIDTH = 640;
        public const int SCREEN_HEIGHT = 480;
        public const int GRID_BLOCK_SIZE = 8;
        public const int GRID_WIDTH = SCREEN_WIDTH / GRID_BLOCK_SIZE;
        public const int GRID_HEIGHT = SCREEN_HEIGHT / GRID_BLOCK_SIZE;
        private const int NUM_MENU_OPTIONS = 2;     // number of options in the main menu
        private const int MENU_OPTION_SPACING = 30; // vertical space between menu choices

        public static double BikeMoveInterval = 0.05d;
        public static double BikeStopThreshold = 0.2d;
        public static Vector2 BikeOrigin = new Vector2(6f, 18f);

        public static Texture2D BackgroundTexture;
        public static Texture2D BikeTexture;
        public static Texture2D TailTexture;
        public static Texture2D[] WallTextures;
        public static Color[] PlayerColors = new Color[] { Color.Green,
                                                           Color.Red };

        private static GameState gameState;
        private static MenuOption menuOption;
        private KeyboardState lastKeyboardState;

        private Bike player1;
        private Bike player2;

        public LaserBikes()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            Content.RootDirectory = "Content";

            explosion = new ExplosionParticleSystem(this, 1);
            Components.Add(explosion);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            SpriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            CreateScene();
            menuOption = MenuOption.Start;
            gameState = GameState.Menu;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            BackgroundTexture = Content.Load<Texture2D>(@"Textures\background");
            BikeTexture = Content.Load<Texture2D>(@"Textures\bike");
            TailTexture = Content.Load<Texture2D>(@"Textures\tail");

            WallTextures = new Texture2D[6];
            WallTextures[0] = Content.Load<Texture2D>(@"Textures\wall_h");
            WallTextures[1] = Content.Load<Texture2D>(@"Textures\wall_v");
            WallTextures[2] = Content.Load<Texture2D>(@"Textures\wall_TopLeft");
            WallTextures[3] = Content.Load<Texture2D>(@"Textures\wall_TopRight");
            WallTextures[4] = Content.Load<Texture2D>(@"Textures\wall_BottomRight");
            WallTextures[5] = Content.Load<Texture2D>(@"Textures\wall_BottomLeft");

            spriteFont = Content.Load<SpriteFont>("Font");
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
            KeyboardState currentKeyboardState = Keyboard.GetState();
            GamePadState currentGamepadState = GamePad.GetState(PlayerIndex.One);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || currentKeyboardState.IsKeyDown(Keys.Escape))
                Exit();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;// difference in elapsed game time since last call

            switch (gameState)
            {
                case GameState.Menu:
                    currentKeyboardState = Keyboard.GetState();

                    if (currentKeyboardState.IsKeyDown(Keys.Enter))
                    {
                        if (menuOption == MenuOption.Quit)
                            this.Exit();
                        else if (menuOption == MenuOption.Start)
                            gameState = GameState.Playing;
                    }
                    // move to the next option
                    // doing modulus by the number of options lets us wrap back around to the first option
                    // this also allows us to easily add / remove options later
                    else if (currentKeyboardState.IsKeyDown(Keys.Up) &&
                              lastKeyboardState.IsKeyUp(Keys.Up))
                    {
                        // up takes the absolute value in case it causes menuOption to go negative
                        menuOption = (MenuOption)Math.Abs((int)(menuOption - 1) % NUM_MENU_OPTIONS);
                    }
                    else if (currentKeyboardState.IsKeyDown(Keys.Down) &&
                             lastKeyboardState.IsKeyUp(Keys.Down))
                    {
                        menuOption = (MenuOption)((int)(menuOption + 1) % NUM_MENU_OPTIONS);
                    }

                    break;

                case GameState.Playing:
                    //gamePadState = GamePad.GetState(bike.PlayerIndex);
                    currentKeyboardState = Keyboard.GetState();

                    for (int i = Actor.Actors.Count - 1; i >= 0; i--)
                    {
                        Actor actor = Actor.Actors[i];
                        actor.Update(gameTime);

                        Bike bike = actor as Bike;
                        if (bike != null)
                        {
                            if (currentKeyboardState.IsKeyDown(GetKey(bike.PlayerIndex, Direction.Up)))
                                bike.ChangeDirection(Direction.Up);
                            else if (currentKeyboardState.IsKeyDown(GetKey(bike.PlayerIndex, Direction.Down)))
                                bike.ChangeDirection(Direction.Down);
                            else if (currentKeyboardState.IsKeyDown(GetKey(bike.PlayerIndex, Direction.Left)))
                                bike.ChangeDirection(Direction.Left);
                            else if (currentKeyboardState.IsKeyDown(GetKey(bike.PlayerIndex, Direction.Right)))
                                bike.ChangeDirection(Direction.Right);

                            //double speedRange = BikeStopThreshold - BikeMoveInterval;
                            //bike.MoveInterval = gamePadState.Triggers.Right * speedRange + BikeMoveInterval;
                        }
                    }

                    if (currentKeyboardState.IsKeyDown(Keys.P) &&
                        lastKeyboardState.IsKeyUp(Keys.P))
                        gameState = GameState.Paused;

                    break;

                case GameState.Collision:
                    currentKeyboardState = Keyboard.GetState();
                    timeSinceCollision += dt;

                    if (timeSinceCollision < EXPLOSION_TIME)
                        UpdateExplosions(dt);

                    // ENTER restarts the game
                    if (currentKeyboardState.IsKeyDown(Keys.Enter))
                    {
                        timeSinceCollision = 0f;
                        CreateScene();
                        gameState = GameState.Playing;
                    }
                    break;

                case GameState.Paused:
                    currentKeyboardState = Keyboard.GetState();

                    if (currentKeyboardState.IsKeyDown(Keys.P) &&
                        lastKeyboardState.IsKeyUp(Keys.P))
                        gameState = GameState.Playing;

                    break;
            }

            currentGamepadState = GamePad.GetState(PlayerIndex.One);
            lastKeyboardState = currentKeyboardState;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            switch (gameState)
            {
                case GameState.Menu:
                    Color startColor = Color.White;
                    Color quitColor = Color.White;

                    // highlight the selected menu option
                    if (menuOption == MenuOption.Start)
                        startColor = Color.Red;
                    else if (menuOption == MenuOption.Quit)
                        quitColor = Color.Red;

                    String controlsString = "CONTROLS:\n" +
                                            "Player 1 (green): Arrow keys\n" +
                                            "Player 2 (red): W, S, A, D\n" +
                                            "Pause: P\n" +
                                            "Quit: Esc";

                    SpriteBatch.Begin();
                    SpriteBatch.Draw(BackgroundTexture, Vector2.Zero, new Color(new Vector4(255f, 255f, 255f, 0.35f)));
                    SpriteBatch.DrawString(spriteFont, "Laser Bikes", new Vector2(10, 60), Color.White);
                    SpriteBatch.DrawString(spriteFont, "Start", new Vector2(10, SCREEN_HEIGHT / 2), startColor);
                    SpriteBatch.DrawString(spriteFont, "Quit", new Vector2(10, SCREEN_HEIGHT / 2 + MENU_OPTION_SPACING), quitColor);
                    SpriteBatch.DrawString(spriteFont, controlsString, new Vector2(SCREEN_WIDTH - 225, SCREEN_HEIGHT - 100), Color.White, 0f, Vector2.Zero, 0.75f, SpriteEffects.None, 0f);
                    SpriteBatch.DrawString(spriteFont, "By: E. Huntley - 2009", new Vector2(10, SCREEN_HEIGHT - 30), Color.White, 0f, Vector2.Zero, 0.6f, SpriteEffects.None, 0f);
                    SpriteBatch.End();
                    break;

                case GameState.Playing:
                case GameState.Collision:
                case GameState.Paused:
                    SpriteBatch.Begin();
                    SpriteBatch.Draw(BackgroundTexture, Vector2.Zero, Color.White);

                    Wall.Draw(SpriteBatch);

                    foreach (Actor actor in Actor.Actors)
                    {
                        Bike bike = actor as Bike;
                        if (bike != null)
                        {
                            if (!bike.IsAlive)
                                continue;
                        }
                        actor.Draw(SpriteBatch);
                    }
                    SpriteBatch.End();
                    break;
            }

            base.Draw(gameTime);
        }

        private void CreateScene()
        {
            Actor.Actors.Clear();
            Wall.Reset();

            player1 = new Bike(PlayerIndex.One, new Vector2(-1f, 10f), Direction.Right);    // x value is started negative because position is updated before first draw
            player2 = new Bike(PlayerIndex.Two, new Vector2(GRID_WIDTH, GRID_HEIGHT - 10), Direction.Left);
        }

        public static void CollideWall(int x, int y)
        {
            CollisionLocation.X = x;
            CollisionLocation.Y = y;
            gameState = GameState.Collision;
        }

        #region Helper Functions

        // Since we have multiple players, different directions are assigned to different keys depending
        // on which player is active
        private static Keys GetKey(PlayerIndex playerIndex, Vector2 direction)
        {
            Keys key = Keys.Escape;
            switch (playerIndex)
            {
                case PlayerIndex.One:
                    if (direction == Direction.Up)
                        key = Keys.Up;
                    else if (direction == Direction.Down)
                        key = Keys.Down;
                    else if (direction == Direction.Left)
                        key = Keys.Left;
                    else if (direction == Direction.Right)
                        key = Keys.Right;
                    break;

                case PlayerIndex.Two:
                    if (direction == Direction.Up)
                        key = Keys.W;
                    else if (direction == Direction.Down)
                        key = Keys.S;
                    else if (direction == Direction.Left)
                        key = Keys.A;
                    else if (direction == Direction.Right)
                        key = Keys.D;
                    break;

                case PlayerIndex.Three:
                    break;

                case PlayerIndex.Four:
                    break;
            }

            return key;
        }

        // this function is called when we want to demo the explosion effect. it
        // updates the timeTillExplosion timer, and starts another explosion effect
        // when the timer reaches zero.
        private void UpdateExplosions(float dt)
        {
            Vector2 where = Vector2.Zero;
            // create the explosion at the location of the bike.
            where.X = CollisionLocation.X * GRID_BLOCK_SIZE;
            where.Y = CollisionLocation.Y * GRID_BLOCK_SIZE;

            explosion.AddParticles(where);
        }

        //  a handy little function that gives a random float between two
        // values. This will be used in several places in the sample, in particilar in
        // ParticleSystem.InitializeParticle.
        public static float RandomBetween(float min, float max)
        {
            return min + (float)random.NextDouble() * (max - min);
        }

        #endregion
    }
}
