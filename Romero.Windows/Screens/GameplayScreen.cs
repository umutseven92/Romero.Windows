#region Using Statements

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        private readonly Player _player;
        private GameTime _gT;
        private readonly List<Zombie> _lZombies;
        private SpriteFont _font;
        TimeSpan _elapsedTime = TimeSpan.Zero;
        float _pauseAlpha;
        readonly Rectangle _bgRectangle = new Rectangle(0, 0, 4096, 4096);
        private readonly Camera2D _cam;
        Texture2D _backgroundTexture;
        private KeyboardState _previousKeyboardState;
       
        #region Zombie Values
        private int _deadZombies;
        private int _zombieModifier; //Zombie base spawn rate, increases with higher difficulty
        private int _difficultyModifier; //Difficulty rate,  increases with higher difficulty
        private int _diagZombieCount; //Diagnostics - zombie count
        private int _wave;
        int _zombieSpawnChecker;
        #endregion

        #region FPS counter
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

        #endregion

        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            #region Difficulty

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

            #endregion

            _player = new Player();
            _cam = new Camera2D(new Viewport(0, 0, Global.DeviceInUse.GraphicsDevice.PresentationParameters.BackBufferWidth, Global.DeviceInUse.GraphicsDevice.PresentationParameters.BackBufferHeight), _player);
            _player.GetCamera(_cam);
            
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
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            _backgroundTexture = _content.Load<Texture2D>("Sprites/ground");
            _player.LoadContent(_content);

            foreach (var z in _lZombies)
            {
                z.LoadContent(_content);
            }
           
            _font = _content.Load<SpriteFont>("Fonts/font");
            
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
            //All zombies in the wave are dead
            if (_deadZombies == _lZombies.Count)
            {
                _deadZombies = 0;
                _zombieModifier++;
                _difficultyModifier++;

                _lZombies.Clear();
                AddZombies(_zombieModifier * _difficultyModifier); //Add the next batch
                _wave++;
                foreach (var z in _lZombies)
                {
                    z.LoadContent(_content);
                }
            }

            _gT = gameTime; //To update the player
           
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

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, _cam.Transform);

            spriteBatch.Draw(_backgroundTexture, _bgRectangle, Color.White);
            foreach (var z in _lZombies)
            {
                var direction = z.SpritePosition - _player.SpritePosition;
                var angle = (float)(Math.Atan2(direction.Y, direction.X) + Math.PI / 2 + Math.PI);

                z.Draw(spriteBatch, angle);
            }
            _player.Draw(spriteBatch);

            spriteBatch.End();

            spriteBatch.Begin();

            DrawDiagnostics(spriteBatch);

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || _pauseAlpha > 0)
            {
                var alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, _pauseAlpha / 2);
                ScreenManager.FadeBackBufferToBlack(alpha);
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
                _player.Update(_gT, _cam); // Player movement
            }

            foreach (var z in _lZombies)
            {

                z.Update(_gT, _player); //Zombie movement
            }

            #region Collision

            foreach (var z in _lZombies)
            {
                #region Bullet - Zombie Collision
                foreach (var b in _player.Bullets)
                {
                    if (z.BoundingBox.Intersects(b.BoundingBox) && z.Visible && b.Visible)
                    {

                        b.Visible = false;
                        KillZombie(z, _zombieSpawnChecker);

                    }
                }
                #endregion

                #region Zombie - Player Collision
                if (z.BoundingBox.Intersects(_player.BoundingBox) && z.Visible && _player.CurrentState != Player.State.Dodging && !_player.Invulnerable)
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

                    _player.Health -= 10;
                    _player.Invulnerable = true;

                }
                #endregion

                #region Sword - Zombie Collision
                if (z.BoundingBox.Intersects(_player.Sword.BoundingBox) && z.Visible && _player.Sword.Visible)
                {

                    KillZombie(z, _zombieSpawnChecker);

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
            if (_player.Health <= 0 && !_player.Dead)
            {
                _player.Dead = true;
                _player.Visible = false;
                ScreenManager.AddScreen(new GameOverScreen(), ControllingPlayer);
            }
            #endregion

        }

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
        /// Draw fps, collision and other diagnostics, P to toggle
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



    }
}
