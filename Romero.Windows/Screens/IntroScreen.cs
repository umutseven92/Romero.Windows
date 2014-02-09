
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace Romero.Windows.Screens
{
    class IntroScreen : MenuScreen
    {
        private bool _readyToGo;
        ContentManager _content;
        private Song _nightShift;


        public override void LoadContent()
        {
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");
            _nightShift = _content.Load<Song>("Music/nightshift.wav");
            MediaPlayer.Play(_nightShift);

            base.LoadContent();
        }

        public IntroScreen()
            : base("Romero")
        {
            var anyKeyMenuEntry = new MenuEntry("Press A On Your Gamepad or Enter On Your Keyboard To Continue");

            anyKeyMenuEntry.Selected += anyKeyMenuEntry_Selected;

            MenuEntries.Add(anyKeyMenuEntry);

        }

        void anyKeyMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            if (_readyToGo)
            {
                 ScreenManager.AddScreen(new MainMenuScreen(), e.PlayerIndex);
            }
           

        }

        protected override void OnCancel(PlayerIndex playerIndex)
        {
            const string message = "Are you sure you want to exit?";

            var confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }

        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }
       

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            var keyboardState = Keyboard.GetState();
            var gamepadState = GamePad.GetState(PlayerIndex.One);
            if (keyboardState.IsKeyDown(Keys.Enter))
            {
                Global.Gamepad = false;
                _readyToGo = true;
            }
            else if (gamepadState.IsButtonDown(Buttons.A))
            {
                Global.Gamepad = true;
                _readyToGo = true;
            }
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }
    }
}
