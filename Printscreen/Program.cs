using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Printscreen
{
    class Program
    {
        private static string _baseName = "printscreen";

        static void Main(string[] args)
        {
            try
            {
                if (args.Length != 0 && args.Length != 1)
                {
                    Console.WriteLine("Invalid number of parameters.");
                    Console.WriteLine("Usage: Printscreen.exe [\"OutputFileName\"]");
                    return;
                }

                if (args.Length == 1)
                    _baseName = args[0];

                _baseName = _baseName + "-" + DateTime.Now.ToString("dd-MM-yy-HH-mm-ss") + ".jpg";

                using (var bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format32bppArgb))
                {
                    using (var gfxScreenshot = Graphics.FromImage(bmpScreenshot))
                    {

                        gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
                        bmpScreenshot.Save(_baseName, ImageFormat.Jpeg);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
