#region Using Statements

using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Romero.Windows.Classes;
using Romero.Windows.ScreenManager;

#endregion

namespace Romero.Windows.Screens
{
    /// <summary>
    /// This screen implements the actual game logic.
    /// </summary>
    class GameplayScreen : GameScreen
    {

        #region Declarations
        ContentManager _content;
        private readonly Player _player; //Player, for single player
        private GameTime _gT; //Gametime for Player.Update()
        private readonly List<Zombie> _lZombies; //Zombies on screen
        private SpriteFont _font;
        TimeSpan _elapsedTime = TimeSpan.Zero;
        float _pauseAlpha;
        private int _deadZombies;
        private int _zombieModifier; //Zombie base spawn rate, increases with higher difficulty
        private int _difficultyModifier; //Difficulty rate,  increases with higher difficulty
        private int _diagZombieCount; //Diagnostics - zombie count
        private int _wave;
        int _frameRate;
        int _frameCounter;
        Texture2D _backgroundTexture;
        private KeyboardState _previousKeyboardState;
        int _zombieSpawnChecker;
        readonly Rectangle _bgRectangle = new Rectangle(0, 0, Global.DeviceInUse.GraphicsDevice.PresentationParameters.BackBufferWidth, Global.DeviceInUse.GraphicsDevice.PresentationParameters.BackBufferHeight);
        #endregion

        #region Functions

        /// <summary>
        /// Add zombies to the game
        /// </summary>
        private void AddZombies(int count)
        {
            Global.ZombieSpawnDelay = 0;
            for (var i = 0; i < count; i++)
            {
                _lZombies.Add(new Zombie { Id = i });
            }
            _diagZombieCount = count;
        }

        /// <summary>
        /// Draw fps, collision and other diagnostics, press P to toggle. Make this inaccessible before release
        /// </summary>
        private void DrawDiagnostics(SpriteBatch spriteBatch)
        {
            _frameCounter++;
            if (Global.IsDiagnosticsOpen)
            {
                spriteBatch.DrawString(_font, string.Format("Enemies on screen: {0}\nWave: {1}\nFPS: {2}", _diagZombieCount, _wave, _frameRate), new Vector2(20, 45), Color.Green);
                spriteBatch.DrawString(_font,
                      _player.Invulnerable
                          ? string.Format("Player health: {0}\nCharacter Name: {1}\nState: {2}, Invulnerable",
                              _player.Health, _player.FullCharacterName, _player.CurrentState)
                          : string.Format("Player health: {0}\nCharacter Name: {1}\nState: {2}", _player.Health,
                              _player.FullCharacterName, _player.CurrentState), new Vector2(1500, 45), Color.Green);
            }

        }

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);


            //Set difficulty
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

            //Player (single player)
            _player = new Player();

            //camera = new Camera2D(Global.GameInProgress) {Focus = _player};

            //Zombie Horde
            _lZombies = new List<Zombie>();
            _wave = 1;
            AddZombies(_zombieModifier * _difficultyModifier);
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content/Sprites");

            _backgroundTexture = _content.Load<Texture2D>("groundTile");
            _player.LoadContent(_content);

            foreach (var z in _lZombies)
            {
                z.LoadContent(_content);
            }

            _font = _content.Load<SpriteFont>("font");

            //Screen Delay
            Thread.Sleep(1000);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
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

            //All zombies on screen dead
            if (_deadZombies == _lZombies.Count)
            {
                _deadZombies = 0;
                _zombieModifier++;
                _difficultyModifier++;

                _lZombies.Clear();
                AddZombies(_zombieModifier * _difficultyModifier);
                _wave++;
                foreach (var z in _lZombies)
                {
                    z.LoadContent(_content);

                }
            }

            //To update player
            _gT = gameTime;

            #region FPS calculation

            _elapsedTime += gameTime.ElapsedGameTime;

            if (_elapsedTime > TimeSpan.FromSeconds(1))
            {
                _elapsedTime -= TimeSpan.FromSeconds(1);
                _frameRate = _frameCounter;
                _frameCounter = 0;
            }

