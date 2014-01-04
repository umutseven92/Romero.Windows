#region Using Statements

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Romero.Windows.Classes
{
    class Zombie : Sprite
    {
        #region Declarations

        public int Id { get; set; }
        const string ZombieAssetName = "enemy";
        private static readonly Random Random = new Random();


        public bool Visible = true;

        Vector2 _direction = Vector2.Zero;
        Vector2 _speed;

        #endregion

        public void LoadContent(ContentManager contentManager)
        {
            switch (Global.ZombieSpawnSeed % 4)
            {
                case 0:
                    SpritePosition = new Vector2(Random.Next(-200, 2120), -200);
                    break;
                case 1:
                    SpritePosition = new Vector2(2120, Random.Next(0, 1080));

                    break;
                case 2:
                    SpritePosition = new Vector2(Random.Next(-200, 2120), 1280);

                    break;
                case 3:
                    SpritePosition = new Vector2(-200, Random.Next(0, 1080));
                    break;
            }
            Global.ZombieSpawnSeed++;

            _speed = new Vector2(Random.Next(25, 100), Random.Next(25, 100));
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
                base.Draw(theSpriteBatch, position);
            }
        }
    }
}
