#region Using Statements

using Microsoft.Xna.Framework;

#endregion

namespace Romero.Windows
{
    /// <summary>
    /// Global variables
    /// </summary>
    public static class Global
    {
        /// <summary>
        /// Graphics Device for diagnostics
        /// </summary>
        public static GraphicsDeviceManager DeviceInUse;

        #region Zombie Spawn Values
        public static int ZombieSpawnDelay = 0;

        public static int ZombieSpawnTicker = 5;

        public static int ZombieSpawnSeed; 
        #endregion

        /// <summary>
        /// Main game currently running
        /// </summary>
        public static Game GameInProgress;
        
        /// <summary>
        /// Full screen
        /// </summary>
        public static bool IsFullScreen = true;

        /// <summary>
        /// Trigger to change fullscreen
        /// </summary>
        public static bool ScreenChanged = false;

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

        /// <summary>
        /// Selectable diffuculty
        /// </summary>
        public enum Difficulty
        {
            Easy,
            Normal,
            Hard,
            Insane
        }

        /// <summary>
        /// Selected diffuculty
        /// </summary>
        public static Difficulty SelectedDifficulty = Difficulty.Normal;

        /// <summary>
        /// Diagnostics open/closed
        /// </summary>
        public static bool IsDiagnosticsOpen = false;


    }

}

