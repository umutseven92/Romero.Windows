#region Using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Romero.Windows.Classes
{
    /// <summary>
    /// The main projectile class
    /// </summary>
    public class Bullet : Sprite
    {
        #region Declarations

        const int MaxDistance = 5000;
        private new const string AssetName = "arrow";
        public bool Visible = false;
        Vector2 _startPosition;
        Vector2 _direction;
        private const float Speed = 1200f;

        #endregion

        public void LoadContent(ContentManager theContentManager)
        {
            LoadContent(theContentManager, AssetName);
            ScaleCalc = 1f;
        }

        public void Update(GameTime theGameTime)
        {
            if (Vector2.Distance(_startPosition, SpritePosition) > MaxDistance)
            {
                Visible = false;
            }

            if (Visible)
            {
                SpritePosition += _direction * Speed * (float)theGameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public override Rectangle BoundingBox
        {
            get
            {
                return new Rectangle(
                    (int)SpritePosition.X - SpriteTexture2D.Width / 2,
                    (int)SpritePosition.Y - SpriteTexture2D.Height / 2,
                    SpriteTexture2D.Width,
                    SpriteTexture2D.Height);
            }
        }

        public override void Draw(SpriteBatch theSpriteBatch)
        {
            if (Visible)
            {
                base.Draw(theSpriteBatch);
            }
        }

        #region Methods

        /// <summary>
        /// Set all the movement values
        /// </summary>
        public void Fire(Vector2 theStartPosition, Vector2 theDirection)
        {
            SpritePosition = theStartPosition;
            _startPosition = theStartPosition;
            _direction = theDirection;
            Visible = true;
        }

        #endregion

    }
}
