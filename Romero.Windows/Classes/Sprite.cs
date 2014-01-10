#region using Statements

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Romero.Windows.Classes
{
    public class Sprite
    {
        #region Declarations

        /// <summary>
        /// Collision box
        /// </summary>
        readonly Texture2D _boundingBoxTexture = new Texture2D(Global.DeviceInUse.GraphicsDevice, 1, 1);
        private Rectangle _boundingBox;
        public Rectangle BoundingBox
        {
            get
            {
                _boundingBox = new Rectangle(
                    (int)SpritePosition.X - SpriteTexture2D.Width / 2,
                    (int)SpritePosition.Y - SpriteTexture2D.Height / 2,
                    SpriteTexture2D.Width,
                    SpriteTexture2D.Height);
                return _boundingBox;
            }
            set { _boundingBox = value; }
        }

        //The current position of the Sprite
        public Vector2 SpritePosition = new Vector2(0, 0);

        //The texture object used when drawing the sprite
        public Texture2D SpriteTexture2D;

        //The asset name for the Sprite's Texture
        public string AssetName;

        //The Size of the Sprite (with scale applied)
        public Rectangle Size;

        //The amount to increase/decrease the size of the original sprite.
        private float _scale = 1.0f;

        //When the scale is modified throught he property, the Size of the sprite is recalculated with the new scale applied
        public float ScaleCalc
        {
            get { return _scale; }
            set
            {
                _scale = value;
                Size = new Rectangle(0, 0, (int)(Source.Width * ScaleCalc), (int)(Source.Height * ScaleCalc));
            }
        }

        //The Rectangular area from the original image that defines the Sprite
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
            _boundingBoxTexture.SetData(new[] { Color.White });
            SpriteTexture2D = contentManager.Load<Texture2D>(assetName);
            AssetName = assetName;
            Source = new Rectangle(0, 0, SpriteTexture2D.Width, SpriteTexture2D.Height);
            Size = new Rectangle(0, 0, (int)(SpriteTexture2D.Width * ScaleCalc), (int)(SpriteTexture2D.Height * ScaleCalc));
        }

        /// <summary>
        /// Draw without direction angle
        /// </summary>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (Global.IsDiagnosticsOpen)
            {
                spriteBatch.Draw(_boundingBoxTexture, BoundingBox, Color.Red);
            }

            spriteBatch.Draw(SpriteTexture2D, SpritePosition,
              new Rectangle(0, 0, SpriteTexture2D.Width, SpriteTexture2D.Height),
                Color.White, 0.0f, new Vector2(SpriteTexture2D.Height / 2, SpriteTexture2D.Width / 2), ScaleCalc, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Draw with direction angle
        /// </summary>
        /// <param name="rotation">Direction angle as float</param>
        public virtual void Draw(SpriteBatch spriteBatch, float rotation)
        {
            if (Global.IsDiagnosticsOpen)
            {
                spriteBatch.Draw(_boundingBoxTexture, BoundingBox, Color.Red);
            }

            spriteBatch.Draw(SpriteTexture2D, SpritePosition,
              new Rectangle(0, 0, SpriteTexture2D.Width, SpriteTexture2D.Height), Color.White, rotation, new Vector2(SpriteTexture2D.Height / 2, SpriteTexture2D.Width / 2), ScaleCalc, SpriteEffects.None, 0);
        }

        public void Update(GameTime gameTime, Vector2 speed, Vector2 direction)
        {
            SpritePosition += direction * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
