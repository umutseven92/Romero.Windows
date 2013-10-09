#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Romero.Windows;
using Romero.Windows.ScreenManager;

#endregion

namespace WindowsGSM1
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {

        Vector2 playerPosition = new Vector2(100, 100);
        ContentManager content;
        SpriteFont gameFont;
        private Texture2D playerTexture2D;
        //private Texture2D playerTexture2D;
        private Player _player;
        float pauseAlpha;



        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
          _player = new Player();
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            gameFont = content.Load<SpriteFont>("font");
            playerTexture2D = content.Load<Texture2D>("player");
            _player.LoadContent(content);
            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
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
            content.Unload();
        }





        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
          _player.Update(gameTime);
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

          

        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            //// Look up inputs for the active player profile.
            //var playerIndex = (int)ControllingPlayer.Value;


            //var keyboardState = input.CurrentKeyboardStates[playerIndex];
            //GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];
            // //The game pauses either if the user presses the pause button, or if
            // //they unplug the active gamepad. This requires us to keep track of
            // //whether a gamepad was ever plugged in, because we don't want to pause
            // //on PC if they are playing with a keyboard and have no gamepad at all!
            //bool gamePadDisconnected = !gamePadState.IsConnected &&
            //                           input.GamePadWasConnected[playerIndex];

            //if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected)
            //{
            //    ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            //}
            //else
            //{ 
               
            //    // Otherwise move the player position.
            //    var movement = Vector2.Zero;

            //    if (keyboardState.IsKeyDown(Keys.Left))
            //        movement.X--;

            //    if (keyboardState.IsKeyDown(Keys.Right))
            //        movement.X++;

            //    if (keyboardState.IsKeyDown(Keys.Up))
            //        movement.Y--;

            //    if (keyboardState.IsKeyDown(Keys.Down))
            //        movement.Y++;

            //    Vector2 thumbstick = gamePadState.ThumbSticks.Left;

            //    movement.X += thumbstick.X;
            //    movement.Y -= thumbstick.Y;

            //    if (movement.Length() > 1)
            //        movement.Normalize();

            //    playerPosition += movement * 2;
            //}

           

        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.CornflowerBlue, 0, 0);

            // Our player and enemy are both actually just text strings.
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();
           //spriteBatch.Draw(playerTexture2D,playerPosition,Color.White);
            _player.Draw(spriteBatch);
            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }


    }
}