            #endregion

            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            _pauseAlpha = coveredByOtherScreen ? Math.Min(_pauseAlpha + 1f / 32, 1) : Math.Max(_pauseAlpha - 1f / 32, 0);

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

            //Kill all zombies
            if (currentKeyboardState.IsKeyDown(Keybinds.DeveloperKillAll) && !_previousKeyboardState.IsKeyDown(Keybinds.DeveloperKillAll))
            {
                foreach (var z in _lZombies)
                {
                    if (!z.Dead && z.Visible)
                    {
                        z.Visible = false;
                        z.Dead = true;
                        _deadZombies++;
                        _diagZombieCount--;
                        if (_deadZombies % Global.ZombieSpawnTicker == 0)
                        {
                            foreach (var d in _lZombies)
                            {
                                if (!d.Dead && !d.Visible && _zombieSpawnChecker < Global.ZombieSpawnTicker)
                                {
                                    d.Visible = true;
                                    _zombieSpawnChecker++;
                                }

                            }

                        }
                        _zombieSpawnChecker = 0;
                    }


                }
            }

            _previousKeyboardState = currentKeyboardState;

            if (input.IsPauseGame(ControllingPlayer))
            {
                // Pause (esc)
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }

            else
            {
                // Player movement
                _player.Update(_gT);
            }

            foreach (var z in _lZombies)
            {
                //Zombie movement
                z.Update(_gT, _player);
            }

            #region Collision



            foreach (var z in _lZombies)
            {

                foreach (var b in _player.Bullets)
                {
                    if (z.BoundingBox.Intersects(b.BoundingBox) && z.Visible && b.Visible)
                    {
                        //Bullet - Zombie Collision
                        b.Visible = false;

                        KillZombie(z, _zombieSpawnChecker);
                    }
                }
                if (z.BoundingBox.Intersects(_player.BoundingBox) && z.Visible && _player.CurrentState != Player.State.Dodging && !_player.Invulnerable)
                {
                    //Zombie-Player Collision
                    _player.Health -= 10;
                    _player.Invulnerable = true;

                }
                if (z.BoundingBox.Intersects(_player.Sword.BoundingBox) && z.Visible && _player.Sword.Visible)
                {
                    //Sword - Zombie Collision
                    //_player.Sword.Visible = false;

                    KillZombie(z, _zombieSpawnChecker);
                }

            }
            /*
                        for (var i = 0; i < _lZombies.Count; i++)
                        {
                            foreach (var z in _lZombies)
                            {
                                if (_lZombies[i].BoundingBox.Intersects(z.BoundingBox) && z.Visible && _lZombies[i].Visible &&
                                    _lZombies[i].Id != z.Id)
                                {
                                    //Zombie - Zombie Collision

                                }
                            }
                        }
            */
            #endregion

            if (_player.Health <= 0 && !_player.Dead)
            {
                //DEATH!
                _player.Dead = true;
                _player.Visible = false;
                ScreenManager.AddScreen(new GameOverScreen(), ControllingPlayer);
            }

        }

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
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // Background
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.CornflowerBlue, 0, 0);

            var spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.Opaque, SamplerState.LinearWrap,
    DepthStencilState.Default, RasterizerState.CullNone);

            // spriteBatch.Draw(_backgroundTexture, screenRectangle, Color.White); --> Non tiled background

            spriteBatch.Draw(_backgroundTexture, Vector2.Zero, _bgRectangle, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.End();

            spriteBatch.Begin();
            _player.Draw(spriteBatch);

            foreach (var z in _lZombies)
            {
                var direction = z.SpritePosition - _player.SpritePosition;
                var angle = (float)(Math.Atan2(direction.Y, direction.X) + Math.PI / 2 + Math.PI);

                z.Draw(spriteBatch, angle);
            }

            DrawDiagnostics(spriteBatch);

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || _pauseAlpha > 0)
            {
                var alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, _pauseAlpha / 2);
                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }


    }
}
