using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Romero.Windows
{
    public class Sword : Sprite
    {
        private new const string AssetName = "sword";
        public bool Visible = false;

        public void LoadContent(ContentManager theContentManager)
        {
            LoadContent(theContentManager, AssetName);
            ScaleCalc = 1f;
        }


        public void Update(Vector2 p,Rectangle playerSize)
        {
            SpritePosition = p;
            
        }

    }


}
