using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace Romero.Windows
{
     class Zombie : Sprite
    {
        const string ZombieAssetName = "enemy";
        const int StartPositionX = 640;
        const int StartPositionY = 200;
        const int EnemySpeed = 300;
        const int MoveUp = -1;
        const int MoveDown = 1;
        const int MoveLeft = -1;
        const int MoveRight = 1;


        Vector2 _direction = Vector2.Zero;
        Vector2 _speed = Vector2.Zero;

        public void LoadContent(ContentManager contentManager)
        {
            SpritePosition = new Vector2(StartPositionX, StartPositionY);
            LoadContent(contentManager, ZombieAssetName);
        }

        public void Update(GameTime gameTime)
        {

            Update(gameTime, _speed, _direction);
        }
    }
}
