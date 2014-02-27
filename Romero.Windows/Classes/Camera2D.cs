#region Using Statements

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Romero.Windows.Classes
{
    /// <summary>
    /// Camera that follows the player
    /// </summary>
    public class Camera2D
    {
        #region Declarations

        protected Viewport Viewport;
        protected MouseState MouseState;
        protected KeyboardState KeyState;
        protected Int32 Scroll;
        private readonly Player _playerToFollow;

        protected float _zoom;
        public float Zoom
        {
            get { return _zoom; }
            set { _zoom = value; }
        }


        protected Matrix _transform;
        public Matrix Transform
        {
            get { return _transform; }
            set { _transform = value; }
        }

        /// <summary>
        /// Inverse of the view matrix, can be used to get objects screen coordinates from its object coordinates
        /// </summary>
        protected Matrix _inverseTransform;
        public Matrix InverseTransform
        {
            get { return _inverseTransform; }
        }

        protected Vector2 _pos;
        public Vector2 Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }

        protected float _rotation;
        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        #endregion

        public Camera2D(Viewport viewport, Player player)
        {
            _zoom = 1.0f;
            Scroll = 1;
            _rotation = 0.0f;
            _pos = Vector2.Zero;
            Viewport = viewport;
            _playerToFollow = player;
        }

        public void Update()
        {
            if (Global.IsDiagnosticsOpen)
            {
                CheckForInput();
            }
            _pos.X = -_playerToFollow.SpritePosition.X + Global.DeviceInUse.PreferredBackBufferWidth / 2;
            _pos.Y = -_playerToFollow.SpritePosition.Y + Global.DeviceInUse.PreferredBackBufferHeight / 2;

            _zoom = MathHelper.Clamp(_zoom, 0.0f, 10.0f); //Clamp zoom value
            _rotation = ClampAngle(_rotation); //Clamp rotation value
            _transform = Matrix.CreateRotationZ(_rotation) *
                                       Matrix.CreateScale(new Vector3(_zoom, _zoom, 1)) *
                                       Matrix.CreateTranslation(_pos.X, _pos.Y, 0); //Create view matrix
            _inverseTransform = Matrix.Invert(_transform); //Update inverse matrix
            

        }

        /// <summary>
        /// Zoom and rotation
        /// </summary>
        protected virtual void CheckForInput()
        {
            MouseState = Mouse.GetState();
            KeyState = Keyboard.GetState();

            #region Rotation
            if (KeyState.IsKeyDown(Keys.Left))
            {
                _rotation -= 0.1f;
            }
            if (KeyState.IsKeyDown(Keys.Right))
            {
                _rotation += 0.1f;
            }
            #endregion
           
            #region Zoom
            if (MouseState.ScrollWheelValue > Scroll)
            {
                _zoom += 0.1f;
                Scroll = MouseState.ScrollWheelValue;
                
            }
            else if (MouseState.ScrollWheelValue != 0 && MouseState.ScrollWheelValue < Scroll)
            {
                _zoom -= 0.1f;
                Scroll = MouseState.ScrollWheelValue;
            }
            #endregion
        }

        /// <summary>
        /// Clamps a radian value between -pi and pi
        /// </summary>
        protected float ClampAngle(float radians)
        {
            while (radians < -MathHelper.Pi)
            {
                radians += MathHelper.TwoPi;
            }
            while (radians > MathHelper.Pi)
            {
                radians -= MathHelper.TwoPi;
            }
            return radians;
        }


    }
}



