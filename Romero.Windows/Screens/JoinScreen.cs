using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Romero.Windows.Screens
{
    internal class JoinScreen : MenuScreen
    {
        private readonly MenuEntry _characterMenuEntry;
        private static readonly string[] Character =
        {
            "Knight Fraser", "Lady Rebecca", "Sire Benjamin",
            "Cleric Diakonos"
        };


        public JoinScreen()
            : base("Join")
        {
            _characterMenuEntry = new MenuEntry(string.Empty);
            SetMenuEntryText();
            var local = new MenuEntry("Join Locally");
            var back = new MenuEntry("Back");
            _characterMenuEntry.Selected += _characterMenuEntry_Selected;
            back.Selected += OnCancel;
            local.Selected += local_Selected;

            MenuEntries.Add(_characterMenuEntry);
            MenuEntries.Add(local);
            MenuEntries.Add(back);

        }

        void local_Selected(object sender, PlayerIndexEventArgs e)
        {

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
