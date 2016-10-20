using System;
using MyMonoGame.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Galactic_Colors_Control;
using System.Threading;
using System.IO;
using Microsoft.Xna.Framework.Input;
using System.Reflection;
using System.Collections.Generic;

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
        private Texture2D logoSprite;
        private Texture2D[] backSprites = new Texture2D[2];
        private double[] backgroundX = new double[2];
        private double[] backgroundY = new double[2];
        private double acceleratorX = 1;

        private Texture2D[] pointerSprites = new Texture2D[1];
        private boxSprites[] buttonsSprites = new boxSprites[1];

        private Client client;
        private Manager GUI = new Manager();

        private string skinName;
        private bool isFullScren = false;

        private enum GameStatus { Home, Connect, Options, Game, Pause, End, Thanks,
            Title,
            Indentification,
            Kick
        }
        private GameStatus gameStatus = GameStatus.Home;

        private int ScreenWidth = 1280;
        private int ScreenHeight = 720;

        private string username = null;

        private static Thread Writer;
        private bool showOKMessage = false;
        private string messageTitle;
        private string messageText = string.Empty;
        private bool showYNMessage = false;
        private bool showLoading = false;
        private bool showChat = false;
        private string chatText = string.Empty;
        private string chatInput = string.Empty;

        public Game1()
        {
            if (isFullScren)
            {
                ScreenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                ScreenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = ScreenWidth;
            graphics.PreferredBackBufferHeight = ScreenHeight;
            graphics.IsFullScreen = isFullScren;
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

            smallFont = content.Load<SpriteFont>("Fonts/small");
            basicFont = content.Load<SpriteFont>("Fonts/basic");
            titleFont = content.Load<SpriteFont>("Fonts/title");

            for (int i = 0; i < pointerSprites.Length; i++) {
                pointerSprites[i] = content.Load<Texture2D>("Textures/Hub/pointer" + i);
            }

            backSprites[0] = content.Load<Texture2D>("Textures/background0");
            backSprites[1] = content.Load<Texture2D>("Textures/background1");

            logoSprite = content.Load<Texture2D>("Textures/LogoSmall");

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
                    Utilities.SpriteFromPng("Skin/" + skinName + "Textures/background0.png", ref backSprites[0], GraphicsDevice);
                    Utilities.SpriteFromPng("Skin/" + skinName + "Textures/background1.png", ref backSprites[1], GraphicsDevice);
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
            switch (gameStatus)
            {
                case GameStatus.Home:
                case GameStatus.Title:
                case GameStatus.Connect:
                case GameStatus.Indentification:
                    backgroundX[0] -= 1 * acceleratorX;
                    backgroundX[1] -= 2 * acceleratorX;
                    break;

                case GameStatus.Game:
                    if (client.Output.Count > 0)
                    {
                        string text = client.Output[0];
                        switch (text)
                        {
                            case "/clear":
                                chatText = string.Empty;
                                break;

                            default:
                                ChatAdd(text);
                                break;
                        }
                        client.Output.Remove(text);
                    }
                    if (!client.isRunning) { gameStatus = GameStatus.Kick; }
                    break;
            }

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

            switch (gameStatus)
            {
                case GameStatus.Title:
                    DrawBackground(0);
                    DrawBackground(1);
                    GUI.Label(new MyMonoGame.Vector(ScreenWidth / 2, ScreenHeight / 2), "Galactic Colors Control", titleFont, new MyMonoGame.Colors(Color.White), Manager.textAlign.centerCenter);
                    break;

                case GameStatus.Home:
                    DrawBackground(0);
                    DrawBackground(1);
                    GUI.Label(new MyMonoGame.Vector(ScreenWidth / 2, ScreenHeight / 4), "Galactic Colors Control", titleFont, new MyMonoGame.Colors(Color.White), Manager.textAlign.centerCenter);
                    GUI.Label(new MyMonoGame.Vector(ScreenWidth / 2, ScreenHeight / 4 + 40), "GUI " + Assembly.GetEntryAssembly().GetName().Version.ToString(), basicFont, new MyMonoGame.Colors(Color.White), Manager.textAlign.centerCenter);
                    if (GUI.Button(new Rectangle(ScreenWidth - 64, ScreenHeight - 74,64,64), logoSprite)) { System.Diagnostics.Process.Start("https://sheychen.shost.ca/"); }
                    if (GUI.Button(new Rectangle(ScreenWidth / 2 - 75, ScreenHeight / 2 - 30, 150, 40), buttonsSprites[0], "Play", basicFont, new MyMonoGame.Colors(Color.White, Color.Green))) {
                        GUI.ResetFocus();
                        client = new Client();
                        new Thread(() => {
                            while (acceleratorX < 5)
                            {
                                Thread.Sleep(20);
                                acceleratorX += 0.1d;
                            }
                            gameStatus = GameStatus.Connect;
                        }).Start();
                    }
                    //if (GUI.Button(new Rectangle(ScreenWidth / 2 - 75, ScreenHeight / 2 + 20, 150, 40), buttonsSprites[0], "Options", basicFont, new MyMonoGame.Colors(Color.White, Color.Blue))) {
                    //    GUI.ResetFocus();
                    //    gameStatus = GameStatus.Options;
                    //}
                    if (GUI.Button(new Rectangle(ScreenWidth / 2 - 75, ScreenHeight / 2 + 70, 150, 40), buttonsSprites[0], "Exit", basicFont, new MyMonoGame.Colors(Color.White, Color.Red))) {
                        GUI.ResetFocus();
                        gameStatus = GameStatus.Title;
                        new Thread(() => {
                            while (acceleratorX > 0)
                            {
                                Thread.Sleep(10);
                                acceleratorX -= 0.01d;
                            }
                            Exit();
                        }).Start();
                    }
                    break;

                case GameStatus.Connect:
                    DrawBackground(0);
                    DrawBackground(1);
                    GUI.Label(new MyMonoGame.Vector(ScreenWidth / 2, ScreenHeight / 4), "Galactic Colors Control", titleFont, new MyMonoGame.Colors(Color.White), Manager.textAlign.centerCenter);
                    if (showLoading)
                    {
                        GUI.Box(new Rectangle(ScreenWidth / 2 - 150, ScreenHeight / 4 + 50, 300, 50), buttonsSprites[0]);
                        GUI.Label(new Rectangle(ScreenWidth / 2 - 150, ScreenHeight / 4 + 50, 300, 50), "Loading", basicFont);
                    }
                    else
                    {
                        if (showOKMessage)
                        {
                            GUI.Box(new Rectangle(ScreenWidth / 2 - 150, ScreenHeight / 4 + 50, 300, 150), buttonsSprites[0]);
                            GUI.Label(new MyMonoGame.Vector(ScreenWidth / 2, ScreenHeight / 4 + 60), messageTitle, basicFont, null, Manager.textAlign.bottomCenter);
                            GUI.Label(new MyMonoGame.Vector(ScreenWidth / 2, ScreenHeight / 4 + 100), messageText, smallFont, null, Manager.textAlign.bottomCenter);
                            if (GUI.Button(new Rectangle(ScreenWidth / 2 - 140, ScreenHeight / 4 + 150, 280, 40), buttonsSprites[0], "Ok", basicFont)) { GUI.ResetFocus(); showOKMessage = false; }
                        }
                        else {
                            if (showYNMessage)
                            {
                                GUI.Box(new Rectangle(ScreenWidth / 2 - 150, ScreenHeight / 4 + 50, 300, 100), buttonsSprites[0]);
                                GUI.Label(new MyMonoGame.Vector(ScreenWidth / 2, ScreenHeight / 4 + 60), messageTitle, basicFont, null, Manager.textAlign.bottomCenter);
                                if (GUI.Button(new Rectangle(ScreenWidth / 2 - 140, ScreenHeight / 4 + 100, 135, 40), buttonsSprites[0], "Yes", basicFont))
                                {
                                    GUI.ResetFocus();
                                    new Thread(ConnectHost).Start();
                                    showYNMessage = false;
                                }
                                if (GUI.Button(new Rectangle(ScreenWidth / 2 + 5, ScreenHeight / 4 + 100, 135, 40), buttonsSprites[0], "No", basicFont))
                                {
                                    client.Output.Clear();
                                    client.ResetHost();
                                    GUI.ResetFocus();
                                    showYNMessage = false;
                                }
                            }
                            else {
                                if (GUI.TextField(new Rectangle(ScreenWidth / 2 - 75, ScreenHeight / 2 - 30, 150, 40), ref username, basicFont, new MyMonoGame.Colors(Color.LightGray, Color.White), Manager.textAlign.centerCenter, "Server address")) { new Thread(ValidateHost).Start(); }
                                if (GUI.Button(new Rectangle(ScreenWidth / 2 - 75, ScreenHeight / 2 + 20, 150, 40), buttonsSprites[0], "Connect", basicFont, new MyMonoGame.Colors(Color.LightGray, Color.White))) { new Thread(ValidateHost).Start(); }
                                if (GUI.Button(new Rectangle(ScreenWidth / 2 - 75, ScreenHeight / 2 + 70, 150, 40), buttonsSprites[0], "Back", basicFont, new MyMonoGame.Colors(Color.LightGray, Color.White)))
                                {
                                    GUI.ResetFocus();
                                    new Thread(() =>
                                    {
                                        while (acceleratorX > 1)
                                        {
                                            Thread.Sleep(20);
                                            acceleratorX -= 0.1d;
                                        }
                                        gameStatus = GameStatus.Home;
                                        username = null;
                                    }).Start();
                                }
                            }
                        }
                    }
                    break;

                case GameStatus.Indentification:
                    DrawBackground(0);
                    DrawBackground(1);
                    GUI.Label(new MyMonoGame.Vector(ScreenWidth / 2, ScreenHeight / 4), "Galactic Colors Control", titleFont, new MyMonoGame.Colors(Color.White), Manager.textAlign.centerCenter);
                    if (showLoading)
                    {
                        GUI.Box(new Rectangle(ScreenWidth / 2 - 150, ScreenHeight / 4 + 50, 300, 50), buttonsSprites[0]);
                        GUI.Label(new Rectangle(ScreenWidth / 2 - 150, ScreenHeight / 4 + 50, 300, 50), "Loading", basicFont);
                    }
                    else
                    {
                        if (showOKMessage)
                        {
                            GUI.Box(new Rectangle(ScreenWidth / 2 - 150, ScreenHeight / 4 + 50, 300, 150), buttonsSprites[0]);
                            GUI.Label(new MyMonoGame.Vector(ScreenWidth / 2, ScreenHeight / 4 + 60), messageTitle, basicFont, null, Manager.textAlign.bottomCenter);
                            GUI.Label(new MyMonoGame.Vector(ScreenWidth / 2, ScreenHeight / 4 + 100), messageText, smallFont, null, Manager.textAlign.bottomCenter);
                            if (GUI.Button(new Rectangle(ScreenWidth / 2 - 140, ScreenHeight / 4 + 150, 280, 40), buttonsSprites[0], "Ok", basicFont)) { GUI.ResetFocus(); showOKMessage = false; }
                        }
                        else {
                            if (GUI.TextField(new Rectangle(ScreenWidth / 2 - 75, ScreenHeight / 2 - 30, 150, 40), ref username, basicFont, new MyMonoGame.Colors(Color.LightGray, Color.White), Manager.textAlign.centerCenter, "Username")) { new Thread(IdentifiacateHost).Start(); }
                            if (GUI.Button(new Rectangle(ScreenWidth / 2 - 75, ScreenHeight / 2 + 20, 150, 40), buttonsSprites[0], "Validate", basicFont, new MyMonoGame.Colors(Color.LightGray, Color.White))) { new Thread(IdentifiacateHost).Start(); }
                            if (GUI.Button(new Rectangle(ScreenWidth / 2 - 75, ScreenHeight / 2 + 70, 150, 40), buttonsSprites[0], "Back", basicFont, new MyMonoGame.Colors(Color.LightGray, Color.White)))
                            {
                                GUI.ResetFocus();
                                new Thread(() =>
                                {
                                    while (acceleratorX > 1)
                                    {
                                        Thread.Sleep(20);
                                        acceleratorX -= 0.1d;
                                    }
                                    gameStatus = GameStatus.Home;
                                    username = null;
                                }).Start();
                            }
                        }
                    }
                    break;

                case GameStatus.Game:
                    DrawBackground(0);
                    DrawBackground(1);
                    GUI.Texture(new Rectangle(0,0,ScreenWidth, 30), nullSprite, new MyMonoGame.Colors(new Color(0.1f,0.1f,0.1f)));
                    if(GUI.Button(new Rectangle(5, 5, 50, 20), (showChat ? "Hide" : "Show") + " chat", smallFont, new MyMonoGame.Colors(Color.White, Color.LightGray, Color.Gray))) { GUI.ResetFocus(); showChat = !showChat; }
                    if (showChat)
                    {
                        GUI.Box(new Rectangle(0, 30, 310, 310), buttonsSprites[0]);
                        if(GUI.TextField(new Rectangle(5,35,305,20), ref chatInput, basicFont, null, Manager.textAlign.centerLeft, "Enter message")) { if(chatInput != null) { ChatAdd(chatInput); client.SendRequest(chatInput); chatInput = null; } }
                        GUI.Label(new Rectangle(5, 60, 305, 245), chatText, smallFont, null, Manager.textAlign.topLeft, true);
                    }
                    break;
            }

            Color ActiveColor = IsActive ? Color.Green : Color.Red;
            GUI.Label(new MyMonoGame.Vector(10, ScreenHeight - 20), (1 / (float)gameTime.ElapsedGameTime.TotalSeconds).ToString(), smallFont, new MyMonoGame.Colors(ActiveColor));
            spriteBatch.Draw(pointerSprites[0], new Rectangle(Mouse.GetState().X - 10, Mouse.GetState().Y - 10, 20, 20), Color.Red);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void ValidateHost()
        {
            showLoading = true;
            if ( username == null) { username = ""; }
            string Host = client.ValidateHost(username);
            if (Host == null)
            {
                messageTitle = "Error";
                messageText = string.Empty;
                foreach(string line in client.Output.ToArray()) { messageText += (line + Environment.NewLine); }
                showOKMessage = true;
                client.Output.Clear();
                client.ResetHost();;
            }
            else
            {
                messageTitle = "Use " + Host + "?";
                showYNMessage = true;
            }
            showLoading = false;
        }

        private void ConnectHost()
        {
            showLoading = true;
            if (client.ConnectHost())
            {
                gameStatus = GameStatus.Indentification;
            }
            else
            {
                messageTitle = "Error";
                messageText = string.Empty;
                foreach (string line in client.Output.ToArray()) { messageText += (line + Environment.NewLine); }
                showOKMessage = true;
                client.Output.Clear();
                client.ResetHost();
            }
            showLoading = false;
        }

        private void IdentifiacateHost()
        {
            showLoading = true;
            if (username != null)
            {
                if(username.Length > 3)
                {
                    client.Output.Clear();
                    client.SendRequest("/connect " + username);
                    bool wait = true;
                    while (wait)
                    {
                        if (client.Output.Count > 0)
                        {
                            wait = false;
                        }
                    }
                    if(client.Output.Contains("Identifiaction succes"))
                    {
                        gameStatus = GameStatus.Game;
                    }
                    else
                    {
                        messageTitle = "Error";
                        messageText = string.Empty;
                        foreach (string line in client.Output.ToArray()) { messageText += (line + Environment.NewLine); }
                        showOKMessage = true;
                        showLoading = false;
                        client.Output.Clear();
                    }
                }
            }
            showLoading = false;
        }

        private void ChatAdd(string text)
        {
            chatText += ((chatText != string.Empty ? Environment.NewLine : "") + text);
        }

        private void DrawBackground(int index)
        {
            if (backgroundX[index] > backSprites[index].Width) { backgroundX[index] = 0; }
            if (backgroundY[index] > backSprites[index].Height) { backgroundY[index] = 0; }
            if (backgroundX[index] < 0) { backgroundX[index] = backSprites[index].Width; }
            if (backgroundY[index] < 0) { backgroundY[index] = backSprites[index].Height; }
            for (int X = -1; X < ScreenWidth / backSprites[index].Width + 1; X++)
            {
                for (int Y = -1; Y < ScreenHeight / backSprites[index].Height + 1; Y++)
                {
                    GUI.Texture(new Rectangle(X * backSprites[index].Width + (int)backgroundX[index], Y * backSprites[index].Height + (int)backgroundY[index], backSprites[index].Width, backSprites[index].Height), backSprites[index], new MyMonoGame.Colors(Color.White));
                }
            }
        }
    }
}
