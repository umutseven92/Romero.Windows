
#region using Statements

using System;
using System.CodeDom;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using OpenTK.Graphics.ES20;

#endregion

namespace Romero.Windows
{
    public class Player : Sprite
    {
        public List<Bullet> bullets = new List<Bullet>();

        ContentManager mContentManager;

        const string PlayerAssetName = "player";
        const int StartPositionX = 125;
        const int StartPositionY = 245;
        const int PlayerSpeed = 500;
        const int MoveUp = -1;
        const int MoveDown = 1;
        const int MoveLeft = -1;
        const int MoveRight = 1;
        const float SprintModifier = 1.5f;

        enum State
        {
            Running,
            Sprinting
        }

        State _currentState = State.Running;

        Vector2 _direction = Vector2.Zero;
        Vector2 _speed = Vector2.Zero;

        

        private bool gamepadConnected = Global.Gamepad;

        private KeyboardState _previousKeyboardState;
        private GamePadState _previousGamePadState;
        private MouseState _previousMouseState;

        public void LoadContent(ContentManager contentManager)
        {
            mContentManager = contentManager;

            foreach (var b in bullets)
            {
                b.LoadContent(contentManager);
            }

            SpritePosition = new Vector2(StartPositionX, StartPositionY);
            base.LoadContent(contentManager, PlayerAssetName);
            Source = new Rectangle(0, 0, 200, Source.Height);

        }

        public void Update(GameTime gameTime)
        {

            var currentKeyboardState = Keyboard.GetState();
            var currentGamepadState = GamePad.GetState(PlayerIndex.One);
            var currentMouseState = Mouse.GetState();

            UpdateMovement(currentKeyboardState, currentGamepadState);
            UpdateBullet(gameTime, currentMouseState, currentGamepadState);

            _previousMouseState = currentMouseState;
            _previousKeyboardState = currentKeyboardState;
            _previousGamePadState = currentGamepadState;

            base.Update(gameTime, _speed, _direction);
        }

        private void UpdateBullet(GameTime gameTime, MouseState currentMouseState, GamePadState currentGamePadState)
        {
            foreach (var b in bullets)
            {
                b.Update(gameTime);
            }
            if (!gamepadConnected)
            {
                if (currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton != ButtonState.Pressed)
                {
                    Shoot(currentMouseState);
                }

            }
            else
            {
                if (currentGamePadState.IsButtonDown(Buttons.RightShoulder) && !_previousGamePadState.IsButtonDown(Buttons.RightShoulder))
                {
                    Shoot(currentGamePadState);
                }
            }

        }



        private void Shoot(MouseState currentMouseState)
        {
            var mousePos = new Vector2(currentMouseState.X, currentMouseState.Y);
            var movement = mousePos - SpritePosition;

            if (movement != Vector2.Zero)
            {
                movement.Normalize();
            }

            var aFireball = new Bullet();
            aFireball.LoadContent(mContentManager);
            aFireball.Fire(SpritePosition
               , movement);
            bullets.Add(aFireball);

        }

        private void Shoot(GamePadState currentGamePadState)
        {
            //Gamepad shooting
        }


        public override void Draw(SpriteBatch theSpriteBatch)
        {
            foreach (var b in bullets)
            {
                b.Draw(theSpriteBatch);
            }
            base.Draw(theSpriteBatch);
        }

