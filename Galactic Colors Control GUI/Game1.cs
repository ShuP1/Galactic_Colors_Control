using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using MyMonoGame.GUI;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

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

        private List<String> chat = new List<string>();

        private string skinName;
        private bool isFullScren = false;
        private Manager GUI = new Manager();

        private enum dataType { message, data };

        Version version;

        private enum GameStatus { Home, Connect, Connection, Options, Game, Pause, End, Thanks, Exit, Error}
        private GameStatus gameStatus = GameStatus.Home;

        private int ScreenWidth = 1280;
        private int ScreenHeight = 720;

        private string addressText;
        private static Socket ClientSocket = new Socket
            (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private static int PORT = 0;
        private static string IP = null;
        private string username;

        private Thread ReceiveThread;
        private static bool _run = true;
        private string errorText;
        private int _errorCount = 0;

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
            version = Assembly.GetEntryAssembly().GetName().Version;
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
                    backgroundX[0] -= 1 * acceleratorX;
                    backgroundX[1] -= 2 * acceleratorX;
                    break;

                case GameStatus.Connect:
                    backgroundX[0] -= 1 * acceleratorX;
                    backgroundX[1] -= 2 * acceleratorX;
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
                case GameStatus.Home:
                    DrawBackground(0);
                    DrawBackground(1);
                    GUI.Label(new MyMonoGame.Vector(ScreenWidth / 2, ScreenHeight / 4), "Galactic Colors Control", titleFont, new MyMonoGame.Colors(Color.White), Manager.textAlign.centerCenter);
                    if (GUI.Button(new Rectangle(ScreenWidth - 64, ScreenHeight - 74,64,64), logoSprite)) { System.Diagnostics.Process.Start("https://sheychen.shost.ca/"); }
                    if (GUI.Button(new Rectangle(ScreenWidth / 2 - 75, ScreenHeight / 2 - 30, 150, 40), buttonsSprites[0], "Connect", basicFont, new MyMonoGame.Colors(Color.White, Color.Green))) { new Thread(ChangeTo).Start(GameStatus.Connect); }
                    if (GUI.Button(new Rectangle(ScreenWidth / 2 - 75, ScreenHeight / 2 + 20, 150, 40), buttonsSprites[0], "Options", basicFont, new MyMonoGame.Colors(Color.White, Color.Blue))) { GUI.ResetFocus(); gameStatus = GameStatus.Options; }
                    if (GUI.Button(new Rectangle(ScreenWidth / 2 - 75, ScreenHeight / 2 + 70, 150, 40), buttonsSprites[0], "Exit", basicFont, new MyMonoGame.Colors(Color.White, Color.Red))) { new Thread(ChangeTo).Start(GameStatus.Exit); }
                    break;

                case GameStatus.Connect:
                    DrawBackground(0);
                    DrawBackground(1);
                    GUI.Label(new MyMonoGame.Vector(ScreenWidth / 2, ScreenHeight / 4), "Connnect", titleFont, new MyMonoGame.Colors(Color.White), Manager.textAlign.centerCenter);
                    if (GUI.TextField(new Rectangle(ScreenWidth / 2 - 75, ScreenHeight / 2 - 30, 150, 40), ref addressText, basicFont, new MyMonoGame.Colors(Color.White), Manager.textAlign.centerCenter, "Server address")) { new Thread(ChangeTo).Start(GameStatus.Connection); }
                    if (GUI.Button(new Rectangle(ScreenWidth / 2 - 75, ScreenHeight / 2 + 20, 150, 40), buttonsSprites[0], "Connection", basicFont)) { new Thread(ChangeTo).Start(GameStatus.Connection); }
                    if (GUI.Button(new Rectangle(ScreenWidth / 2 - 75, ScreenHeight / 2 + 70, 150, 40), buttonsSprites[0], "Back", basicFont, new MyMonoGame.Colors(Color.White, Color.Red))) { new Thread(ChangeTo).Start(GameStatus.Home); }
                    break;

                case GameStatus.Connection:
                    DrawBackground(0);
                    DrawBackground(1);
                    GUI.Label(new MyMonoGame.Vector(ScreenWidth / 2, ScreenHeight / 4), "Connnection", titleFont, new MyMonoGame.Colors(Color.White), Manager.textAlign.centerCenter);
                    GUI.Label(new Rectangle(ScreenWidth / 2 - 75, ScreenHeight / 4 + 30, 150, 40), addressText, basicFont, new MyMonoGame.Colors(Color.White));
                    if (GUI.TextField(new Rectangle(ScreenWidth / 2 - 75, ScreenHeight / 2 - 30, 150, 40), ref username, basicFont, new MyMonoGame.Colors(Color.White), Manager.textAlign.centerCenter, "Username")) { if (username != null) { new Thread(ChangeTo).Start(GameStatus.Game); } }
                    if (GUI.Button(new Rectangle(ScreenWidth / 2 - 75, ScreenHeight / 2 + 20, 150, 40), buttonsSprites[0], "Connection", basicFont)) { if (username != null) { new Thread(ChangeTo).Start(GameStatus.Game); } }
                    if (GUI.Button(new Rectangle(ScreenWidth / 2 - 75, ScreenHeight / 2 + 70, 150, 40), buttonsSprites[0], "Back", basicFont, new MyMonoGame.Colors(Color.White, Color.Red))) { _run = false; ReceiveThread.Join(); new Thread(ChangeTo).Start(GameStatus.Home); }
                    break;

                case GameStatus.Error:
                    GUI.Label(new MyMonoGame.Vector(ScreenWidth / 2, ScreenHeight / 4), "Error", titleFont, null, Manager.textAlign.centerCenter);
                    GUI.Label(new Rectangle(ScreenWidth / 2 - 75, ScreenHeight / 4 + 30, 150, 40), errorText, basicFont);
                    if (GUI.Button(new Rectangle(ScreenWidth / 2 - 75, ScreenHeight / 2 + 70, 150, 40), buttonsSprites[0], "Exit", basicFont, new MyMonoGame.Colors(Color.White, Color.Red))) { _run = false; ReceiveThread.Join(); new Thread(ChangeTo).Start(GameStatus.Exit); }
                    break;

                case GameStatus.Options:

                    break;

                case GameStatus.Game:
                    DrawBackground(0);
                    DrawBackground(1);
                    int i = 1;
                    foreach(string ligne in chat.ToArray().Reverse())
                    {
                        GUI.Label(new MyMonoGame.Vector(10, ScreenHeight - 12 * i), ligne, smallFont, new MyMonoGame.Colors(Color.White), Manager.textAlign.topRight);
                        i++;
                    }
                    break;

                case GameStatus.Pause:

                    break;

                case GameStatus.End:

                    break;

                case GameStatus.Thanks:

                    break;
            }

            Color ActiveColor = IsActive ? Color.Green : Color.Red;
            GUI.Label(new MyMonoGame.Vector(10, 10), (1 / (float)gameTime.ElapsedGameTime.TotalSeconds).ToString(), smallFont, new MyMonoGame.Colors(ActiveColor));
            spriteBatch.Draw(pointerSprites[0], new Rectangle(Mouse.GetState().X - 10, Mouse.GetState().Y - 10, 20, 20), Color.Red);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void ChangeTo( object target )
        {
            GUI.ResetFocus();
            switch ((GameStatus)target)
            {
                case GameStatus.Home:
                    if(gameStatus == GameStatus.Connect)
                    {
                        while (acceleratorX > 1)
                        {
                            Thread.Sleep(20);
                            acceleratorX -= 0.1d;
                        }
                    }
                    if (gameStatus == GameStatus.Connection || gameStatus == GameStatus.Error)
                    {
                        ClientSocket.Close();
                        ClientSocket = new Socket
            (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        while (acceleratorX > 1)
                        {
                            Thread.Sleep(20);
                            acceleratorX -= 0.1d;
                        }
                    }
                    break;

                case GameStatus.Connect:
                    addressText = null;
                    while (acceleratorX < 5)
                    {
                        Thread.Sleep(20);
                        acceleratorX += 0.1d;
                    }
                    break;

                case GameStatus.Connection:
                    if(addressText == null) { addressText = ""; }
                    string text = addressText;
                    string[] parts = text.Split(new char[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 0)
                    {
                        parts = new string[] { "" };
                        PORT = 25001;
                    }
                    else
                    {
                        if (parts.Length > 1)
                        {
                            if (!int.TryParse(parts[1], out PORT)) { PORT = 0; }
                            if (PORT < 0 || PORT > 65535) { PORT = 0; }
                        }
                        else
                        {
                            PORT = 25001;
                        }
                    }
                    if (PORT != 0)
                    {
                        try
                        {
                            IPHostEntry ipHostEntry = Dns.GetHostEntry(parts[0]);
                            IPAddress host = ipHostEntry.AddressList.First(a => a.AddressFamily == AddressFamily.InterNetwork);
                            IP = host.ToString();
                        }
                        catch (Exception e)
                        {
                            addressText = e.Message;
                            PORT = 0;
                            Thread.CurrentThread.Abort();
                        }
                    }
                    else
                    {
                        addressText = "Incorrect port";
                        Thread.CurrentThread.Abort();
                    }
                    if(IP != null)
                    {
                        int attempts = 0;
                        while (!ClientSocket.Connected && attempts < 5)
                        {
                            try
                            {
                                attempts++;
                                addressText = "Connection attempt " + attempts;
                                ClientSocket.Connect(IP, PORT);
                            }
                            catch (SocketException)
                            {
                                addressText = "Error";
                            }
                        }
                        if (attempts < 5)
                        {
                            addressText = "Connected to " + IP.ToString();
                            _run = true;
                            chat.Clear();
                            ReceiveThread = new Thread(ReceiveLoop);
                            ReceiveThread.Start();
                        }
                        else
                        {
                            addressText = "Can't connected to " + IP.ToString();
                            Thread.CurrentThread.Abort();
                        }
                    }
                    break;

                case GameStatus.Game:
                    Send("/connect " + username, dataType.message);
                    break;

                case GameStatus.Exit:
                    while (acceleratorX > 0)
                    {
                        Thread.Sleep(20);
                        acceleratorX -= 0.1d;
                    }
                    Thread.Sleep(500);
                    Exit();
                    break;
            }
            gameStatus = (GameStatus)target;
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

        private void ReceiveLoop()
        {
            while (_run)
            {
                var buffer = new byte[2048];
                int received = 0;
                try
                {
                    received = ClientSocket.Receive(buffer, SocketFlags.None);
                }
                catch
                {
                    errorText = "Server Timeout";
                    new Thread(ChangeTo).Start(GameStatus.Error);
                }
                if (received == 0) return;
                _errorCount = 0;
                var data = new byte[received];
                Array.Copy(buffer, data, received);
                byte[] type = new byte[4];
                type = data.Take(4).ToArray();
                type.Reverse();
                dataType dtype = (dataType)BitConverter.ToInt32(type, 0);
                byte[] bytes = null;
                bytes = data.Skip(4).ToArray();
                switch (dtype)
                {
                    case dataType.message:
                        string text = Encoding.ASCII.GetString(bytes);
                        if (text[0] == '/')
                        {
                            text = text.Substring(1);
                            text = text.ToLower();
                            string[] array = text.Split(new char[1] { ' ' }, 4, StringSplitOptions.RemoveEmptyEntries);
                            switch (array[0])
                            {
                                case "kick":
                                    if (array.Length > 1)
                                    {
                                        errorText = "Kick : " + array[1];
                                        new Thread(ChangeTo).Start(GameStatus.Error);
                                    }
                                    else
                                    {
                                        errorText = "Kick by server";
                                        new Thread(ChangeTo).Start(GameStatus.Error);
                                    }
                                    _run = false;
                                    break;

                                default:
                                    chat.Add("Unknown action from server");
                                    break;
                            }
                        }
                        else
                        {
                            chat.Add(text);
                        }
                        break;

                    case dataType.data:
                        chat.Add("data");
                        break;
                }
                Thread.Sleep(200);
            }
        }

        private void Send(object data, dataType dtype)
        {
            byte[] type = new byte[4];
            type = BitConverter.GetBytes((int)dtype);
            byte[] bytes = null;
            switch (dtype)
            {
                case dataType.message:
                    bytes = Encoding.ASCII.GetBytes((string)data);
                    break;

                case dataType.data:
                    BinaryFormatter bf = new BinaryFormatter();
                    using (MemoryStream ms = new MemoryStream())
                    {
                        bf.Serialize(ms, data);
                        bytes = ms.ToArray();
                    }
                    break;
            }
            byte[] final = new byte[type.Length + bytes.Length];
            type.CopyTo(final, 0);
            bytes.CopyTo(final, type.Length);
            try
            {
                ClientSocket.Send(final);
            }
            catch
            {
                chat.Add("Can't contact server : " + _errorCount);
                _errorCount++;
            }
            if (_errorCount >= 5)
            {
                errorText = "Can't contact server";
                new Thread(ChangeTo).Start(GameStatus.Error);
            }
        }
    }
}
