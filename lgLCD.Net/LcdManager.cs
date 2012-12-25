using System;
using System.Windows.Forms;

namespace LgLCD.Net {
    /// <summary>
    /// Delegate for when the user presses the LCD buttons
    /// </summary>
    /// <param name="pButton"></param>
    public delegate void buttonPressed(LCDButton pButton);

    /// <summary>
    /// Delegate that will be called when a new bitmap must be drawn
    /// </summary>
    /// <returns></returns>
    public delegate IntPtr GetBitmap();

    /// <summary>
    /// Class that manages the initialization and closing of the LCD display connection
    /// </summary>
    public static class LcdManager {
        public static event buttonPressed buttonPressed;

        /// <summary>
        /// Initialization function for the LCD connection
        /// </summary>
        /// <param name="pAppletName">Display Name</param>
        /// <param name="pForceToFront">Make the applet show up right away</param>
        /// <param name="pDelegate">Function that will provide the bitmap to be displayed, function must return an IntPtr to the bitmap</param>
        public static void Init(string pAppletName, bool pForceToFront, GetBitmap pDelegate) {
            //Set up the initial connection & start drawing on the display
            LcdHelper.Connect(pAppletName, pForceToFront, pDelegate);

            LcdHelper.buttonPressed += new buttonPressed(LcdHelper_buttonPressed);

            //make sure to close the connection when the app closes
            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
        }


        /// <summary>
        /// Callback called when a button on the displayed is pressed
        /// </summary>
        /// <param name="pButton"></param>
        private static void LcdHelper_buttonPressed(LCDButton pButton) {
            if (buttonPressed != null) {
                buttonPressed(pButton);
            }
        }

        /// <summary>
        /// Disconnects the application from the LCD
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Application_ApplicationExit(object sender, EventArgs e) {
            LcdHelper.Disconnect();
        }
    }
}