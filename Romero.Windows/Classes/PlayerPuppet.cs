#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Romero.Windows.Classes
{
    public class PlayerPuppet : Sprite
    {
        ContentManager _contentManager;
        const int StartPositionX = 2048;
        const int StartPositionY = 2048;
        public string PlayerAssetName = "deacon";
        public long id;
        public string playerName;

        public void LoadContent(ContentManager contentManager)
        {
            _contentManager = contentManager;

            SpritePosition = new Vector2(StartPositionX, StartPositionY);
            LoadContent(_contentManager, PlayerAssetName);
            Source = new Rectangle(0, 0, 200, Source.Height);

        }

        public void Draw(SpriteBatch spriteBatch,Vector2 position)
        {
            spriteBatch.Draw(SpriteTexture2D, position,
              new Rectangle(0, 0, SpriteTexture2D.Width, SpriteTexture2D.Height),
                Color.White, 0.0f, new Vector2(SpriteTexture2D.Height / 2, SpriteTexture2D.Width / 2), ScaleCalc, SpriteEffects.None, 0);

        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, float angle)
        {
            spriteBatch.Draw(SpriteTexture2D, position,
              new Rectangle(0, 0, SpriteTexture2D.Width, SpriteTexture2D.Height),
                Color.White, angle, new Vector2(SpriteTexture2D.Height / 2, SpriteTexture2D.Width / 2), ScaleCalc, SpriteEffects.None, 0);
        }
    }
}
