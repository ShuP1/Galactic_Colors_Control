using Galactic_Colors_Control;
using Galactic_Colors_Control_Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyCommon;
using MyMonoGame.GUI;
using System;
using System.IO;
using System.Reflection;
using System.Xml;

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

        public Background background;

        public MultiLang multilang = new MultiLang();
        public Config config = new Config();
        public Logger logger = new Logger();
        public Client client; //Client Core
        public Manager GUI = new Manager(); //MyMonogameGUI

        private bool isFullScreen = false;

        public States.State gameState = new States.TitleState(new States.MainMenuState(), new TimeSpan(0, 0, 5));

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
            config = XmlManager.Load<Config>(AppDomain.CurrentDomain.BaseDirectory + "Config.xml", XmlManager.LoadMode.ReadCreateOrReplace, XmlReader.Create("ConfigSchema.xsd"), logger);
            config.PostSave();
            logger.Write("Galactic Colors Control GUI " + Assembly.GetEntryAssembly().GetName().Version.ToString(), Logger.logType.fatal);
            logger.Initialise(config.logPath, config.logBackColor, config.logForeColor, config.logLevel, Program._debug, Program._dev, false);
            multilang.Initialise(Common.dictionary);
            if (Program._debug) { logger.Write("CLIENT IS IN DEBUG MODE !", Logger.logType.error, Logger.logConsole.show); }
            if (Program._dev) { logger.Write("CLIENT IS IN DEV MODE !", Logger.logType.error, Logger.logConsole.show); }

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

            GUI.content.Initialise(content, GraphicsDevice);

            //Need OpenAL Update for Windows 10 at least
            GUI.content.AddSound("alert");
            GUI.content.AddSound("bip");
            GUI.content.AddSound("change");
            GUI.content.AddSound("valid");

            GUI.content.AddFont("small");
            GUI.content.AddFont("basic");
            GUI.content.AddFont("title");

            GUI.content.AddTexture("pointer", "Hub/pointer0");

            GUI.content.AddTexture("background0");
            GUI.content.AddTexture("background1");

            GUI.content.AddTexture("logoSmall", "LogoSmall");

            GUI.content.AddBox("Default", "Hub/Buttons/0");

            //Load from files
            if (Directory.Exists("Skin/" + config.skin))
            {
                if (Directory.Exists("Skin/" + config.skin + "/Sounds"))
                {
                    GUI.content.EditSound("alert", MyMonoGame.Utilities.FromFile.SoundFromMp3("Skin/" + config.skin + "/Sounds/alert.mp3"));
                    GUI.content.EditSound("bip", MyMonoGame.Utilities.FromFile.SoundFromMp3("Skin/" + config.skin + "/Sounds/bip.mp3"));
                    GUI.content.EditSound("change", MyMonoGame.Utilities.FromFile.SoundFromMp3("Skin/" + config.skin + "/Sounds/change.mp3"));
                    GUI.content.EditSound("valid", MyMonoGame.Utilities.FromFile.SoundFromMp3("Skin/" + config.skin + "/Sounds/valid.mp3"));
                }

                if (Directory.Exists("Skin/" + config.skin + "/Textures"))
                {
                    GUI.content.EditTexture("background0", MyMonoGame.Utilities.FromFile.SpriteFromPng("Skin/" + config.skin + "Textures/background0.png", GraphicsDevice));
                    GUI.content.EditTexture("background1", MyMonoGame.Utilities.FromFile.SpriteFromPng("Skin/" + config.skin + "Textures/background1.png", GraphicsDevice));
                    if (Directory.Exists("Skin/" + config.skin + "/Textures/Hub/"))
                    {
                        if (Directory.Exists("Skin/" + config.skin + "/Textures/Hub/Buttons"))
                        {
                            GUI.content.EditBox("Default", MyMonoGame.Utilities.FromFile.BoxFormFolder("Skin/" + config.skin + "Textures/Hub/Buttons/0", GraphicsDevice));
                        }

                        GUI.content.EditTexture("pointer", MyMonoGame.Utilities.FromFile.SpriteFromPng("Skin/" + config.skin + "/Textures/Hub/pointer0.png", GraphicsDevice));
                    }
                }
            }

            background = new Background(new Texture2D[2] { GUI.content.GetTexture("background0"), GUI.content.GetTexture("background1") }, new double[2] { 1, 2 }); //Background initialisation
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
            GUI.Update();
            gameState.Update();

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

#if DEBUG
            Color ActiveColor = IsActive ? Color.Green : Color.Red;
            GUI.Label(new Vector(10, ScreenHeight - 20), (1 / (float)gameTime.ElapsedGameTime.TotalSeconds).ToString(), GUI.content.GetFont("small"), new MyMonoGame.Colors(ActiveColor));
#endif

            spriteBatch.Draw(GUI.content.GetTexture("pointer"), new Rectangle(Mouse.GetState().X - 10, Mouse.GetState().Y - 10, 20, 20), Color.Red);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}