#region Using Statements

using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

#endregion

namespace Romero.Windows.Screens
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {

        ContentManager _content;
        private Song _nightShift;

        #region Initialization

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base("Main Menu")
        {

            // Create our menu entries.
            var playGameMenuEntry = new MenuEntry("Play Singleplayer");
            var multiplayerCreateMenuEntry = new MenuEntry("Create Multiplayer Game");
            var multiplayerJoinMenuEntry = new MenuEntry("Join Multiplayer Game");
            var optionsMenuEntry = new MenuEntry("Options");
            var exitMenuEntry = new MenuEntry("Exit");

            // Hook up menu event handlers.
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            multiplayerCreateMenuEntry.Selected += multiplayerCreateMenuEntry_Selected;
            multiplayerJoinMenuEntry.Selected += multiplayerJoinMenuEntry_Selected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(multiplayerCreateMenuEntry);
            MenuEntries.Add(multiplayerJoinMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }

        #endregion

        public override void LoadContent()
        {
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");
            if (MediaPlayer.State == MediaState.Stopped)
            {
                _nightShift = _content.Load<Song>("Music/nightshift.wav");
                MediaPlayer.Volume = 1;
                MediaPlayer.Play(_nightShift);
            }

            base.LoadContent();
        }

        #region Handle Input

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new CharacterSelectScreen(), e.PlayerIndex);

            //LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new GameplayScreen());
        }

        void multiplayerJoinMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new JoinScreen(), e.PlayerIndex);
        }

        void multiplayerCreateMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new LobbyScreen(), e.PlayerIndex);
        }


        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
        }


        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            const string message = "Are you sure you want to exit?";

            var confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }


        #endregion
    }
}
