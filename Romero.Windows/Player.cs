using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Romero.Windows
{
    class Player : Sprite
    {
        const string PlayerAssetName = "player";
        const int StartPositionX = 125;
        const int StartPositionY = 245;
        const int PlayerSpeed = 300;
        const int MoveUp = -1;
        const int MoveDown = 1;
        const int MoveLeft = -1;
        const int MoveRight = 1;
        const float SprintModifier = 2.0f;

        enum State
        {
            Running,
            Sprinting
        }

        State _currentState = State.Running;

        Vector2 _direction = Vector2.Zero;
        Vector2 _speed = Vector2.Zero;

        private bool gamepadConnected = true;
        KeyboardState _previousKeyboardState;
        private GamePadState _previousGamePadState;

        public void LoadContent(ContentManager contentManager)
        {
            SpritePosition = new Vector2(StartPositionX, StartPositionY);
            LoadContent(contentManager, PlayerAssetName);
        }

        public void Update(GameTime gameTime)
        {

            var currentKeyboardState = Keyboard.GetState();
            var currentGamepadState = GamePad.GetState(PlayerIndex.One);

            UpdateMovement(currentKeyboardState, currentGamepadState);

            _previousKeyboardState = currentKeyboardState;
            _previousGamePadState = currentGamepadState;

            Update(gameTime, _speed, _direction);
        }

        private void UpdateMovement(KeyboardState currentKeyboardState, GamePadState currentGamePadState)
        {


            switch (gamepadConnected)
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
