using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace HTTPSniffer
{
    class Program
    {
        private static string _textBuffer = string.Empty;
        private static string _lastNameSaved;
        private static readonly ManualResetEvent _cleanCacheWriteBuffer = new ManualResetEvent(true);
        private static readonly ManualResetEvent _saveDiskWriteBuffer = new ManualResetEvent(true);
        private static byte[] _buffer;
        private static Socket _mainSocket;
        private static int _intervalFlush = 1;
        private static string _baseName = "httpSniffer";

        static void Main(string[] args)
        {
            try
            {
                if (args.Length != 0 && args.Length != 2)
                {
                    Console.WriteLine("Invalid number of parameters.");
                    Console.WriteLine("Usage: HTTPSniffer.exe [\"OutputFileName\" NumericIntervalToFlushTrafficToDisk]");
                    return;
                }

                if (args.Length == 2)
                {
                    _baseName = args[0];
                    _intervalFlush = int.Parse(args[1]);
                }

                InitSocket();

                var lastTimeSaved = DateTime.Now;
                while (true)
                {
                    if ((DateTime.Now - lastTimeSaved).TotalMinutes >= _intervalFlush)
                    {
                        lastTimeSaved = DateTime.Now;
                        CheckAndSave();
                    }

                    Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void InitSocket()
        {
            _mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);
            _mainSocket.Bind(new IPEndPoint(IPAddress.Parse(LocalIpAddress()), 0));
            _mainSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, true);

            _mainSocket.IOControl(IOControlCode.ReceiveAll, BitConverter.GetBytes(1), null);
            _buffer = new byte[_mainSocket.ReceiveBufferSize];

            _mainSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, OnReceive, null);
        }

        private static string LocalIpAddress()
        {
            var localIP = "";
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }

        private static void OnReceive(IAsyncResult ar)
        {
            var nReceived = _mainSocket.EndReceive(ar);

            if (nReceived <= 0)
                return;

            var ipHeader = new IPHeader(_buffer, nReceived);

            if (ipHeader.ProtocolType == Protocol.TCP)
            {
                var tcpHeader = new TCPHeader(ipHeader.Data, ipHeader.MessageLength);

                var text = Encoding.ASCII.GetString(tcpHeader.Data, 0, tcpHeader.MessageLength);
                if (text.Contains("HTTP"))
                {
                    var p = System.Diagnostics.Process.GetProcessById(WinApi.GetWindowProcessID());
                    var thisapplication = WinApi.ActiveApplTitle().Trim().Replace("\0", "") + "######" + p.ProcessName;

                    //Entro a escribir en el _buffer
                    _saveDiskWriteBuffer.Reset();
                    _cleanCacheWriteBuffer.WaitOne();
                    Logger(thisapplication, text);
                    _saveDiskWriteBuffer.Set();
                    //Termine de escribir en el _buffer
                }
            }

            _buffer = new byte[_mainSocket.ReceiveBufferSize];
            _mainSocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, OnReceive, null);
        }

        private static void Logger(string appRunning, string text)
        {
            _textBuffer += string.Format("[DateTime:{0} App: {1}]", DateTime.Now.ToString("dd-MM-yyyy-hh:mm:ss"), appRunning);
            _textBuffer += "\n";
            _textBuffer += text;
            _textBuffer += "\n";
        }

        private static void CheckAndSave()
        {
            if (_lastNameSaved == null)
            {
                _lastNameSaved = DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss");
                SaveLogfile(Path.Combine(Application.StartupPath, _baseName + "-" + _lastNameSaved + ".txt"));
            }
            else
            {
                //If we have name, we use it
                SaveLogfile(Path.Combine(Application.StartupPath, _baseName + "-" + _lastNameSaved + ".txt"));

                //We check if we need change name file, because the hour changed!
                var lastSaveHour = _lastNameSaved.Substring(11, 2);
                if (lastSaveHour != DateTime.Now.Hour.ToString())
                {
                    ClearMemoryCache();
                    _lastNameSaved = DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss");
                }
            }
        }

        private static void SaveLogfile(string path)
        {
            _saveDiskWriteBuffer.WaitOne();
            if (string.IsNullOrEmpty(_textBuffer))
                return;

            using (var file = new TxtFile(path))
            {
                file.Create();

                _saveDiskWriteBuffer.WaitOne();
                file.Write(_textBuffer);
                file.Close();
            }
        }

        private static void ClearMemoryCache()
        {
            //Entro a limpiar el _buffer
            _cleanCacheWriteBuffer.Reset();
            _textBuffer = string.Empty;
            _cleanCacheWriteBuffer.Set();
            //Termine de limpiar el _buffer
        }
    }
}
