using System;

namespace Keylogger
{
    internal static class WinApi
    {
        #region [ Windows API Functions Declarations ]

        //This Function is used to get Active Window Title
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hwnd, string lpString, int cch);

        //This Function is used to get Handle for Active Window
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern IntPtr GetForegroundWindow();

        //This Function is used to get Active process ID
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern Int32 GetWindowThreadProcessId(IntPtr hWnd, out Int32 lpdwProcessId);

        #endregion

        /// <summary>
        /// This Function is used to get Active process ID, throug the active windows
        /// </summary>
        public static Int32 GetWindowProcessID()
        {
            var hwnd = GetForegroundWindow();

            Int32 pid;
            GetWindowThreadProcessId(hwnd, out pid);

            return pid;
        }

        /// <summary>
        /// This method is used to get active application's title using GetWindowText() method present in user32.dll,, throug the active windows
        /// </summary>
        public static string ActiveApplTitle()
        {
            var hwnd = GetForegroundWindow();

            if (hwnd.Equals(IntPtr.Zero)) 
                return "";

            var lpText = new string((char)0, 100);
            var intLength = GetWindowText(hwnd, lpText, lpText.Length);

            if ((intLength <= 0) || (intLength > lpText.Length)) 
                return "unknown";

            return lpText.Trim();
        }
    }
}