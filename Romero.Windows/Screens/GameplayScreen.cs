#region Using Statements

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Romero.Windows.Classes;
using Romero.Windows.ScreenManager;

#endregion

namespace Romero.Windows.Screens
{
    /// <summary>
    /// This screen implements the actual game logic
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Declarations

        ContentManager _content;
        private readonly Player _singlePlayerPlayer;
        private GameTime _gT;
        private readonly List<Zombie> _lZombies;
        TimeSpan _elapsedTime = TimeSpan.Zero;
        float _pauseAlpha;
        readonly Rectangle _bgRectangle = new Rectangle(0, 0, 4096, 4096); //Map size
        private readonly Camera2D _cam;
        private KeyboardState _previousKeyboardState;
        float _songFadeCounterStart;
        private bool _everythingIsLoaded;

        #region Texture & Song & Font & Sound Effect

        Texture2D _backgroundTexture;
        private SpriteFont _diagnosticFont;
        private SpriteFont _waveFont;
        private SoundEffect _bell;

        #endregion

        #region Multiplayer

        private readonly NetClient _client;
        private readonly Dictionary<long, string> _playerNames;
        private readonly Dictionary<long, Vector2> _playerPositions = new Dictionary<long, Vector2>();
        private readonly Dictionary<long, float> _playerAngles = new Dictionary<long, float>();
        private Player _multiPlayerOne;
        private PlayerPuppet _multiPlayerTwo;
        private PlayerPuppet _multiPlayerThree;
        private PlayerPuppet _multiPlayerFour;
        private readonly List<PlayerPuppet> _otherPlayers = new List<PlayerPuppet>();
        private int _previousSpritePositionX = 2048;
        private int _previousSpritePositionY = 2048;
        private float _previousAngle;

        #endregion

        #region Game Mode

        enum GameMode
        {
            Singleplayer,
            Multiplayer
        }

        private readonly GameMode _currentGameMode;

        #endregion

        #region Zombie Values

        private int _deadZombies;
        private int _zombieModifier; //Zombie base spawn rate, increases with higher difficulty
        private int _difficultyModifier; //Difficulty rate,  increases with higher difficulty
        private int _diagZombieCount; //Diagnostics - zombie count
        private int _wave = 1;
        int _zombieSpawnChecker;
        private int _howManyZombiesEachRound = 10;
        private const float HowManyZombieModifier = 1.5f;

        #endregion

        #region FPS Counter

        int _frameRate;
        int _frameCounter;

        #endregion

        #region Vibration Timer
        /*
        internal bool Vibrate = false;
        const int VibrationTime = 2;
        private float vibrationCounterStart;
        */
        #endregion

        #region Wave Notifier

        private bool _waveNotify;
        private const int NotifyTime = 3;
        private float _notifyCounterStart;

        #endregion

        #endregion

        /// <summary>
        /// Singleplayer Constructor
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            _currentGameMode = GameMode.Singleplayer;

            SetDifficulty();

            _singlePlayerPlayer = new Player();
            _cam = new Camera2D(new Viewport(0, 0, Global.DeviceInUse.GraphicsDevice.PresentationParameters.BackBufferWidth, Global.DeviceInUse.GraphicsDevice.PresentationParameters.BackBufferHeight), _singlePlayerPlayer);
            _singlePlayerPlayer.GetCamera(_cam);

