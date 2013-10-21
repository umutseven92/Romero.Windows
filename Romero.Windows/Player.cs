#region using Statements

using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

#endregion

namespace Romero.Windows
{
    public class Player : Sprite
    {
        #region Declarations

        public List<Bullet> Bullets = new List<Bullet>();
        ContentManager _contentManager;
        const string PlayerAssetName = "player";
        const int StartPositionX = 125;
        const int StartPositionY = 245;
        const int PlayerSpeed = 500;
        const int MoveUp = -1;
        const int MoveDown = 1;
        const int MoveLeft = -1;
        const int MoveRight = 1;
        const float DodgeModifier = 5.5f;
        private const float SprintModifier = 2.5f;

        public enum State
        {
            Running,
            Dodging,
            Sprinting
        }

        public State CurrentState = State.Running;
        Vector2 _direction = Vector2.Zero;
        Vector2 _speed = Vector2.Zero;
        private GamePadState _previousGamePadState;
        private MouseState _previousMouseState;
        private KeyboardState _previouseKeyboardState;

        #endregion

        public void LoadContent(ContentManager contentManager)
        {
            _contentManager = contentManager;

            foreach (var b in Bullets)
            {
                b.LoadContent(contentManager);
            }

            SpritePosition = new Vector2(StartPositionX, StartPositionY);
            LoadContent(contentManager, PlayerAssetName);
            Source = new Rectangle(0, 0, 200, Source.Height);

        }

        public void Update(GameTime gameTime)
        {
            var currentKeyboardState = Keyboard.GetState();
            var currentGamepadState = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular);
            var currentMouseState = Mouse.GetState();

            UpdateMovement(currentKeyboardState, currentGamepadState);
            UpdateBullet(gameTime, currentMouseState, currentGamepadState);

            _previousMouseState = currentMouseState;
            _previousGamePadState = currentGamepadState;
            _previouseKeyboardState = currentKeyboardState;

            Update(gameTime, _speed, _direction);
        }

        private void UpdateBullet(GameTime gameTime, MouseState currentMouseState, GamePadState currentGamePadState)
        {
            foreach (var b in Bullets)
            {
                b.Update(gameTime);
            }

            //Mouse shooting
            if (!Global.Gamepad)
            {
                if (currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton != ButtonState.Pressed)
                {
                    Shoot(currentMouseState);
                }

            }

            //Gamepad shooting
            else
            {
                if (currentGamePadState.IsButtonDown(Buttons.RightShoulder) && !_previousGamePadState.IsButtonDown(Buttons.RightShoulder))
                {
                    Shoot(currentGamePadState);
                }
            }

        }

        /// <summary>
        /// Mouse shooting
        /// </summary>
        private void Shoot(MouseState currentMouseState)
        {
            var mousePos = new Vector2(currentMouseState.X, currentMouseState.Y);
            var movement = mousePos - SpritePosition;

            if (movement != Vector2.Zero)
            {
                movement.Normalize();
            }

            var bullet = new Bullet();
            bullet.LoadContent(_contentManager);
            bullet.Fire(SpritePosition, movement);
            Bullets.Add(bullet);

        }

        /// <summary>
        /// Gamepad shooting
        /// </summary>
        private void Shoot(GamePadState currentGamePadState)
        {
            var thumb = new Vector2(currentGamePadState.ThumbSticks.Right.X, -currentGamePadState.ThumbSticks.Right.Y);

            if (thumb != Vector2.Zero)
            {
                var bullet = new Bullet();
                bullet.LoadContent(_contentManager);
                bullet.Fire(SpritePosition, thumb);
                Bullets.Add(bullet);
            }

        }


        public override void Draw(SpriteBatch theSpriteBatch)
        {
            foreach (var b in Bullets)
            {
                b.Draw(theSpriteBatch);
            }
            base.Draw(theSpriteBatch);
        }

        private void UpdateMovement(KeyboardState currentKeyboardState, GamePadState currentGamePadState)
        {
            switch (Global.Gamepad)
            {
                #region Gamepad Controls

                case true:
                    CurrentState = State.Running;
                    
                    if (currentGamePadState.IsButtonDown(Buttons.LeftStick) && !_previousGamePadState.IsButtonDown(Buttons.LeftStick))
                    {
                        CurrentState = State.Dodging;
                    }
                    
                    if (currentGamePadState.IsButtonDown(Buttons.LeftShoulder))
                    {
                        CurrentState = State.Sprinting;
                    }

                    switch (CurrentState)
                    {
                        #region Gamepad Running

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

                        #region Gamepad Dodging

                        case State.Dodging:

                            _speed = Vector2.Zero;
                            _direction = Vector2.Zero;

                            if (currentGamePadState.ThumbSticks.Left.X <= -0.3)
                            {
                                _speed.X = PlayerSpeed * DodgeModifier;
                                _direction.X = MoveLeft;
                            }

                            else if (currentGamePadState.ThumbSticks.Left.X >= 0.3)
                            {
                                _speed.X = PlayerSpeed * DodgeModifier;
                                _direction.X = MoveRight;
                            }

                            if (currentGamePadState.ThumbSticks.Left.Y >= 0.3)
                            {
                                _speed.Y = PlayerSpeed * DodgeModifier;
                                _direction.Y = MoveUp;
                            }

                            else if (currentGamePadState.ThumbSticks.Left.Y <= -0.3)
                            {
                                _speed.Y = PlayerSpeed * DodgeModifier;
                                _direction.Y = MoveDown;
                            }
                            break;

                        #endregion

                        #region Gamepad Sprinting
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

                #region Keyboard Controls

                case false:

                    CurrentState = State.Running;

                    if (currentKeyboardState.IsKeyDown(Keys.Space) && !_previouseKeyboardState.IsKeyDown(Keys.Space))
                    {
                        CurrentState = State.Dodging;

                    }
                    
                    if (currentKeyboardState.IsKeyDown(Keys.LeftShift))
                    {
                        CurrentState = State.Sprinting;
                    }


                    switch (CurrentState)
                    {

                        #region Keyboard Running

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

                        #region Keyboard Dodging

                        case State.Dodging:

                            _speed = Vector2.Zero;
                            _direction = Vector2.Zero;

                            if (currentKeyboardState.IsKeyDown(Keys.A))
                            {
                                _speed.X = PlayerSpeed * DodgeModifier;
                                _direction.X = MoveLeft;
                            }

                            else if (currentKeyboardState.IsKeyDown(Keys.D))
                            {
                                _speed.X = PlayerSpeed * DodgeModifier;
                                _direction.X = MoveRight;
                            }

                            if (currentKeyboardState.IsKeyDown(Keys.W))
                            {
                                _speed.Y = PlayerSpeed * DodgeModifier;
                                _direction.Y = MoveUp;
                            }

                            else if (currentKeyboardState.IsKeyDown(Keys.S))
                            {
                                _speed.Y = PlayerSpeed * DodgeModifier;
                                _direction.Y = MoveDown;
                            }
                            break;

                        #endregion

                        #region Keyboard Sprinting
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
