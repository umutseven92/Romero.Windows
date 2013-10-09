#region using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Romero.Windows
{
    class Sprite
    {
        //The current position of the Sprite
        public Vector2 SpritePosition = new Vector2(0, 0);

        //The texture object used when drawing the sprite
        private Texture2D _spriteTexture2D;

        //The asset name for the Sprite's Texture
        public string AssetName;

        //The Size of the Sprite (with scale applied)
        public Rectangle Size;

        //The amount to increase/decrease the size of the original sprite. 
        private float _scale = 1.0f;

        //When the scale is modified throught he property, the Size of the 
        //sprite is recalculated with the new scale applied.
        public float ScaleCalc
        {
            get { return _scale; }
            set
            {
                _scale = value;
                //Recalculate the Size of the Sprite with the new scale
                Size = new Rectangle(0, 0, (int) (_spriteTexture2D.Width*_scale), (int) (_spriteTexture2D.Height*_scale));
            }
        }

        //Load the texture for the sprite using the Content Pipeline
        public void LoadContent(ContentManager contentManager, string assetName)
        {
            _spriteTexture2D = contentManager.Load<Texture2D>(assetName);
            AssetName = assetName;
            Size = new Rectangle(0, 0, (int)(_spriteTexture2D.Width * ScaleCalc), (int)(_spriteTexture2D.Height * ScaleCalc));
        }

        //Draw the sprite to the screen
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_spriteTexture2D, SpritePosition,
                new Rectangle(0, 0, _spriteTexture2D.Width, _spriteTexture2D.Height),
                Color.White, 0.0f, Vector2.Zero, ScaleCalc, SpriteEffects.None, 0);
        }

        //Update the Sprite and change it's position based on the passed in speed, direction and elapsed time.
        public void Update(GameTime gameTime, Vector2 speed, Vector2 direction)
        {
            SpritePosition += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