        private void UpdateMovement(KeyboardState currentKeyboardState, GamePadState currentGamePadState)
        {


            switch (Global.Gamepad)
            {
                #region Gamepad
                case true:
                    if (currentGamePadState.IsButtonDown(Buttons.A))
                    {
                        _currentState = State.Sprinting;
                    }
                    else
                    {
                        _currentState = State.Running;
                    }

                    switch (_currentState)
                    {
                        #region Running
                        case State.Running:
                            _speed = Vector2.Zero;
                            _direction = Vector2.Zero;
                            if (currentGamePadState.ThumbSticks.Left.X <= -0.3)
                            {
                                _speed.X = PlayerSpeed;
                                _direction.X = MoveLeft;
                            }

                            else if (currentGamePadState.ThumbSticks.Left.X >= 0.3)
                            {
                                _speed.X = PlayerSpeed;
                                _direction.X = MoveRight;
                            }

                            if (currentGamePadState.ThumbSticks.Left.Y >= 0.3)
                            {
                                _speed.Y = PlayerSpeed;
                                _direction.Y = MoveUp;
                            }

                            else if (currentGamePadState.ThumbSticks.Left.Y <= -0.3)
                            {
                                _speed.Y = PlayerSpeed;
                                _direction.Y = MoveDown;
                            }
                            break;
                        #endregion
                        #region Sprinting
                        case State.Sprinting:
                            _speed = Vector2.Zero;
                            _direction = Vector2.Zero;
                            if (currentGamePadState.ThumbSticks.Left.X <= -0.3)
                            {
                                _speed.X = PlayerSpeed * SprintModifier;
                                _direction.X = MoveLeft;
                            }

                            else if (currentGamePadState.ThumbSticks.Left.X >= 0.3)
                            {
                                _speed.X = PlayerSpeed * SprintModifier;
                                _direction.X = MoveRight;
                            }

                            if (currentGamePadState.ThumbSticks.Left.Y >= 0.3)
                            {
                                _speed.Y = PlayerSpeed * SprintModifier;
                                _direction.Y = MoveUp;
                            }

                            else if (currentGamePadState.ThumbSticks.Left.Y <= -0.3)
                            {
                                _speed.Y = PlayerSpeed * SprintModifier;
                                _direction.Y = MoveDown;
                            }
                            break;
                        #endregion
                    }

                    break;
                #endregion
                #region Keyboard
                case false:
                    if (currentKeyboardState.IsKeyDown(Keys.LeftShift))
                    {
                        _currentState = State.Sprinting;
                    }
                    else
                    {
                        _currentState = State.Running;
                    }

                    switch (_currentState)
                    {
                        #region Running
                        case State.Running:
                            _speed = Vector2.Zero;
                            _direction = Vector2.Zero;
                            if (currentKeyboardState.IsKeyDown(Keys.A))
                            {
                                _speed.X = PlayerSpeed;
                                _direction.X = MoveLeft;
                            }
                            else if (currentKeyboardState.IsKeyDown(Keys.D))
                            {
                                _speed.X = PlayerSpeed;
                                _direction.X = MoveRight;
                            }
                            if (currentKeyboardState.IsKeyDown(Keys.W))
                            {
                                _speed.Y = PlayerSpeed;
                                _direction.Y = MoveUp;
                            }
                            else if (currentKeyboardState.IsKeyDown(Keys.S))
                            {
                                _speed.Y = PlayerSpeed;
                                _direction.Y = MoveDown;
                            }
                            break;
                        #endregion
                        #region Sprinting
                        case State.Sprinting:
                            _speed = Vector2.Zero;
                            _direction = Vector2.Zero;
                            if (currentKeyboardState.IsKeyDown(Keys.A))
                            {
                                _speed.X = PlayerSpeed * SprintModifier;
                                _direction.X = MoveLeft;
                            }
                            else if (currentKeyboardState.IsKeyDown(Keys.D))
                            {
                                _speed.X = PlayerSpeed * SprintModifier;
                                _direction.X = MoveRight;
                            }
                            if (currentKeyboardState.IsKeyDown(Keys.W))
                            {
                                _speed.Y = PlayerSpeed * SprintModifier;
                                _direction.Y = MoveUp;
                            }
                            else if (currentKeyboardState.IsKeyDown(Keys.S))
                            {
                                _speed.Y = PlayerSpeed * SprintModifier;
                                _direction.Y = MoveDown;
                            }
                            break;
                        #endregion
                    }
                    break;

                #endregion
            }


        }
    }
}
