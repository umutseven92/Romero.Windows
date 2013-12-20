using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Romero.Windows.Screens
{
    class GameOverScreen:MenuScreen
    {
        

        public GameOverScreen() : base("Game Over")
        {
            var retryMenuEntry = new MenuEntry("Retry");
            var mainMenuEntry = new MenuEntry("Main Menu");
            var exitMenuEntry = new MenuEntry("Quit Game");

            retryMenuEntry.Selected += retryMenuEntry_Selected;
            mainMenuEntry.Selected += mainMenuEntry_Selected;
            exitMenuEntry.Selected += OnCancel;

            MenuEntries.Add(retryMenuEntry);
            MenuEntries.Add(mainMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }

        protected override void OnCancel(PlayerIndex playerIndex)
        {
            const string message = "Are you sure you want to exit?";

            var confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += confirmExitMessageBox_Accepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }

        void confirmExitMessageBox_Accepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }

        void mainMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            const string message = "Are you sure you want to quit this game?";

            var confirmQuitMessageBox = new MessageBoxScreen(message);

            confirmQuitMessageBox.Accepted += confirmQuitMessageBox_Accepted;

            ScreenManager.AddScreen(confirmQuitMessageBox, ControllingPlayer);
        }

        void confirmQuitMessageBox_Accepted(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                          new MainMenuScreen());
        }

        void retryMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new GameplayScreen());
        }
    }
}
