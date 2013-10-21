
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
        private static readonly string[] Control = { "Keyboard", "Gamepad" };
        private static int _currentControl = 0;

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
            SetMenuEntryText();
            var back = new MenuEntry("Back");

            // Hook up menu event handlers.
            back.Selected += OnCancel;
            _gamepadMenuEntry.Selected += gamepadMenuEntry_Selected;

            // Add entries to the menu.
            MenuEntries.Add(_gamepadMenuEntry);
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

        #endregion
    }
}
