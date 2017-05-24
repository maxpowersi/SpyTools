using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;

namespace WebcamSnap
{
    class Program
    {
        private static string _baseName = "webcamsnap";
        private static bool _pictureSaved;

        static void Main(string[] args)
        {
            try
            {
                if (args.Length != 0 && args.Length != 1)
                {
                    Console.WriteLine("Invalid number of parameters.");
                    Console.WriteLine("Usage: WebcamSnap.exe [\"OutputFileName\"]");
                    Environment.Exit(0);
                }

                if (args.Length == 1)
                    _baseName = args[0];

                _baseName = _baseName + "-" + DateTime.Now.ToString("dd-MM-yy-HH-mm-ss") + ".jpg";

                //Get device
                var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                if (videoDevices.Count == 0)
                    Console.WriteLine("No webcam was detected");

                //Start capture
                var videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                videoSource.NewFrame += video_NewFrame;
                videoSource.Start();

                while (!_pictureSaved)
                    Application.DoEvents();

                videoSource.SignalToStop();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Environment.Exit(0);
            }
        }

        private static void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                var img = (Bitmap) eventArgs.Frame.Clone();

                var outputFileName = Path.Combine(Application.StartupPath, _baseName);
                using (var memory = new MemoryStream())
                {
                    using (var fs = new FileStream(outputFileName, FileMode.Create, FileAccess.ReadWrite))
                    {
                        img.Save(memory, ImageFormat.Jpeg);
                        var bytes = memory.ToArray();
                        fs.Write(bytes, 0, bytes.Length);
                    }
                }

                _pictureSaved = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Environment.Exit(0);
            }
        }
    }
}