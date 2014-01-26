#region Using Statements

using Microsoft.Xna.Framework.Input;
using OpenTK.Input; 

#endregion

namespace Romero.Windows
{
    /// <summary>
    /// Keybinds
    /// </summary>
    public static class Keybinds
    {
        #region Developer Controls

        public static Keys DeveloperDiagnostics = Keys.P;
        public static Keys DeveloperKillAll = Keys.K; 

        #endregion

        #region Keyboard Controls

        public static Keys KeyboardUp = Keys.W;
        public static Keys KeyboardDown = Keys.S;
        public static Keys KeyboardRight = Keys.D;
        public static Keys KeyboardLeft = Keys.A;
        public static Keys KeyboardSprint = Keys.LeftShift;
        public static Keys KeyboardDodge = Keys.Space;
        public static MouseButton KeyboardShoot = MouseButton.Left;
        public static MouseButton KeyboardSwing = MouseButton.Right;

        #endregion

        #region Gamepad Controls

        public static float GamepadXMovement = 0.3f;
        public static float GamepadYMovement = 0.3f;
        public static Buttons GamepadSprint = Buttons.RightTrigger;
        public static Buttons GamepadDodge = Buttons.LeftTrigger;
        public static Buttons GamepadShoot = Buttons.RightShoulder;
        public static Buttons GamepadSwing = Buttons.LeftShoulder; 

        #endregion
    }
}