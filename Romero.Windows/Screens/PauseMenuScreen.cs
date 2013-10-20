
namespace Romero.Windows.Screens
{
    /// <summary>
    /// The pause menu comes up over the top of the game,
    /// giving the player options to resume or quit.
    /// </summary>
    class PauseMenuScreen : MenuScreen
    {

        private static readonly string[] Control = { "Keyboard", "Gamepad" };
        private static int _currentControl = 0;
        readonly MenuEntry _gamepadMenuEntry;

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public PauseMenuScreen()
            : base("Paused")
        {
            // Create our menu entries.
            var resumeGameMenuEntry = new MenuEntry("Resume Game");
            var quitGameMenuEntry = new MenuEntry("Quit Game");
            _gamepadMenuEntry = new MenuEntry("Input: " + Control[_currentControl]);

            // Hook up menu event handlers.
            resumeGameMenuEntry.Selected += OnCancel;
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;
            _gamepadMenuEntry.Selected += gamepadMenuEntry_Selected;

            // Add entries to the menu.
            MenuEntries.Add(resumeGameMenuEntry);
            MenuEntries.Add(_gamepadMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);

        }




        #endregion

        #region Handle Input
        
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
            _gamepadMenuEntry.Text = "Input: " + Control[_currentControl];
        }

        /// <summary>
        /// Event handler for when the Quit Game menu entry is selected.
        /// </summary>
        void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            const string message = "Are you sure you want to quit this game?";

            var confirmQuitMessageBox = new MessageBoxScreen(message);

            confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmQuitMessageBox, ControllingPlayer);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to quit" message box. This uses the loading screen to
        /// transition from the game back to the main menu screen.
        /// </summary>
        void ConfirmQuitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                           new MainMenuScreen());
        }


        #endregion
    }
}
