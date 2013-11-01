using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Romero.Windows.Screens
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class OptionsMenuScreen : MenuScreen
    {
        #region Fields

        readonly MenuEntry _gamepadMenuEntry;
        private readonly MenuEntry _difficultyMenuEntry;
        private readonly MenuEntry _characterMenuEntry;
        private readonly MenuEntry _fullScreenMenuEntry;
        private static readonly string[] Control = { "Keyboard", "Gamepad" };
        private static int _currentControl = 0;
        private static readonly string[] Difficulty = { "Easy", "Normal", "Hard", "Insane" };
        private static readonly string[] Character = { "Knight Fraser", "Lady Rebecca", "Sire Benjamin", "Cleric Diakonos" };
        private static readonly string[] FullScreen = { "Full Screen", "Windowed" };
        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScreen()
            : base("Options")
        {
            // Create our menu entries.
            _gamepadMenuEntry = new MenuEntry(string.Empty);
            _difficultyMenuEntry = new MenuEntry(string.Empty);
            _characterMenuEntry = new MenuEntry(string.Empty);
            _fullScreenMenuEntry = new MenuEntry(string.Empty);
            SetMenuEntryText();
            var back = new MenuEntry("Back");

            // Hook up menu event handlers.
            back.Selected += OnCancel;
            _gamepadMenuEntry.Selected += gamepadMenuEntry_Selected;
            _difficultyMenuEntry.Selected += _difficultyMenuEntry_Selected;
            _characterMenuEntry.Selected += _characterMenuEntry_Selected;
            _fullScreenMenuEntry.Selected += _fullScreenMenuEntry_Selected;

            // Add entries to the menu.
            MenuEntries.Add(_characterMenuEntry);
            MenuEntries.Add(_difficultyMenuEntry);
            MenuEntries.Add(_gamepadMenuEntry);
            MenuEntries.Add(_fullScreenMenuEntry);
            MenuEntries.Add(back);

        }



        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            if (Global.Gamepad)
            {
                _gamepadMenuEntry.Text = "Input: " + Control[1];
            }
            else
            {
                _gamepadMenuEntry.Text = "Input: " + Control[0];
            }

            switch (Global.SelectedDifficulty)
            {
                case Global.Difficulty.Easy:
                    _difficultyMenuEntry.Text = "Difficulty: " + Difficulty[0];

                    break;
                case Global.Difficulty.Normal:
                    _difficultyMenuEntry.Text = "Difficulty: " + Difficulty[1];

                    break;
                case Global.Difficulty.Hard:
                    _difficultyMenuEntry.Text = "Difficulty: " + Difficulty[2];

                    break;
                case Global.Difficulty.Insane:
                    _difficultyMenuEntry.Text = "Difficulty: " + Difficulty[3];

                    break;
            }

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
            _fullScreenMenuEntry.Text = Global.IsFullScreen ? FullScreen[0] : FullScreen[1];

        }


        #endregion

        #region Handle Input

        /// <summary>
        /// Gamepad selection
        /// </summary>
        void gamepadMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            switch (_currentControl)
            {
                case 0:
                    _currentControl++;
                    Global.Gamepad = true;
                    break;
                case 1:
                    _currentControl = 0;
                    Global.Gamepad = false;
                    break;
            }

            SetMenuEntryText();
        }

        /// <summary>
        /// Difficulty selection
        /// </summary>
        void _difficultyMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            switch (_difficultyMenuEntry.Text)
            {
                case "Difficulty: Easy":
                    Global.SelectedDifficulty = Global.Difficulty.Normal;
                    break;
                case "Difficulty: Normal":
                    Global.SelectedDifficulty = Global.Difficulty.Hard;
                    break;
                case "Difficulty: Hard":
                    Global.SelectedDifficulty = Global.Difficulty.Insane;
                    break;
                case "Difficulty: Insane":
                    Global.SelectedDifficulty = Global.Difficulty.Easy;
                    break;
            }
            SetMenuEntryText();
        }

        /// <summary>
        /// Character selection
        /// </summary>
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

        /// <summary>
        /// Full screen
        /// </summary>
        void _fullScreenMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            if (Global.IsFullScreen)
            {
                Global.IsFullScreen = false;
                Global.ScreenChanged = true;
            }
            else
            {
                Global.IsFullScreen = true;
                Global.ScreenChanged = true;
            }
            

            SetMenuEntryText();
        }
        #endregion
    }
}
