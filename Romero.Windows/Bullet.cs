using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Romero.Windows
{
   public class Bullet:Sprite
    {
        const int MaxDistance = 1000;
        private new const string AssetName = "bullet";

        public bool Visible = false;

        Vector2 mStartPosition;
        Vector2 mSpeed;
        Vector2 mDirection;
        private const float Speed = 1200f;

        public void LoadContent(ContentManager theContentManager)
        {
            base.LoadContent(theContentManager, AssetName);
            ScaleCalc = 1f;
        }

        public void Update(GameTime theGameTime)
        {
            if (Vector2.Distance(mStartPosition, SpritePosition) > MaxDistance)
            {
                Visible = false;
            }

            if (Visible)
            {
                SpritePosition += mDirection * Speed * (float)theGameTime.ElapsedGameTime.TotalSeconds;
                
            }
        }

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
            mStartPosition = theStartPosition;
          mDirection = theDirection;
            Visible = true;
        }

        
    }
}
