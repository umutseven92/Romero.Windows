using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework.Media;

namespace Romero.Windows.Screens
{
    internal class CharacterSelectScreen : MenuScreen
    {
        private static readonly string[] Character =
        {
            "Knight Fraser", "Lady Rebecca", "Sire Benjamin",
            "Cleric Diakonos"
        };

        private readonly MenuEntry _characterMenuEntry;

        public CharacterSelectScreen()
            : base("Character Select")
        {
            _characterMenuEntry = new MenuEntry(string.Empty);
            SetMenuEntryText();
            var back = new MenuEntry("Back");
            var play = new MenuEntry("Play");

            play.Selected += play_Selected;
            back.Selected += OnCancel;
            _characterMenuEntry.Selected += _characterMenuEntry_Selected;
            MenuEntries.Add(_characterMenuEntry);
            MenuEntries.Add(play);
            MenuEntries.Add(back);
        }

        void play_Selected(object sender, PlayerIndexEventArgs e)
        {
            
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new GameplayScreen());
        }

        void _characterMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            switch (_characterMenuEntry.Text)
            {
                case "Character: Knight Fraser":
                    Global.SelectedCharacter = Global.Character.Becky;
                    break;
                case "Character: Lady Rebecca":
                    Global.SelectedCharacter = Global.Character.Ben;
                    break;
                case "Character: Sire Benjamin":
                    Global.SelectedCharacter = Global.Character.Deacon;
                    break;
                case "Character: Cleric Diakonos":
                    Global.SelectedCharacter = Global.Character.Fraser;
                    break;
            }
            SetMenuEntryText();
        }

        private void SetMenuEntryText()
        {
            switch (Global.SelectedCharacter)
            {
                case Global.Character.Fraser:
                    _characterMenuEntry.Text = "Character: " + Character[0];
                    break;
                case Global.Character.Becky:
                    _characterMenuEntry.Text = "Character: " + Character[1];
                    break;
                case Global.Character.Ben:
                    _characterMenuEntry.Text = "Character: " + Character[2];
                    break;
                case Global.Character.Deacon:
                    _characterMenuEntry.Text = "Character: " + Character[3];
                    break;
            }
        }
    }
}
