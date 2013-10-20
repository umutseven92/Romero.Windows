using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace Romero.Windows
{
    class Zombie : Sprite
    {
        const string ZombieAssetName = "enemy";
        private static Random random = new Random();
        int StartPositionX = 640;
        int StartPositionY = 200;
        const int EnemySpeed = 300;
        const int MoveUp = -1;
        const int MoveDown = 1;
        const int MoveLeft = -1;
        const int MoveRight = 1;
        public bool Visible = true;

        Vector2 _direction = Vector2.Zero;
        Vector2 _speed = Vector2.Zero;

        public void LoadContent(ContentManager contentManager)
        {
            SpritePosition = new Vector2(random.Next(1080), random.Next(720));
            LoadContent(contentManager, ZombieAssetName);

        }

        public void Update(GameTime gameTime)
        {
            if (Visible)
            {
                Update(gameTime, _speed, _direction);
            }

        }

        public override void Draw(SpriteBatch theSpriteBatch)
        {
            if (Visible)
            {
                base.Draw(theSpriteBatch);
            }
        }
    }
}
