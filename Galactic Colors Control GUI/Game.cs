using Galactic_Colors_Control;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyMonoGame.GUI;
using System;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Galactic_Colors_Control_GUI
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        public static Game singleton;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private ContentManager content;

        private SoundEffect[] effects = new SoundEffect[4]; //click, hover, explosion,...

        public Fonts fonts;

        internal static Texture2D nullSprite;

        private Texture2D[] pointerSprites = new Texture2D[1];
        public boxSprites[] buttonsSprites = new boxSprites[1];
        public Background background;

        public Client client; //Client Core
        public Manager GUI = new Manager(); //MyMonogameGUI

        private string skinName;
        private bool isFullScreen = false;

        public States.State gameState = new States.TitleState(new States.MainMenuState(), new TimeSpan(0,0,5));

        private int _ScreenWidth = 1280;
        private int _ScreenHeight = 720;

        public int ScreenWidth { get { return _ScreenWidth; } private set { _ScreenWidth = value; } }
        public int ScreenHeight { get { return _ScreenHeight; } private set { _ScreenHeight = value; } }

        public Game()
        {
            singleton = this;
            if (isFullScreen) //Fullscreen resolution
            {
                ScreenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                ScreenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = ScreenWidth;
            graphics.PreferredBackBufferHeight = ScreenHeight;
            graphics.IsFullScreen = isFullScreen;
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
            nullSprite = new Texture2D(GraphicsDevice, 1, 1);
            nullSprite.SetData(new Color[1 * 1] { Color.White });

            GUI.Initialise();
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

            //Need OpenAL Update for Windows 10 at least
            effects[0] = content.Load<SoundEffect>("Sounds/alert");
            effects[1] = content.Load<SoundEffect>("Sounds/bip");
            effects[2] = content.Load<SoundEffect>("Sounds/change");
            effects[3] = content.Load<SoundEffect>("Sounds/valid");

            fonts.small = content.Load<SpriteFont>("Fonts/small");
            fonts.basic = content.Load<SpriteFont>("Fonts/basic");
            fonts.title = content.Load<SpriteFont>("Fonts/title");

            for (int i = 0; i < pointerSprites.Length; i++)
            {
                pointerSprites[i] = content.Load<Texture2D>("Textures/Hub/pointer" + i);
            }

            Texture2D[] backSprites = new Texture2D[2];
            backSprites[0] = content.Load<Texture2D>("Textures/background0");
            backSprites[1] = content.Load<Texture2D>("Textures/background1");

            States.MainMenuState.logoSprite = content.Load<Texture2D>("Textures/LogoSmall");

            for (int i = 0; i < buttonsSprites.Length; i++)
            {
                buttonsSprites[i].topLeft = content.Load<Texture2D>("Textures/Hub/Buttons/" + i + "/topLeft");
                buttonsSprites[i].topCenter = content.Load<Texture2D>("Textures/Hub/Buttons/" + i + "/topCenter");
                buttonsSprites[i].topRight = content.Load<Texture2D>("Textures/Hub/Buttons/" + i + "/topRight");
                buttonsSprites[i].centerLeft = content.Load<Texture2D>("Textures/Hub/Buttons/" + i + "/centerLeft");
                buttonsSprites[i].centerCenter = content.Load<Texture2D>("Textures/Hub/Buttons/" + i + "/centerCenter");
                buttonsSprites[i].centerRight = content.Load<Texture2D>("Textures/Hub/Buttons/" + i + "/centerRight");
                buttonsSprites[i].bottomLeft = content.Load<Texture2D>("Textures/Hub/Buttons/" + i + "/bottomLeft");
                buttonsSprites[i].bottomCenter = content.Load<Texture2D>("Textures/Hub/Buttons/" + i + "/bottomCenter");
                buttonsSprites[i].bottomRight = content.Load<Texture2D>("Textures/Hub/Buttons/" + i + "/bottomRight");
            }

            //Load from files
            if (Directory.Exists("Skin/" + skinName))
            {
                if (Directory.Exists("Skin/" + skinName + "/Sounds"))
                {
                    Utilities.SoundFromMp3("Skin/" + skinName + "/Sounds/alert.mp3", ref effects[0]);
                    Utilities.SoundFromMp3("Skin/" + skinName + "/Sounds/bip.mp3", ref effects[1]);
                    Utilities.SoundFromMp3("Skin/" + skinName + "/Sounds/change.mp3", ref effects[2]);
                    Utilities.SoundFromMp3("Skin/" + skinName + "/Sounds/valid.mp3", ref effects[3]);
                }

                if (Directory.Exists("Skin/" + skinName + "/Textures"))
                {
                    Utilities.SpriteFromPng("Skin/" + skinName + "Textures/background0.png", ref backSprites[0], GraphicsDevice);
                    Utilities.SpriteFromPng("Skin/" + skinName + "Textures/background1.png", ref backSprites[1], GraphicsDevice);
                    if (Directory.Exists("Skin/" + skinName + "/Textures/Hub/"))
                    {
                        if (Directory.Exists("Skin/" + skinName + "/Textures/Hub/Buttons"))
                        {
                            for (int i = 0; i < buttonsSprites.Length; i++)
                            {
                                Utilities.SpriteFromPng("Skin/" + skinName + "Textures/Hub/Buttons/" + i + "/topLeft.png", ref buttonsSprites[i].topLeft, GraphicsDevice);
                                Utilities.SpriteFromPng("Skin/" + skinName + "Textures/Hub/Buttons/" + i + "/topCenter.png", ref buttonsSprites[i].topCenter, GraphicsDevice);
                                Utilities.SpriteFromPng("Skin/" + skinName + "Textures/Hub/Buttons/" + i + "/topRight.png", ref buttonsSprites[i].topRight, GraphicsDevice);
                                Utilities.SpriteFromPng("Skin/" + skinName + "Textures/Hub/Buttons/" + i + "/centerLeft.png", ref buttonsSprites[i].centerLeft, GraphicsDevice);
                                Utilities.SpriteFromPng("Skin/" + skinName + "Textures/Hub/Buttons/" + i + "/centerCenter.png", ref buttonsSprites[i].centerCenter, GraphicsDevice);
                                Utilities.SpriteFromPng("Skin/" + skinName + "Textures/Hub/Buttons/" + i + "/centerRight.png", ref buttonsSprites[i].centerRight, GraphicsDevice);
                                Utilities.SpriteFromPng("Skin/" + skinName + "Textures/Hub/Buttons/" + i + "/bottomLeft.png", ref buttonsSprites[i].bottomLeft, GraphicsDevice);
                                Utilities.SpriteFromPng("Skin/" + skinName + "Textures/Hub/Buttons/" + i + "/bottomCenter.png", ref buttonsSprites[i].bottomCenter, GraphicsDevice);
                                Utilities.SpriteFromPng("Skin/" + skinName + "Textures/Hub/Buttons/" + i + "/bottomRight.png", ref buttonsSprites[i].bottomRight, GraphicsDevice);
                            }
                        }

                        for (int i = 0; i < pointerSprites.Length; i++)
                        {
                            Utilities.SpriteFromPng("Skin/" + skinName + "/Textures/Hub/pointer" + i + ".png", ref pointerSprites[i], GraphicsDevice);
                        }
                    }
                }
            }

            background = new Background(backSprites, new double[2] { 1, 2 }); //Background initialisation
            background.speedX = 1;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            gameState.Update();
            GUI.Update();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGray);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            GUI.Draw(spriteBatch);
            gameState.Draw(spriteBatch);
           
            Color ActiveColor = IsActive ? Color.Green : Color.Red;
            GUI.Label(new MyMonoGame.Vector(10, ScreenHeight - 20), (1 / (float)gameTime.ElapsedGameTime.TotalSeconds).ToString(), fonts.small, new MyMonoGame.Colors(ActiveColor));
            spriteBatch.Draw(pointerSprites[0], new Rectangle(Mouse.GetState().X - 10, Mouse.GetState().Y - 10, 20, 20), Color.Red);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}