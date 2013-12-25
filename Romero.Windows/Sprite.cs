#region using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Romero.Windows.ScreenManager;

#endregion

namespace Romero.Windows
{
    public class Sprite
    {
        #region Declarations

        /// <summary>
        /// Collision box
        /// </summary>
        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle(
                    (int)SpritePosition.X,
                    (int)SpritePosition.Y,
                    _spriteTexture2D.Width,
                    _spriteTexture2D.Height);
            }
        }


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
                Size = new Rectangle(0, 0, (int)(Source.Width * ScaleCalc), (int)(Source.Height * ScaleCalc));
            }
        }

        //The Rectangular area from the original image that 
        //defines the Sprite. 
        Rectangle _source;
        public Rectangle Source
        {
            get { return _source; }
            set
            {
                _source = value;
                Size = new Rectangle(0, 0, (int)(_source.Width * ScaleCalc), (int)(_source.Height * ScaleCalc));
            }
        }

        #endregion

        public void LoadContent(ContentManager contentManager, string assetName)
        {
         
            _spriteTexture2D = contentManager.Load<Texture2D>(assetName);
            AssetName = assetName;
            Source = new Rectangle(0, 0, _spriteTexture2D.Width, _spriteTexture2D.Height);
            Size = new Rectangle(0, 0, (int)(_spriteTexture2D.Width * ScaleCalc), (int)(_spriteTexture2D.Height * ScaleCalc));
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_spriteTexture2D, SpritePosition,
              new Rectangle(0, 0, _spriteTexture2D.Width, _spriteTexture2D.Height),
                Color.White, 0.0f, new Vector2(_spriteTexture2D.Height / 2, _spriteTexture2D.Width / 2), ScaleCalc, SpriteEffects.None, 0);
        }

        public virtual void Draw(SpriteBatch spriteBatch, float rotation)
        {
            
            spriteBatch.Draw(_spriteTexture2D, SpritePosition,
              new Rectangle(0, 0, _spriteTexture2D.Width, _spriteTexture2D.Height),
                Color.White, rotation, new Vector2(_spriteTexture2D.Height / 2, _spriteTexture2D.Width / 2), ScaleCalc, SpriteEffects.None, 0);
        }

        public void Update(GameTime gameTime, Vector2 speed, Vector2 direction)
        {
            SpritePosition += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
