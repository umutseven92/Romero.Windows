
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

        readonly MenuEntry _ungulateMenuEntry;
        readonly MenuEntry _languageMenuEntry;
        readonly MenuEntry _frobnicateMenuEntry;
        readonly MenuEntry _elfMenuEntry;
        readonly MenuEntry _gamepadMenuEntry;

        enum Ungulate
        {
            BactrianCamel,
            Dromedary,
            Llama,
        }

        static Ungulate _currentUngulate = Ungulate.Dromedary;

        static readonly string[] Languages = { "C#", "French", "Deoxyribonucleic acid" };
        static int _currentLanguage = 0;

        private static readonly string[] Control = { "Keyboard", "Gamepad" };
        private static int _currentControl = 0;
        static bool _frobnicate = true;

        static int _elf = 23;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScreen()
            : base("Options")
        {
            // Create our menu entries.
            _ungulateMenuEntry = new MenuEntry(string.Empty);
            _languageMenuEntry = new MenuEntry(string.Empty);
            _frobnicateMenuEntry = new MenuEntry(string.Empty);
            _elfMenuEntry = new MenuEntry(string.Empty);
            _gamepadMenuEntry = new MenuEntry(string.Empty);

            SetMenuEntryText();

            var back = new MenuEntry("Back");

            // Hook up menu event handlers.
            _ungulateMenuEntry.Selected += UngulateMenuEntrySelected;
            _languageMenuEntry.Selected += LanguageMenuEntrySelected;
            _frobnicateMenuEntry.Selected += FrobnicateMenuEntrySelected;
            _elfMenuEntry.Selected += ElfMenuEntrySelected;
            back.Selected += OnCancel;
            _gamepadMenuEntry.Selected += gamepadMenuEntry_Selected;
            
            // Add entries to the menu.
            MenuEntries.Add(_ungulateMenuEntry);
            MenuEntries.Add(_languageMenuEntry);
            MenuEntries.Add(_frobnicateMenuEntry);
            MenuEntries.Add(_elfMenuEntry);
            MenuEntries.Add(_gamepadMenuEntry);
            MenuEntries.Add(back);

        }




        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            _ungulateMenuEntry.Text = "Preferred ungulate: " + _currentUngulate;
            _languageMenuEntry.Text = "Language: " + Languages[_currentLanguage];
            _frobnicateMenuEntry.Text = "Frobnicate: " + (_frobnicate ? "on" : "off");
            _elfMenuEntry.Text = "elf: " + _elf;
            _gamepadMenuEntry.Text = "Input: " + Control[_currentControl];
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
        /// Event handler for when the Ungulate menu entry is selected.
        /// </summary>
        void UngulateMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            _currentUngulate++;

            if (_currentUngulate > Ungulate.Llama)
                _currentUngulate = 0;

            SetMenuEntryText();
        }


        /// <summary>
        /// Event handler for when the Language menu entry is selected.
        /// </summary>
        void LanguageMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            _currentLanguage = (_currentLanguage + 1) % Languages.Length;

            SetMenuEntryText();
        }


        /// <summary>
        /// Event handler for when the Frobnicate menu entry is selected.
        /// </summary>
        void FrobnicateMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            _frobnicate = !_frobnicate;

            SetMenuEntryText();
        }


        /// <summary>
        /// Event handler for when the Elf menu entry is selected.
        /// </summary>
        void ElfMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            _elf++;

            SetMenuEntryText();
        }


        #endregion
    }
}
