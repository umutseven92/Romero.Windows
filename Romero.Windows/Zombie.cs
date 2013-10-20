#region Using Statements

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Romero.Windows
{
    class Zombie : Sprite
    {
        #region Declarations

        public int Id { get; set; }
        const string ZombieAssetName = "enemy";
        private static readonly Random Random = new Random();

        //int StartPositionX = 640;
        //int StartPositionY = 200;

        const int EnemySpeed = 300;
        const int MoveUp = -1;
        const int MoveDown = 1;
        const int MoveLeft = -1;
        const int MoveRight = 1;
        public bool Visible = true;

        readonly Vector2 _direction = Vector2.Zero;
        readonly Vector2 _speed = Vector2.Zero;

        #endregion

        public void LoadContent(ContentManager contentManager)
        {
            SpritePosition = new Vector2(Random.Next(1080), Random.Next(720));
            LoadContent(contentManager, ZombieAssetName);
        }

        /// <summary>
        /// Update only when visible
        /// </summary>
        public void Update(GameTime gameTime)
        {
            if (Visible)
            {
                Update(gameTime, _speed, _direction);
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
    }
}
