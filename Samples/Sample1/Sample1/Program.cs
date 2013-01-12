using System;
using System.Windows.Forms;
using LgLCD.Net;
using System.Drawing;

namespace Sample1 {
    class Program {
        private static bool _blnIsColorDisplay = false;
        [STAThread]
        static void Main() {

            LcdManager.Init("MyApplet", true, GetBitmap);
            Application.Run();
        }

        private static IntPtr GetBitmap() {
            Bitmap objBitmap;
            if (_blnIsColorDisplay) {
                objBitmap = new Bitmap(320, 240);
            } else {
                objBitmap = new Bitmap(160, 43);
            }

            //create an empty Bitmap
            Graphics g = Graphics.FromImage(objBitmap);
            g.Clear(Color.White);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            g.DrawString("My Applet", new Font("Arial", 14, FontStyle.Regular), SystemBrushes.WindowText, 0, 20);

            return objBitmap.GetHbitmap();
        }
    }
}
