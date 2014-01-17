#region Using Statements

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Romero.Windows.Classes
{
    /// <summary>
    /// Main enemy class
    /// </summary>
    class Zombie : Sprite
    {
        #region Declarations

        public int Id { get; set; }
        const string ZombieAssetName = "enemy";

        private static readonly Random Random = new Random(); //Spawn position randomizer
        private static readonly Random HighSpeedRandom = new Random(); //Fast zombie randomizer

        public bool Visible = true;
        public bool Dead = false;
        Vector2 _direction = Vector2.Zero;
        Vector2 _speed;

        #endregion

        public void LoadContent(ContentManager contentManager)
        {
            #region Spawn Position
            switch (Global.ZombieSpawnSeed % 4)
            {
                case 0:
                    SpritePosition = new Vector2(Random.Next(-200, 4096), -200);
                    break;
                case 1:
                    SpritePosition = new Vector2(4296, Random.Next(0, 4096));

                    break;
                case 2:
                    SpritePosition = new Vector2(Random.Next(-200, 4296), 4296);

                    break;
                case 3:
                    SpritePosition = new Vector2(-200, Random.Next(0, 4096));
                    break;
            }
            #endregion

            Global.ZombieSpawnSeed++;
            Global.ZombieSpawnDelay++;

            #region Speed
            if (HighSpeedRandom.Next(0, 40) == 5)
            {
                _speed = new Vector2(300);
            }
            else
            {
                _speed = new Vector2(Random.Next(25, 100));
            }
            #endregion

            if (Global.ZombieSpawnDelay > 10)
            {
                Visible = false;
            }

            LoadContent(contentManager, ZombieAssetName);
        }

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

        public override void Draw(SpriteBatch theSpriteBatch, float position)
        {
            if (Visible)
            {
                base.Draw(theSpriteBatch, position);
            }
        }
    }
}
