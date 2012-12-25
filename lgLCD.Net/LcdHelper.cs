using System;
using System.Timers;

namespace LgLCD.Net {

    
    /// <summary>
    /// Class that contains the process logic for the LCD display
    /// </summary>
    internal static class LcdHelper {
        /// <summary>
        /// Event that is raised when the user presses one of the LCD buttons
        /// </summary>
        public static event buttonPressed buttonPressed;

        //Connection and device variables
        private static int _intDevice = 0;
        private static int _intDeviceType = 0;
        private static int _intConnection = 0;

        /// <summary>
        /// GetBitmap function that will be called when a new LCD bitmap is needed
        /// </summary>
        private static GetBitmap _delGetBitmapFunction;

        /// <summary>
        /// Sets up the initial connection with the keyboard
        /// Also makes the applet pop up to the front
        /// </summary>
        public static void Connect(string pAppletName, bool pForceForground, GetBitmap pDelegate) {
            _delGetBitmapFunction = pDelegate;

            //Make it work for both color and b&w displays
            if (DMcLgLCD.ERROR_SUCCESS == DMcLgLCD.LcdInit()) {
                _intConnection = DMcLgLCD.LcdConnect(pAppletName, 0, 0);

                if (DMcLgLCD.LGLCD_INVALID_CONNECTION != _intConnection) {
                    _intDevice = DMcLgLCD.LcdOpenByType(_intConnection, DMcLgLCD.LGLCD_DEVICE_QVGA);

                    if (DMcLgLCD.LGLCD_INVALID_DEVICE == _intDevice) {
                        _intDevice = DMcLgLCD.LcdOpenByType(_intConnection, DMcLgLCD.LGLCD_DEVICE_BW);
                        if (DMcLgLCD.LGLCD_INVALID_DEVICE != _intDevice) {
                            _intDeviceType = DMcLgLCD.LGLCD_DEVICE_BW;
                        }
                    } else {
                        _intDeviceType = DMcLgLCD.LGLCD_DEVICE_QVGA;
                    }
                }

                //Set the initial bitmap
                SetBitmap(_delGetBitmapFunction());

                //Force the applet to the front if needed
                if (pForceForground) {
                    DMcLgLCD.LcdSetAsLCDForegroundApp(_intDevice, DMcLgLCD.LGLCD_FORE_YES);
                }

                //Start drawing
                StartDrawing();
            }

            //Set callback function for the keyboard buttons - raise the event 
            if (_intDeviceType > 0) {
                DMcLgLCD.LcdSetButtonCallback(delegate(int pDeviceType, int pButton) {
                    if (buttonPressed != null) {
                        LCDButton objButton = (LCDButton)pButton;
                        buttonPressed(objButton);
                    }
                });
            }
        }

        /// <summary>
        /// Starts the drawing on the LCD
        /// </summary>
        private static void StartDrawing() {
            //Refresh the LCD display every 100ms, ideal is between 30 & 100ms
            //Keep in mind that B&W lcd's will have a blur when the fps is to high
            Timer objTimer = new Timer(100);
            objTimer.Elapsed += new ElapsedEventHandler(
                delegate {
                    SetBitmap(_delGetBitmapFunction());
                }
            );
            objTimer.Start();
        }

        /// <summary>
        /// Shortcut function to draw a new bitmap on the display
        /// </summary>
        /// <param name="pBitmap"></param>
        private static void SetBitmap(IntPtr pBitmap) {
            DMcLgLCD.LcdUpdateBitmap(_intDevice, pBitmap, DMcLgLCD.LGLCD_DEVICE_BW);
        }

        /// <summary>
        /// Close the connection to the LCD device
        /// </summary>
        internal static void Disconnect() {
            DMcLgLCD.LcdClose(_intDevice);
            DMcLgLCD.LcdDisconnect(_intConnection);
            DMcLgLCD.LcdDeInit();
        }


    }
}
