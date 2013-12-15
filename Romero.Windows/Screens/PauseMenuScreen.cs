
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
        private readonly MenuEntry _fullScreenMenuEntry;
        private static readonly string[] FullScreen = { "Full Screen", "Windowed" };
        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public PauseMenuScreen()
            : base("Paused")
        {
            // Create our menu entries.
            var resumeGameMenuEntry = new MenuEntry("Resume Game");
            var mainMenuEntry = new MenuEntry("Back to Main Menu");
            var quitMenuEntry = new MenuEntry("Quit Game");
            _gamepadMenuEntry = new MenuEntry(string.Empty);
            _fullScreenMenuEntry = new MenuEntry(string.Empty);

            SetMenuEntryText();
            // Hook up menu event handlers.
            resumeGameMenuEntry.Selected += OnCancel;
            mainMenuEntry.Selected += QuitGameMenuEntrySelected;
            _gamepadMenuEntry.Selected += gamepadMenuEntry_Selected;
            _fullScreenMenuEntry.Selected += _fullScreenMenuEntry_Selected;
            quitMenuEntry.Selected += quitMenuEntry_Selected;

            // Add entries to the menu.
            MenuEntries.Add(resumeGameMenuEntry);
            MenuEntries.Add(_gamepadMenuEntry);
            MenuEntries.Add(_fullScreenMenuEntry);
            MenuEntries.Add(mainMenuEntry);
            MenuEntries.Add(quitMenuEntry);

        }



        #endregion

        #region Handle Input

        private void SetMenuEntryText()
        {
            if (Global.Gamepad)
            {
                _gamepadMenuEntry.Text = "Input: " + Control[1];
            }
            else
            {
                _gamepadMenuEntry.Text = "Input: " + Control[0];
            }
            _fullScreenMenuEntry.Text = Global.IsFullScreen ? FullScreen[0] : FullScreen[1];

        }

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

        void quitMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            const string message = "Are you sure you want to exit?";

            var confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += confirmExitMessageBox_Accepted;
            ScreenManager.AddScreen(confirmExitMessageBox, e.PlayerIndex);

        }

        void confirmExitMessageBox_Accepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }

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
