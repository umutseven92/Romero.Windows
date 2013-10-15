using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;


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
        
        
        Vector2 _direction = Vector2.Zero;
        Vector2 _speed = Vector2.Zero;

        public void LoadContent(ContentManager contentManager)
        {
            SpritePosition = new Vector2(random.Next(800), random.Next(800));
            LoadContent(contentManager, ZombieAssetName);
            
        }

        public void Update(GameTime gameTime)
        {
           
            Update(gameTime, _speed, _direction);
        }
    }
}
