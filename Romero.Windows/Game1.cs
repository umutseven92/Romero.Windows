#region Using Statements

using System;
using System.IO;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Romero.Windows.Screens;

#endregion

namespace Romero.Windows
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        public GraphicsDeviceManager Graphics;
        SpriteBatch _spriteBatch;

        private readonly string _userConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Romero","UserConfig");

        // By preloading any assets used by UI rendering, we avoid framerate glitches
        // when they suddenly need to be loaded in the middle of a menu transition.
        static readonly string[] PreloadAssets =
        {
            "gradient",
        };

        public Game1()
        {
            if (!File.Exists(Path.Combine(_userConfigPath,"config.xml")))
            {
                CreateUserConfig();
            }

            Graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferHeight = 1080,
                PreferredBackBufferWidth = 1920,
                IsFullScreen = Global.IsFullScreen
            };

            Graphics.ApplyChanges();
            Global.DeviceInUse = Graphics;
            Content.RootDirectory = "Content/Sprites";
            Global.GameInProgress = this;
            var screenManager = new ScreenManager.ScreenManager(this);

            Components.Add(screenManager);

            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                Global.Gamepad = true;
            }

            // Activate the first screens.
            screenManager.AddScreen(new BackgroundScreen(), null);
            screenManager.AddScreen(new MainMenuScreen(), null);
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);


            foreach (var asset in PreloadAssets)
            {
                Content.Load<object>(asset);
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (Global.ScreenChanged)
            {
                Graphics.ToggleFullScreen();
                Graphics.ApplyChanges();
                Global.ScreenChanged = false;
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

            base.Draw(gameTime);
        }

        private void CreateUserConfig()
        {
            Directory.CreateDirectory(_userConfigPath);
            var doc = new XDocument
                (new XComment("User configuration files"),
                new XElement("Root",
                    new XElement("SelectedCharacter", "Fraser"),
                    new XElement("SelectedDifficulty", "Normal"),
                    new XElement("IsDiagnosticsOpen", "true"))
                    );
            doc.Save(Path.Combine(_userConfigPath,"config.xml"));

        }

    }
}
