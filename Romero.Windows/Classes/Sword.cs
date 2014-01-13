using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Romero.Windows.Classes
{
    public class Sword : Sprite
    {
        private new const string AssetName = "sword";
        public bool Visible = false;
         public  int MaxDistance = 80;
        Vector2 _startPosition;
        Vector2 _direction;
        private const float Speed = 500f; 

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

        public void Swing(Vector2 theStartPosition, Vector2 theDirection)
        {
            SpritePosition = theStartPosition;
            _startPosition = theStartPosition;
            _direction = theDirection;
            Visible = true;
        }
    }


}
