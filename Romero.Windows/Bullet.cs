#region Using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Romero.Windows
{
    public class Bullet : Sprite
    {
        #region Declarations

        const int MaxDistance = 5000;
        private new const string AssetName = "bullet";
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
        
        /// <summary>
        /// Update only when visible
        /// </summary>
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
        
        /// <summary>
        /// Draw only when visible
        /// </summary>
        public override void Draw(SpriteBatch theSpriteBatch)
        {
            if (Visible)
            {
                base.Draw(theSpriteBatch);
            }
        }



        public void Fire(Vector2 theStartPosition, Vector2 theDirection)
        {
            SpritePosition = theStartPosition;
            _startPosition = theStartPosition;
            _direction = theDirection;
            Visible = true;
        }


    }
}
