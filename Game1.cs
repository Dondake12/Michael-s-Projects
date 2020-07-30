using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RC_Framework;
using System;

namespace Assignment
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static RC_GameStateManager gameStateManager;

        public static string dir = "";
        public static int timer = 0;
        public static Random rnd = new Random();
        public bool showBoundary = false;

        public static KeyboardState keyState;
        public static KeyboardState prevKeyState;

        public static SpriteBatch sb;
        public static GraphicsDevice gd;

        public int score = 0;
        public SpriteFont scoreFont;
        public String status;
        public SpriteFont helpFont;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = 950;
            graphics.PreferredBackBufferWidth = 1400;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

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
            gameStateManager = new RC_GameStateManager();

            // TODO: use this.Content to load your game content here
            LineBatch.init(GraphicsDevice);
            Dir.dir = Util.findDirWithFile(@"Art\Sea04.png");

            gameStateManager = new RC_GameStateManager();

            gameStateManager.AddLevel(0, new GameLevel_0_Default());
            gameStateManager.getLevel(0).InitializeLevel(GraphicsDevice, spriteBatch, Content, gameStateManager);
            gameStateManager.getLevel(0).LoadContent();
            
            gameStateManager.AddLevel(1, new GameLevel_1());
            gameStateManager.getLevel(1).InitializeLevel(GraphicsDevice, spriteBatch, Content, gameStateManager);
            gameStateManager.getLevel(1).LoadContent();

            gameStateManager.AddLevel(2, new GameLevel_2());
            gameStateManager.getLevel(2).InitializeLevel(GraphicsDevice, spriteBatch, Content, gameStateManager);
            gameStateManager.getLevel(2).LoadContent();

            gameStateManager.AddLevel(3, new GameLevel_Pause());
            gameStateManager.getLevel(3).InitializeLevel(GraphicsDevice, spriteBatch, Content, gameStateManager);
            gameStateManager.getLevel(3).LoadContent();

            gameStateManager.AddLevel(4, new GameLevel_End());
            gameStateManager.getLevel(4).InitializeLevel(GraphicsDevice, spriteBatch, Content, gameStateManager);
            gameStateManager.getLevel(4).LoadContent();

            gameStateManager.AddLevel(5, new GameLevel_Instructions());
            gameStateManager.getLevel(5).InitializeLevel(GraphicsDevice, spriteBatch, Content, gameStateManager);
            gameStateManager.getLevel(5).LoadContent();

            gameStateManager.setLevel(2);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            //prevKeyState = keyState;
            //keyState = Keyboard.GetState();

            gameStateManager.getCurrentLevel().Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            gameStateManager.getCurrentLevel().Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
