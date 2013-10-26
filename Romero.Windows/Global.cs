
namespace Romero.Windows
{
    /// <summary>
    /// Global variables
    /// </summary>
    public static class Global
    {
        /// <summary>
        /// Gamepad connection - OptionsMenuScreen.cs, PauseMenuScreen.cs, Player.cs
        /// </summary>
        public static bool Gamepad = false;

        /// <summary>
        /// Selectable characters
        /// </summary>
        public enum Character
        {
            Fraser,
            Becky,
            Deacon,
            Ben
        }

        /// <summary>
        /// Selected character
        /// </summary>
        public static Character SelectedCharacter = Character.Fraser;
    }

}

