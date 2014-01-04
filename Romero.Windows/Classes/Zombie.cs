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

        public bool Visible = true;

        Vector2 _direction = Vector2.Zero;
        readonly Vector2 _speed = new Vector2(50,50);

        #endregion

        public void LoadContent(ContentManager contentManager)
        {
            SpritePosition = new Vector2(Random.Next(1920), Random.Next(1080));
            LoadContent(contentManager, ZombieAssetName);
        }

        /// <summary>
        /// Update only when visible
        /// </summary>
        public void Update(GameTime gameTime, Player player)
        {
            if (Visible)
            {
                UpdateMovement(player);
                Update(gameTime, _speed, _direction);
            }
        }
        
        private void UpdateMovement(Player player)
        {
            var playerPos = new Vector2(player.SpritePosition.X, player.SpritePosition.Y);
            var movement = playerPos - SpritePosition;

            movement.Normalize();
            _direction = movement;

        }

        /// <summary>
        /// Draw only when visible
        /// </summary>
        public override void Draw(SpriteBatch theSpriteBatch, float position)
        {
            if (Visible)
            {
                base.Draw(theSpriteBatch,position);
            }
        }
    }
}
