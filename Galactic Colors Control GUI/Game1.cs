using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        internal static Texture2D nullSprite;
        private Texture2D[] pointerSprites = new Texture2D[1];
        private GUI.boxSprites[] buttonsSprites = new GUI.boxSprites[1];

        private List<GUI.Element> elements = new List<GUI.Element>();

        Version version;

        private MouseState oldState;
        private MouseState newState;
        private int mouseX;
        private int mouseY;
        private Keys[] oldKeys;
        private Keys[] newKeys;

        private string skinName;

        private enum GameStatus { Home, Options, Game, Pause, End, Thanks }
        private GameStatus gameStatus;

        private int ScreenWidth = 1080;
        private int ScreenHeight = 720;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = ScreenWidth;
            graphics.PreferredBackBufferHeight = ScreenHeight;
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

            GUI.KeyString.InitializeKeyString();
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

            smallFont = content.Load<SpriteFont>("Fonts/small");
            basicFont = content.Load<SpriteFont>("Fonts/basic");
            titleFont = content.Load<SpriteFont>("Fonts/title");

            for (int i = 0; i < pointerSprites.Length; i++) {
                pointerSprites[i] = content.Load<Texture2D>("Textures/Hub/pointer" + i);
            }

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
                    if (Directory.Exists("Skin/" + skinName + "/Textures/Hub/"))
                    {
                        if(Directory.Exists("Skin/" + skinName + "/Textures/Hub/Buttons"))
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

            // TODO: use this.Content to load your game content here
            ChangeTo(GameStatus.Home);
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

            oldState = newState;
            newState = Mouse.GetState();
            mouseX = newState.X;
            mouseY = newState.Y;
            GUI.Mouse nowState;
            nowState.leftPress = (oldState.LeftButton == ButtonState.Released && newState.LeftButton == ButtonState.Pressed);
            nowState.leftRelease = (oldState.LeftButton == ButtonState.Pressed && newState.LeftButton == ButtonState.Released);
            nowState.rightPress = (oldState.LeftButton == ButtonState.Released && newState.LeftButton == ButtonState.Pressed);
            nowState.rightRelease = (oldState.LeftButton == ButtonState.Pressed && newState.LeftButton == ButtonState.Released);

            oldKeys = newKeys;
            newKeys = Keyboard.GetState().GetPressedKeys();

            Keys key = Keys.None;

            foreach (Keys newKey in newKeys)
            {
                if (!oldKeys.Contains(newKey)) { key = newKey; }
            }

            if (IsActive)
            {
                EventArgs e = new EventArgs();
                foreach(GUI.Element element in elements.ToArray())
                {
                    element.Update(mouseX, mouseY, nowState, key, Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift),e);
                }
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
            GraphicsDevice.Clear(Color.DarkGray);

            spriteBatch.Begin();

            /*
            switch (gameStatus)
            {
                case GameStatus.Home:
                    drawElements = true;
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
            */

                foreach (GUI.Element element in elements)
            {
                element.Draw(spriteBatch);
            }

            spriteBatch.Draw(pointerSprites[0], new Rectangle(mouseX - 10, mouseY - 10, 20, 20), Color.Red);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void ChangeTo(GameStatus newGameStatus)
        {
            //Things to do when leave status
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

            elements.Clear();

            //Initialise new status
            switch (newGameStatus)
            {
                case GameStatus.Home:
                    elements.Add(new GUI.Label(new GUI.Vector(ScreenWidth / 2, ScreenHeight / 4), "Galactic Colors Control", titleFont, new GUI.Colors(Color.DarkRed, Color.Green), GUI.Label.textAlign.centerCenter));
                    elements.Add(new GUI.TextField(new GUI.Vector(ScreenWidth / 2, ScreenHeight / 2), null, basicFont, new GUI.Colors(Color.White, Color.WhiteSmoke, Color.LightGray), GUI.Label.textAlign.centerCenter, "Server address", ConnectClick));
                    //elements.Add(new GUI.BoxButton(new Rectangle(ScreenWidth / 2 - 100, ScreenHeight * 3 / 4, 200, 40), buttonsSprites[0], new GUI.Colors(Color.White, Color.LightGray, Color.DarkGray), ConnectClick));
                    elements.Add(new GUI.BoxLabelButton(new Rectangle(ScreenWidth / 2 - 100, ScreenHeight * 3 / 4,200,40),buttonsSprites[0], new GUI.Colors(Color.White, Color.LightGray, Color.DarkGray), "Connect", basicFont, new GUI.Colors(Color.Black, Color.Black, Color.White), GUI.Label.textAlign.centerCenter, ConnectClick));
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
            gameStatus = newGameStatus;
        }

        private void ConnectClick(object sender, EventArgs e)
        {
            Console.WriteLine("plop");
        }
    }
}
