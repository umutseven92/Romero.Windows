
using OpenTK.Graphics.ES10;

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
        private static readonly string[] Control = { "Keyboard", "Gamepad" };
        private static int _currentControl = 0;
        private static readonly string[] Difficulty = { "Easy", "Normal", "Hard", "Insane" };
        private static int _currentDifficuly = 0;

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
            SetMenuEntryText();
            var back = new MenuEntry("Back");

            // Hook up menu event handlers.
            back.Selected += OnCancel;
            _gamepadMenuEntry.Selected += gamepadMenuEntry_Selected;
            _difficultyMenuEntry.Selected += _difficultyMenuEntry_Selected;

            // Add entries to the menu.
            MenuEntries.Add(_gamepadMenuEntry);
            MenuEntries.Add(_difficultyMenuEntry);
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

            switch (Global.SelectedDiffuculty)
            {
                case Global.Diffuculty.Easy:
                    _difficultyMenuEntry.Text = "Difficulty: "+Difficulty[0];
                    _currentDifficuly = 0;
                    break;
                case Global.Diffuculty.Normal:
                    _difficultyMenuEntry.Text = "Difficulty: "+Difficulty[1];
                    _currentDifficuly = 1;
                    break;
                case Global.Diffuculty.Hard:
                    _difficultyMenuEntry.Text = "Difficulty: "+Difficulty[2];
                    _currentDifficuly = 2;
                    break;
                case Global.Diffuculty.Insane:
                    _difficultyMenuEntry.Text = "Difficulty: "+Difficulty[3];
                    _currentDifficuly = 3;
                    break;
            }

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
                    Global.SelectedDiffuculty = Global.Diffuculty.Normal;
                    break;
                case "Difficulty: Normal":
                    Global.SelectedDiffuculty = Global.Diffuculty.Hard;
                    break;
                case "Difficulty: Hard":
                    Global.SelectedDiffuculty = Global.Diffuculty.Insane;
                    break;
                case "Difficulty: Insane":
                    Global.SelectedDiffuculty = Global.Diffuculty.Easy;
                    break;
            }
            SetMenuEntryText();
        }

        #endregion
    }
}
