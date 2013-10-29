#region Using Statements

using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
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
        private readonly Player _player;
        float _pauseAlpha;
        private GameTime _gT;
        private readonly List<Zombie> _lZombies;
        private int _deadZombies = 0;
        private int _zombieModifier;
        #endregion



        #region Functions

        /// <summary>
        /// Add zombies to the game randomly.
        /// </summary>
        private void AddZombies(int count)
        {
            for (var i = 0; i < count; i++)
            {
                _lZombies.Add(new Zombie { Id = i });
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

            //Difficulty
            switch (Global.SelectedDifficulty)
            {
                case Global.Difficulty.Easy:
                    _zombieModifier = 1;
                    break;
                case Global.Difficulty.Normal:
                    _zombieModifier = 2;
                    break;
                case Global.Difficulty.Hard:
                    _zombieModifier = 3;
                    break;
                case Global.Difficulty.Insane:
                    _zombieModifier = 5;
                    break;
            }

            //Main Player
            _player = new Player();

            //Zombie Horde
            _lZombies = new List<Zombie>();
            AddZombies(_zombieModifier * 5);
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            _player.LoadContent(_content);

            foreach (var z in _lZombies)
            {
                z.LoadContent(_content);
            }

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
            if (_deadZombies == _lZombies.Count)
            {
                _deadZombies = 0;
                _zombieModifier++;

                _lZombies.Clear();
                AddZombies(_zombieModifier * 5);

                foreach (var z in _lZombies)
                {
                    z.LoadContent(_content);
                }
            }

            //For Player update
            _gT = gameTime;


            #region Collision

            foreach (var z in _lZombies)
            {

                foreach (var b in _player.Bullets)
                {
                    if (z.BoundingBox.Intersects(b.BoundingBox) && z.Visible && b.Visible)
                    {
                        //Bullet - Zombie Collision
                        b.Visible = false;

                        z.Visible = false;
                        _deadZombies++;

                    }
                }
                if (z.BoundingBox.Intersects(_player.BoundingBox) && z.Visible && _player.CurrentState != Player.State.Dodging)
                {
                    //Zombie-Player Collision

                }

            }

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

            if (input.IsPauseGame(ControllingPlayer))
            {
                // Pause
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
                // Player movement
                _player.Update(_gT);
            }
            foreach (var z in _lZombies)
            {
                z.Update(_gT, _player);
            }
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

            spriteBatch.Begin();

            _player.Draw(spriteBatch);

            foreach (var z in _lZombies)
            {
                z.Draw(spriteBatch);

            }
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
