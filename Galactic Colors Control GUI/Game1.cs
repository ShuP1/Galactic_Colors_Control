using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Reflection;

namespace Galactic_Colors_Control_GUI
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        ContentManager content;

        private SoundEffect[] effects = new SoundEffect[4];

        private SpriteFont smallFont;
        private SpriteFont basicFont;
        private SpriteFont titleFont;

        private Texture2D nullSprite;
        private Texture2D[] pointerSprites = new Texture2D[1];

        Version version;

        private MouseState oldState;
        private MouseState newState;
        private int mouseX;
        private int mouseY;
        private bool haveOverButton = false;

        private string skinName;

        private enum GameStatus { Home, Options, Game, Pause, End, Thanks }
        private GameStatus gameStatus;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            content = Content;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            version = Assembly.GetEntryAssembly().GetName().Version;
            nullSprite = new Texture2D(GraphicsDevice, 1, 1);
            nullSprite.SetData(new Color[1 * 1] { Color.White });

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

            //effects[0] = content.Load<SoundEffect>("Sounds/alert");
            //effects[1] = content.Load<SoundEffect>("Sounds/bip");
            //effects[2] = content.Load<SoundEffect>("Sounds/change");
            //effects[3] = content.Load<SoundEffect>("Sounds/valid");

            smallFont = content.Load<SpriteFont>("Fonts/small");
            basicFont = content.Load<SpriteFont>("Fonts/basic");
            titleFont = content.Load<SpriteFont>("Fonts/title");

            for (int i = 0; i < pointerSprites.Length; i++) {
                Console.WriteLine("Load pointer" + i);
                pointerSprites[i] = content.Load<Texture2D>("Textures/Hub/pointer" + i);
            }

            if (Directory.Exists("Skin/" + skinName))
            {
                if (File.Exists("Skin/" + skinName + "/Sounds/alert.mp3")) using (FileStream fileStream = new FileStream("Skin/" + skinName + "/Sounds/alert.mp3", FileMode.Open)) { effects[0] = SoundEffect.FromStream(fileStream); }
                if (File.Exists("Skin/" + skinName + "/Sounds/bip.mp3")) using (FileStream fileStream = new FileStream("Skin/" + skinName + "/Sounds/bip.mp3", FileMode.Open)) { effects[1] = SoundEffect.FromStream(fileStream); }
                if (File.Exists("Skin/" + skinName + "/Sounds/change.mp3")) using (FileStream fileStream = new FileStream("Skin/" + skinName + "/Sounds/change.mp3", FileMode.Open)) { effects[2] = SoundEffect.FromStream(fileStream); }
                if (File.Exists("Skin/" + skinName + "/Sounds/valid.mp3")) using (FileStream fileStream = new FileStream("Skin/" + skinName + "/Sounds/valid.mp3", FileMode.Open)) { effects[3] = SoundEffect.FromStream(fileStream); }

                for (int i = 0; i < pointerSprites.Length; i++)
                {
                    if (File.Exists("Skin/" + skinName + "/Textures/Hub/pointer" + i + ".png")) using (FileStream fileStream = new FileStream("Skin/" + skinName + "/Textures/Hub/pointer" + i + ".png", FileMode.Open)) { pointerSprites[i] = Texture2D.FromStream(GraphicsDevice, fileStream); }
                }
            }

            // TODO: use this.Content to load your game content here
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

            mouseX = Mouse.GetState().X;
            mouseY = Mouse.GetState().Y;
            if (IsActive)
            {

            }

            switch (gameStatus)
            {
                case GameStatus.Home:

                    break;

                case GameStatus.Options:

                    break;

                case GameStatus.Game:

                    break;

                case GameStatus.Pause:

                    break;

                case GameStatus.End:

                    break;

                case GameStatus.Thanks:

                    break;
            }
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            switch (gameStatus)
            {
                case GameStatus.Home:

                    break;

                case GameStatus.Options:

                    break;

                case GameStatus.Game:

                    break;

                case GameStatus.Pause:

                    break;

                case GameStatus.End:

                    break;

                case GameStatus.Thanks:

                    break;
            }

            spriteBatch.Draw(pointerSprites[0], new Rectangle(mouseX - 10, mouseY - 10, 20, 20), Color.Red);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