            _lZombies = new List<Zombie>();
            AddZombies(_zombieModifier * _difficultyModifier);

        }

        /// <summary>
        /// Multiplayer Constructor
        /// </summary>
        /// <param name="players">Dictionary of players</param>
        /// <param name="client">NetClient of the player</param>
        public GameplayScreen(Dictionary<long, string> players, NetClient client)
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            SetDifficulty();

            Global.GameInProgress.Exiting += GameInProgress_Exiting;
            _currentGameMode = GameMode.Multiplayer;
            _client = client;
            _playerNames = players;

            InitializeMultiplayerPlayers(_playerNames.Count);

            _cam = new Camera2D(new Viewport(0, 0, Global.DeviceInUse.GraphicsDevice.PresentationParameters.BackBufferWidth, Global.DeviceInUse.GraphicsDevice.PresentationParameters.BackBufferHeight), _multiPlayerOne);
            _multiPlayerOne.GetCamera(_cam);

            _lZombies = new List<Zombie>();
            AddZombies(_zombieModifier * _difficultyModifier);

        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            #region Load Assets

            _backgroundTexture = _content.Load<Texture2D>("Sprites/ground");
            _diagnosticFont = _content.Load<SpriteFont>("Fonts/font");
            _waveFont = _content.Load<SpriteFont>("Fonts/waveFont");
            _bell = _content.Load<SoundEffect>("Sounds/bell");

            #endregion

            #region Load Players

            if (_currentGameMode == GameMode.Singleplayer)
            {
                _singlePlayerPlayer.LoadContent(_content);
            }
            else
            {
                LoadMultiplayerPlayers(_playerNames.Count);

                var i = 0;
                _playerNames.Remove(_client.UniqueIdentifier);
                foreach (var p in _playerNames)
                {
                    _otherPlayers[i].id = p.Key;
                    _otherPlayers[i].playerName = p.Value;
                    i++;
                }
                _playerNames.Clear();
            }

            #endregion

            #region Load Zombies

            foreach (var z in _lZombies)
            {
                z.LoadContent(_content);
            }

            #endregion

            //Screen Delay
            Thread.Sleep(1000);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
            _waveNotify = true;
        }

        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            _content.Unload();
        }

        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            _gT = gameTime;
            FadeOutSong(gameTime, 0.1f, 0.01f, 0.05f);

            if (_currentGameMode == GameMode.Multiplayer)
            {
                SendInfoToServer();
                ReadInfoFromServer();
            }

            #region Wave Notifier

            if (_waveNotify)
            {
                _notifyCounterStart += (float)_gT.ElapsedGameTime.TotalSeconds;
                if (_notifyCounterStart >= NotifyTime)
                {
                    _waveNotify = false;
                    _notifyCounterStart = 0f;
                }
            }

            #endregion

            CheckZombieDeaths();

            #region FPS calculation

            _elapsedTime += gameTime.ElapsedGameTime;
            if (_elapsedTime > TimeSpan.FromSeconds(1))
            {
                _elapsedTime -= TimeSpan.FromSeconds(1);
                _frameRate = _frameCounter;
                _frameCounter = 0;
            }

            #endregion

            _cam.Update();
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            _pauseAlpha = coveredByOtherScreen ? Math.Min(_pauseAlpha + 1f / 32, 1) : Math.Max(_pauseAlpha - 1f / 32, 0);

        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                                 Color.DarkGreen, 0, 0);

            var spriteBatch = ScreenManager.SpriteBatch;

            #region Draw Everything But GUI

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, _cam.Transform);

            spriteBatch.Draw(_backgroundTexture, _bgRectangle, Color.White);
            foreach (var z in _lZombies)
            {
                Vector2 direction;

                if (_currentGameMode == GameMode.Singleplayer)
                {
                    direction = z.SpritePosition - _singlePlayerPlayer.SpritePosition;
                }
                else
                {
                    direction = z.SpritePosition - _multiPlayerOne.SpritePosition;
                }

                var angle = (float)(Math.Atan2(direction.Y, direction.X) + Math.PI / 2 + Math.PI);

                z.Draw(spriteBatch, angle);
            }

            #region Draw Players

            if (_currentGameMode == GameMode.Singleplayer)
            {
                _singlePlayerPlayer.Draw(spriteBatch);
            }
            else
            {
                _multiPlayerOne.Draw(spriteBatch);
                var i = 0;
                _playerPositions.Remove(_client.UniqueIdentifier);
                foreach (var playerPosition in _playerPositions)
                {
                    var angle = _playerAngles.Single(a => a.Key == playerPosition.Key);
                    _otherPlayers[i].Draw(spriteBatch, playerPosition.Value, angle.Value);
                    i++;
                }

            }

            #endregion

            spriteBatch.End();

            #endregion

            #region Draw GUI

            spriteBatch.Begin();

            DrawDiagnostics(spriteBatch);

            #region Draw Wave Notifier

            if (_waveNotify && _everythingIsLoaded)
            {
                spriteBatch.DrawString(_waveFont, "Wave " + _wave, new Vector2(Global.DeviceInUse.PreferredBackBufferWidth / 2 - 70, Global.DeviceInUse.PreferredBackBufferHeight * 1/6), Color.Red);
            }

            #endregion

            spriteBatch.End();

            #endregion

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || _pauseAlpha > 0)
            {
                var alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, _pauseAlpha / 2);
                ScreenManager.FadeBackBufferToBlack(alpha);
            }

            if (!_everythingIsLoaded)
            {
                _everythingIsLoaded = true;
            }
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            var currentKeyboardState = Keyboard.GetState();

            if (currentKeyboardState.IsKeyDown(Keybinds.DeveloperDiagnostics) && !_previousKeyboardState.IsKeyDown(Keybinds.DeveloperDiagnostics))
            {
                Global.IsDiagnosticsOpen = !Global.IsDiagnosticsOpen;
            }

            #region Developer Kill Cheat

            if (currentKeyboardState.IsKeyDown(Keybinds.DeveloperKillAll) && !_previousKeyboardState.IsKeyDown(Keybinds.DeveloperKillAll))
            {
                foreach (var z in _lZombies.Where(z => !z.Dead && z.Visible))
                {
                    z.Visible = false;
                    z.Dead = true;
                    _deadZombies++;
                    _diagZombieCount--;
                    if (_deadZombies % Global.ZombieSpawnTicker == 0)
                    {
                        foreach (var d in _lZombies.Where(d => !d.Dead && !d.Visible && _zombieSpawnChecker < Global.ZombieSpawnTicker))
                        {
                            d.Visible = true;
                            _zombieSpawnChecker++;
                        }

                    }
                    _zombieSpawnChecker = 0;
                }
            }

            #endregion

            _previousKeyboardState = currentKeyboardState;

            if (input.IsPauseGame(ControllingPlayer))
            {
                // Pause (esc)
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
                #region Player Movement

                if (_currentGameMode == GameMode.Singleplayer)
                {
                    _singlePlayerPlayer.Update(_gT, _cam);
                }
                else
                {
                    _multiPlayerOne.Update(_gT, _cam);
                }

                #endregion

            }

            #region Zombie Movement

            foreach (var z in _lZombies)
            {
                z.Update(_gT, _currentGameMode == GameMode.Singleplayer ? _singlePlayerPlayer : _multiPlayerOne);
            }

            #endregion

            #region Collision

            foreach (var z in _lZombies)
            {
                #region Bullet - Zombie Collision

                if (_currentGameMode == GameMode.Singleplayer)
                {
                    foreach (var b in _singlePlayerPlayer.Bullets)
                    {
                        if (z.BoundingBox.Intersects(b.BoundingBox) && z.Visible && b.Visible)
                        {

                            b.Visible = false;
                            KillZombie(z, _zombieSpawnChecker);

                        }
                    }
                }
                else
                {
                    foreach (var b in _multiPlayerOne.Bullets)
                    {
                        if (z.BoundingBox.Intersects(b.BoundingBox) && z.Visible && b.Visible)
                        {

                            b.Visible = false;
                            KillZombie(z, _zombieSpawnChecker);

                        }
                    }
                }

                #endregion

                #region Zombie - Player Collision

                if (_currentGameMode == GameMode.Singleplayer)
                {
                    if (z.BoundingBox.Intersects(_singlePlayerPlayer.BoundingBox) && z.Visible && _singlePlayerPlayer.CurrentState != Player.State.Dodging && !_singlePlayerPlayer.Invulnerable)
                    {

                        #region Vibration
                        /*
                    if (Global.Gamepad)
                    {
                        if (!Vibrate)
                        {
                            Vibrate = true;
                        }

                        if (Vibrate)
                        {
                            GamePad.SetVibration(PlayerIndex.One, 1.0f, 1.0f);
                            vibrationCounterStart += (float)_gT.ElapsedGameTime.TotalSeconds;
                            if (vibrationCounterStart >= VibrationTime)
                            {
                                GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
                                Vibrate = false;
                                vibrationCounterStart = 0f;
                            }

                        }


                    } 
                     */
                        #endregion

                        _singlePlayerPlayer.Health -= 10;
                        _singlePlayerPlayer.Invulnerable = true;

                    }
                }
                else
                {
                    if (z.BoundingBox.Intersects(_multiPlayerOne.BoundingBox) && z.Visible && _multiPlayerOne.CurrentState != Player.State.Dodging && !_multiPlayerOne.Invulnerable)
                    {

                        _multiPlayerOne.Health -= 10;
                        _multiPlayerOne.Invulnerable = true;

                    }
                }


                #endregion

                #region Sword - Zombie Collision

                if (_currentGameMode == GameMode.Singleplayer)
                {
                    if (z.BoundingBox.Intersects(_singlePlayerPlayer.Sword.BoundingBox) && z.Visible && _singlePlayerPlayer.Sword.Visible)
                    {
                        KillZombie(z, _zombieSpawnChecker);
                    }
                }
                else
                {
                    if (z.BoundingBox.Intersects(_multiPlayerOne.Sword.BoundingBox) && z.Visible && _multiPlayerOne.Sword.Visible)
                    {
                        KillZombie(z, _zombieSpawnChecker);
                    }
                }


                #endregion
            }

            #region Zombie - Zombie Collision
            /*
            for (var i = 0; i < _lZombies.Count; i++)
            {
                foreach (var z in _lZombies)
                {
                    if (_lZombies[i].BoundingBox.Intersects(z.BoundingBox) && z.Visible && _lZombies[i].Visible &&
                        _lZombies[i].Id != z.Id)
                    {

                    }
                }
            } 
            */
            #endregion

            #endregion

            #region Player Death & Game Over

            if (_currentGameMode == GameMode.Singleplayer)
            {
                if (_singlePlayerPlayer.Health <= 0 && !_singlePlayerPlayer.Dead)
                {
                    _singlePlayerPlayer.Dead = true;
                    _singlePlayerPlayer.Visible = false;
                    ScreenManager.AddScreen(new GameOverScreen(), ControllingPlayer);
                }
            }
            else
            {
                if (_multiPlayerOne.Health <= 0 && !_multiPlayerOne.Dead)
                {
                    _multiPlayerOne.Dead = true;
                    _multiPlayerOne.Visible = false;
                    ScreenManager.AddScreen(new GameOverScreen(), ControllingPlayer);
                }
            }

            #endregion

        }

        #region Methods

        /// <summary>
        /// Add zombies to the game
        /// </summary>
        private void AddZombies(int count)
        {
            Global.ZombieSpawnDelay = 0;
            for (var i = 0; i < count; i++)
            {
                _lZombies.Add(new Zombie(_howManyZombiesEachRound) { Id = i });
            }
            _diagZombieCount = count;
        }

        /// <summary>
        /// Draw fps, collision and other diagnostics, P to toggle
        /// </summary>
        private void DrawDiagnostics(SpriteBatch spriteBatch)
        {
            _frameCounter++;
            if (Global.IsDiagnosticsOpen)
            {
                spriteBatch.DrawString(_diagnosticFont, string.Format("Enemies on screen: {0}\nWave: {1}\nFPS: {2}", _diagZombieCount, _wave, _frameRate), new Vector2(20, 45), Color.Green);
                spriteBatch.DrawString(_diagnosticFont,
                      _singlePlayerPlayer.Invulnerable
                          ? string.Format("Player health: {0}\nCharacter Name: {1}\nState: {2}, Invulnerable",
                              _singlePlayerPlayer.Health, _singlePlayerPlayer.FullCharacterName, _singlePlayerPlayer.CurrentState)
                          : string.Format("Player health: {0}\nCharacter Name: {1}\nState: {2}", _singlePlayerPlayer.Health,
                              _singlePlayerPlayer.FullCharacterName, _singlePlayerPlayer.CurrentState), new Vector2(1500, 45), Color.Green);
            }

        }

        /// <summary>
        /// Kill the zombie safely
        /// </summary>
        private void KillZombie(Zombie z, int zombieSpawnChecker)
        {
            z.Visible = false;
            z.Dead = true;
            _deadZombies++;
            _diagZombieCount--;
            if (_deadZombies % 5 == 0)
            {
                foreach (var d in _lZombies)
                {
                    if (!d.Dead && !d.Visible && zombieSpawnChecker < Global.ZombieSpawnTicker)
                    {
                        d.Visible = true;
                        zombieSpawnChecker++;
                    }

                }
                Global.ZombieSpawnTicker++;
            }
            zombieSpawnChecker = 0;
        }

        /// <summary>
        /// Send name, position and angle
        /// </summary>
        public void SendInfoToServer()
        {
            if (Convert.ToInt32(_multiPlayerOne.SpritePosition.X) != _previousSpritePositionX ||
                    Convert.ToInt32(_multiPlayerOne.SpritePosition.Y) != _previousSpritePositionY || _multiPlayerOne.Angle != _previousAngle)
            {
                var om = _client.CreateMessage();
                om.Write(Global.PlayerName);
                om.Write(Convert.ToInt32(_multiPlayerOne.SpritePosition.X)); // very inefficient to send a full Int32 (4 bytes) but we'll use this for simplicity
                om.Write(Convert.ToInt32(_multiPlayerOne.SpritePosition.Y));
                om.Write(_multiPlayerOne.Angle);
                _client.SendMessage(om, NetDeliveryMethod.Unreliable);
                _previousSpritePositionX = Convert.ToInt32(_multiPlayerOne.SpritePosition.X);
                _previousSpritePositionY = Convert.ToInt32(_multiPlayerOne.SpritePosition.Y);
                _previousAngle = _multiPlayerOne.Angle;
            }
        }

        public void CheckZombieDeaths()
        {
            if (_deadZombies == _lZombies.Count)
            {
                _waveNotify = true;
                _deadZombies = 0;
                _zombieModifier++;
                _difficultyModifier++;
                _howManyZombiesEachRound = (int)(_howManyZombiesEachRound * HowManyZombieModifier);
                _lZombies.Clear();
                AddZombies(_zombieModifier * _difficultyModifier); //Add the next batch
                _wave++;

                foreach (var z in _lZombies)
                {
                    z.LoadContent(_content);
                }

                _bell.Play();
            }
        }

        /// <summary>
        /// Read name, position and angle
        /// </summary>
        private void ReadInfoFromServer()
        {
            NetIncomingMessage msg;
            while ((msg = _client.ReadMessage()) != null)
            {

                switch (msg.MessageType)
                {

                    case NetIncomingMessageType.Data:
                        var who = msg.ReadInt64();
                        var name = msg.ReadString();
                        var x = msg.ReadFloat();
                        var y = msg.ReadFloat();
                        var angle = msg.ReadFloat();
                        _playerNames[who] = name;
                        _playerAngles[who] = angle;
                        _playerPositions[who] = new Vector2(x, y);
                        break;
                    case NetIncomingMessageType.WarningMessage:

                        break;

                }
            }
        }

        public void SetDifficulty()
        {
            switch (Global.SelectedDifficulty)
            {
                case Global.Difficulty.Easy:
                    _zombieModifier = 1;
                    _difficultyModifier = 5;
                    break;
                case Global.Difficulty.Normal:
                    _zombieModifier = 2;
                    _difficultyModifier = 6;
                    break;
                case Global.Difficulty.Hard:
                    _zombieModifier = 3;
                    _difficultyModifier = 7;
                    break;
                case Global.Difficulty.Insane:
                    _zombieModifier = 5;
                    _difficultyModifier = 8;
                    break;
            }
        }

        public void InitializeMultiplayerPlayers(int howManyPlayers)
        {
            switch (howManyPlayers)
            {
                case 2:
                    _multiPlayerOne = new Player();
                    _multiPlayerTwo = new PlayerPuppet();
                    _otherPlayers.Add(_multiPlayerTwo);

                    break;
                case 3:
                    _multiPlayerOne = new Player();
                    _multiPlayerTwo = new PlayerPuppet();
                    _multiPlayerThree = new PlayerPuppet();
                    _otherPlayers.Add(_multiPlayerTwo);
                    _otherPlayers.Add(_multiPlayerThree);

                    break;
                case 4:
                    _multiPlayerOne = new Player();
                    _multiPlayerTwo = new PlayerPuppet();
                    _multiPlayerThree = new PlayerPuppet();
                    _multiPlayerFour = new PlayerPuppet();
                    _otherPlayers.Add(_multiPlayerTwo);
                    _otherPlayers.Add(_multiPlayerThree);
                    _otherPlayers.Add(_multiPlayerFour);
                    break;
            }
        }

        public void LoadMultiplayerPlayers(int howManyPlayers)
        {
            switch (howManyPlayers)
            {
                case 2:
                    _multiPlayerOne.LoadContent(_content);
                    _multiPlayerTwo.LoadContent(_content);
                    break;
                case 3:
                    _multiPlayerOne.LoadContent(_content);
                    _multiPlayerTwo.LoadContent(_content);
                    _multiPlayerThree.LoadContent(_content);
                    break;
                case 4:
                    _multiPlayerOne.LoadContent(_content);
                    _multiPlayerTwo.LoadContent(_content);
                    _multiPlayerThree.LoadContent(_content);
                    _multiPlayerFour.LoadContent(_content);
                    break;
            }
        }

        void GameInProgress_Exiting(object sender, EventArgs e)
        {
            if (_currentGameMode == GameMode.Multiplayer)
            {
                _client.Disconnect("Client shutdown");
                _client.Shutdown("Client shutdown");
            }
        }

        private void FadeOutSong(GameTime gameTime, float fadeDuration, float valueToMuteIn,
            float valueToFade)
        {
            if (MediaPlayer.Volume > valueToMuteIn && MediaPlayer.State == MediaState.Playing)
            {
                _songFadeCounterStart += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (_songFadeCounterStart >= fadeDuration)
                {
                    MediaPlayer.Volume -= valueToFade;
                    _songFadeCounterStart = 0;
                }

            }
            if (MediaPlayer.Volume < 0)
            {
                MediaPlayer.Stop();
            }
        }

        #endregion

    }
}
