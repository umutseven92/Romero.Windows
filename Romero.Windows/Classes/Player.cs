#region using Statements

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Romero.Windows.Classes
{
    /// <summary>
    /// The main player
    /// </summary>
    public class Player : Sprite
    {
        #region Declarations

        public List<Bullet> Bullets = new List<Bullet>();
        public Sword Sword = new Sword();
        ContentManager _contentManager;
        private GameTime _gT;
        private Camera2D _cam;

        #region Player Modifiers
        public string PlayerAssetName;
        private readonly int _playerSpeed;
        private readonly float _dodgeModifier;
        private readonly bool _canDodge;
        public string FullCharacterName;
        internal int Health;
        #endregion

        const int StartPositionX = 2048;
        const int StartPositionY = 2048;

        const int MoveUp = -1;
        const int MoveDown = 1;
        const int MoveLeft = -1;
        const int MoveRight = 1;

        private const float SprintModifier = 2.0f;
        internal bool Dead = false;
        float _playerAngle;
        public bool Visible = true;

        #region Invulnerability
        internal bool Invulnerable = false;
        const int InvulnTime = 2;
        float _invulnCounterStart;
        #endregion

        #region Shooting
        private bool _canShoot = true;
        private readonly float _shootDelay;
        private float _shootCounterStart;
        #endregion

        #region Blinking
        private float _blinkCounterStart;
        private const float BlinkDelay = 0.1f;
        #endregion

        #region Sprinting
        private bool _canSprint = true;
        private readonly float _sprintTime;
        private readonly float _sprintDelay;
        private float _sprintCounterStart;
        private float _sprintDelayCounterStart;
        #endregion

        #region Swinging
        private bool _canSwing = true;
        private readonly float _swingDelay;
        private float _swingCounterStart;
        #endregion

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

        public Player()
        {
            switch (Global.SelectedCharacter)
            {
                #region Knight Fraser
                case Global.Character.Fraser:
                    PlayerAssetName = "fraser";
                    FullCharacterName = "Knight Fraser";
                    _playerSpeed = 300;
                    _shootDelay = 1;
                    Health = 300;
                    _canDodge = false;
                    _dodgeModifier = 0f;
                    _sprintTime = 3;
                    _sprintDelay = 5;
                    _swingDelay = 1;
                    break;
                #endregion

                #region Lady Rebecca
                case Global.Character.Becky:
                    PlayerAssetName = "becky";
                    FullCharacterName = "Lady Rebecca";
                    _playerSpeed = 450;
                    _shootDelay = 0.8f;
                    Health = 120;
                    _canDodge = true;
                    _dodgeModifier = 50f;
                    _sprintTime = 7;
                    _sprintDelay = 4;
                    _swingDelay = 0.5f;
                    break;
                #endregion

                #region Sire Benjamin
                case Global.Character.Ben:
                    PlayerAssetName = "ben";
                    FullCharacterName = "Sire Benjamin";
                    _playerSpeed = 340;
                    _shootDelay = 0.5f;
                    Health = 150;
                    _canDodge = false;
                    _dodgeModifier = 0f;
                    _sprintTime = 5;
                    _sprintDelay = 4;
                    _swingDelay = 0.7f;
                    break;
                #endregion

                #region Cleric Diakanos
                case Global.Character.Deacon:
                    PlayerAssetName = "deacon";
                    FullCharacterName = "Cleric Diakonos";
                    _playerSpeed = 400;
                    _shootDelay = 1;
                    Health = 100;
                    _canDodge = false;
                    _dodgeModifier = 0f;
                    _sprintTime = 4;
                    _sprintDelay = 4;
                    _swingDelay = 1;
                    break;
                #endregion
            }

        }

        public void LoadContent(ContentManager contentManager)
        {
            _contentManager = contentManager;

            foreach (var b in Bullets)
            {
                b.LoadContent(contentManager);
            }

            Sword.LoadContent(_contentManager);
            SpritePosition = new Vector2(StartPositionX, StartPositionY);
            LoadContent(contentManager, PlayerAssetName);
            Source = new Rectangle(0, 0, 200, Source.Height);

        }

        public void Update(GameTime gameTime, Camera2D camera)
        {
            _cam = camera;
            _gT = gameTime;

            var currentKeyboardState = Keyboard.GetState();
            var currentGamepadState = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular);
            var currentMouseState = Mouse.GetState();

            StayInScreen();

            UpdateMovement(currentKeyboardState, currentGamepadState);
            UpdateBullet(gameTime, currentMouseState, currentGamepadState);
            UpdateSword(gameTime, currentMouseState, currentGamepadState);

            _previousMouseState = currentMouseState;
            _previousGamePadState = currentGamepadState;
            _previouseKeyboardState = currentKeyboardState;

            CheckTimers(gameTime);

            Update(gameTime, _speed, _direction);
        }

        /// <summary>
        /// Set timer values
        /// </summary>
        private void CheckTimers(GameTime gameTime)
        {
            #region Invulnerability & Blink
            if (Invulnerable)
            {
                Blink(gameTime);
                _invulnCounterStart += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_invulnCounterStart >= InvulnTime)
                {
                    Visible = true;
                    Invulnerable = false;
                    _invulnCounterStart = 0f;
                }

            }
            #endregion

            #region Shooting
            if (!_canShoot)
            {
                _shootCounterStart += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_shootCounterStart >= _shootDelay)
                {
                    _canShoot = true;
                    _shootCounterStart = 0f;
                }
            }
            #endregion

            #region Swinging
            if (!_canSwing)
            {
                _swingCounterStart += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_swingCounterStart >= _swingDelay)
                {
                    _canSwing = true;
                    _swingCounterStart = 0f;
                }
            }
            #endregion

            #region Sprinting
            if (!_canSprint)
            {
                _sprintDelayCounterStart += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_sprintDelayCounterStart >= _sprintDelay)
                {
                    _sprintDelayCounterStart = 0;
                    _canSprint = true;
                }
            }
            #endregion
        }


        /// <summary>
        /// Check if character is out of screen
        /// </summary>
        private void StayInScreen()
        {
            if (SpritePosition.X < 0)
            {
                SpritePosition.X = 0;
            }
            else if (SpritePosition.X >= 4096)
            {
                SpritePosition.X = 4096 - Size.X;
            }

            if (SpritePosition.Y <= 0)
            {
                SpritePosition.Y = 0;
            }
            else if (SpritePosition.Y >= 4096)
            {
                SpritePosition.Y = 4096 - Size.Y;
            }
        }


        /// <summary>
        /// Blink when hit
        /// </summary>
        private void Blink(GameTime gT)
        {
            _blinkCounterStart += (float)gT.ElapsedGameTime.TotalSeconds;

            if (_blinkCounterStart >= BlinkDelay)
            {
                Visible = !Visible;
                _blinkCounterStart = 0f;
            }

        }

        private void UpdateSword(GameTime gameTime, MouseState currentMouseState, GamePadState currentGamepadState)
        {
            Sword.Update(gameTime);

            #region Mouse
            if (!Global.Gamepad)
            {
                if (currentMouseState.RightButton == ButtonState.Pressed && !Sword.Visible && _previousMouseState.RightButton != ButtonState.Pressed && CurrentState == State.Running && _canSwing)
                {
                    Swing(currentMouseState);
                    _canSwing = false;
                }
            }
            #endregion

            #region Gamepad
            else
            {
                if (currentGamepadState.IsButtonDown(Keybinds.GamepadSwing) && !Sword.Visible && !_previousGamePadState.IsButtonDown(Keybinds.GamepadSwing) && CurrentState == State.Running && _canSwing)
                {
                    Swing(currentGamepadState);
                    _canSwing = false;
                }
            }
            #endregion
        }

        private void UpdateBullet(GameTime gameTime, MouseState currentMouseState, GamePadState currentGamePadState)
        {
            foreach (var b in Bullets)
            {
                b.Update(gameTime);
            }

            #region Mouse
            if (!Global.Gamepad)
            {
                if (currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton != ButtonState.Pressed && CurrentState == State.Running)
                {
                    if (_canShoot)
                    {
                        Shoot(currentMouseState);
                        _canShoot = false;
                    }

                }

            }
            #endregion

            #region Gamepad
            else
            {
                if (currentGamePadState.IsButtonDown(Keybinds.GamepadShoot) && !_previousGamePadState.IsButtonDown(Keybinds.GamepadShoot) && CurrentState == State.Running)
                {
                    if (_canShoot)
                    {
                        Shoot(currentGamePadState);
                        _canShoot = false;
                    }

                }
            }
            #endregion

        }

        /// <summary>
        /// Mouse shooting
        /// </summary>
        private void Shoot(MouseState currentMouseState)
        {
            var mousePos = new Vector2(currentMouseState.X, currentMouseState.Y);
            mousePos = Vector2.Transform(mousePos, _cam.InverseTransform); //Get the mouse pos relative to the camera
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

        /// <summary>
        /// Mouse swinging
        /// </summary>
        private void Swing(MouseState currentMouseState)
        {
            var mousePos = new Vector2(currentMouseState.X, currentMouseState.Y);
            mousePos = Vector2.Transform(mousePos, _cam.InverseTransform); //Get the mouse pos relative to the camera
            var movement = mousePos - SpritePosition;

            if (movement != Vector2.Zero)
            {
                movement.Normalize();
            }
            Sword.Swing(SpritePosition, movement);

        }

        /// <summary>
        /// Gamepad swinging
        /// </summary>
        private void Swing(GamePadState currentGamePadState)
        {
            var swingThumb = new Vector2(currentGamePadState.ThumbSticks.Right.X, -currentGamePadState.ThumbSticks.Right.Y);
            if (swingThumb != Vector2.Zero)
            {
                Sword.Swing(SpritePosition, swingThumb);

            }

        }

        public override void Draw(SpriteBatch theSpriteBatch)
        {
            var curMouse = Mouse.GetState();
            var currentGamePadState = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular);
            var mouseLoc = new Vector2(curMouse.X, curMouse.Y);
            mouseLoc = Vector2.Transform(mouseLoc, _cam.InverseTransform); //Get the mouse pos relative to the camera
            var direction = (SpritePosition) - mouseLoc;

            #region Mouse Angle
            if (!Global.Gamepad)
            {
                _playerAngle = (float)(Math.Atan2(direction.Y, direction.X) + Math.PI / 2 + Math.PI);
            }
            #endregion

            #region Gamepad Angle
            else
            {
                var thumb = new Vector2(currentGamePadState.ThumbSticks.Right.X, currentGamePadState.ThumbSticks.Right.Y);
                if (thumb != Vector2.Zero)
                {
                    _playerAngle =
                    (float)
                        (Math.Atan2(currentGamePadState.ThumbSticks.Right.X, currentGamePadState.ThumbSticks.Right.Y));
                }

            }
            #endregion

            if (Sword.Visible)
            {
                Sword.Draw(theSpriteBatch, _playerAngle);
            }

            foreach (var b in Bullets)
            {
                b.Draw(theSpriteBatch);
            }

            if (Visible)
            {
                base.Draw(theSpriteBatch, _playerAngle);
            }

        }

        /// <summary>
        /// Get the camera from the GameplayScreen
        /// </summary>
        public void GetCamera(Camera2D cameraBeingUsed)
        {
            _cam = cameraBeingUsed;
        }

        /// <summary>
        /// Update character movement
        /// </summary>
        private void UpdateMovement(KeyboardState currentKeyboardState, GamePadState currentGamePadState)
        {
            switch (Global.Gamepad)
            {
                #region Gamepad Controls

                case true:
                    CurrentState = State.Running;

                    if (currentGamePadState.IsButtonDown(Keybinds.GamepadDodge) && !_previousGamePadState.IsButtonDown(Keybinds.GamepadDodge))
                    {
                        if (_canDodge)
                        {
                            CurrentState = State.Dodging;
                        }

                    }

                    if (currentGamePadState.IsButtonDown(Keybinds.GamepadSprint))
                    {
                        if (_canSprint)
                        {
                            _sprintCounterStart += (float)_gT.ElapsedGameTime.TotalSeconds;
                        }

                        if (_sprintCounterStart >= _sprintTime)
                        {
                            _canSprint = false;
                            _sprintCounterStart = 0;
                        }


                        if (_canSprint)
                        {
                            CurrentState = State.Sprinting;
                        }

                    }

                    switch (CurrentState)
                    {
                        #region Gamepad Running

                        case State.Running:

                            _sprintCounterStart -= (float)_gT.ElapsedGameTime.TotalSeconds;
                            if (_sprintCounterStart <= 0)
                            {
                                _sprintCounterStart = 0;
                            }
                            _speed = Vector2.Zero;
                            _direction = Vector2.Zero;

                            if (currentGamePadState.ThumbSticks.Left.X <= -0.3)
                            {
                                _speed.X = _playerSpeed;
                                _direction.X = MoveLeft;
                            }

                            else if (currentGamePadState.ThumbSticks.Left.X >= 0.3)
                            {
                                _speed.X = _playerSpeed;
                                _direction.X = MoveRight;
                            }

                            if (currentGamePadState.ThumbSticks.Left.Y >= 0.3)
                            {
                                _speed.Y = _playerSpeed;
                                _direction.Y = MoveUp;
                            }

                            else if (currentGamePadState.ThumbSticks.Left.Y <= -0.3)
                            {
                                _speed.Y = _playerSpeed;
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
                                _speed.X = _playerSpeed * _dodgeModifier;
                                _direction.X = MoveLeft;
                            }

                            else if (currentGamePadState.ThumbSticks.Left.X >= 0.3)
                            {
                                _speed.X = _playerSpeed * _dodgeModifier;
                                _direction.X = MoveRight;
                            }

                            if (currentGamePadState.ThumbSticks.Left.Y >= 0.3)
                            {
                                _speed.Y = _playerSpeed * _dodgeModifier;
                                _direction.Y = MoveUp;
                            }

                            else if (currentGamePadState.ThumbSticks.Left.Y <= -0.3)
                            {
                                _speed.Y = _playerSpeed * _dodgeModifier;
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
                                _speed.X = _playerSpeed * SprintModifier;
                                _direction.X = MoveLeft;
                            }

                            else if (currentGamePadState.ThumbSticks.Left.X >= 0.3)
                            {
                                _speed.X = _playerSpeed * SprintModifier;
                                _direction.X = MoveRight;
                            }

                            if (currentGamePadState.ThumbSticks.Left.Y >= 0.3)
                            {
                                _speed.Y = _playerSpeed * SprintModifier;
                                _direction.Y = MoveUp;
                            }

                            else if (currentGamePadState.ThumbSticks.Left.Y <= -0.3)
                            {
                                _speed.Y = _playerSpeed * SprintModifier;
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

                    if (currentKeyboardState.IsKeyDown(Keybinds.KeyboardDodge) && !_previouseKeyboardState.IsKeyDown(Keybinds.KeyboardDodge))
                    {
                        if (_canDodge)
                        {
                            CurrentState = State.Dodging;
                        }

                    }

                    if (currentKeyboardState.IsKeyDown(Keybinds.KeyboardSprint))
                    {

                        if (_canSprint)
                        {
                            _sprintCounterStart += (float)_gT.ElapsedGameTime.TotalSeconds;
                        }

                        if (_sprintCounterStart >= _sprintTime)
                        {
                            _canSprint = false;
                            _sprintCounterStart = 0;
                        }


                        if (_canSprint)
                        {
                            CurrentState = State.Sprinting;
                        }


                    }

                    switch (CurrentState)
                    {

                        #region Keyboard Running

                        case State.Running:

                            _sprintCounterStart -= (float)_gT.ElapsedGameTime.TotalSeconds;
                            if (_sprintCounterStart <= 0)
                            {
                                _sprintCounterStart = 0;
                            }

                            _speed = Vector2.Zero;
                            _direction = Vector2.Zero;

                            if (currentKeyboardState.IsKeyDown(Keys.A))
                            {
                                _speed.X = _playerSpeed;
                                _direction.X = MoveLeft;
                            }

                            else if (currentKeyboardState.IsKeyDown(Keys.D))
                            {
                                _speed.X = _playerSpeed;
                                _direction.X = MoveRight;
                            }

                            if (currentKeyboardState.IsKeyDown(Keys.W))
                            {
                                _speed.Y = _playerSpeed;
                                _direction.Y = MoveUp;
                            }

                            else if (currentKeyboardState.IsKeyDown(Keys.S))
                            {
                                _speed.Y = _playerSpeed;
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
                                _speed.X = _playerSpeed * _dodgeModifier;
                                _direction.X = MoveLeft;
                            }

                            else if (currentKeyboardState.IsKeyDown(Keys.D))
                            {
                                _speed.X = _playerSpeed * _dodgeModifier;
                                _direction.X = MoveRight;
                            }

                            if (currentKeyboardState.IsKeyDown(Keys.W))
                            {
                                _speed.Y = _playerSpeed * _dodgeModifier;
                                _direction.Y = MoveUp;
                            }

                            else if (currentKeyboardState.IsKeyDown(Keys.S))
                            {
                                _speed.Y = _playerSpeed * _dodgeModifier;
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
                                _speed.X = _playerSpeed * SprintModifier;
                                _direction.X = MoveLeft;
                            }

                            else if (currentKeyboardState.IsKeyDown(Keys.D))
                            {
                                _speed.X = _playerSpeed * SprintModifier;
                                _direction.X = MoveRight;
                            }

                            if (currentKeyboardState.IsKeyDown(Keys.W))
                            {
                                _speed.Y = _playerSpeed * SprintModifier;
                                _direction.Y = MoveUp;
                            }

                            else if (currentKeyboardState.IsKeyDown(Keys.S))
                            {
                                _speed.Y = _playerSpeed * SprintModifier;
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
