using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace SmartAlarmClock
{
    /// <summary>
    /// Класс, реализующий общие обьявления функций API для работы с окнами Windows 
    /// </summary>
    public static class WindowsManagerAPI
    {
        /// <summary>
        /// Find window by Caption only. Note you must pass IntPtr.Zero as the first parameter.
        /// </summary>
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        const UInt32 WM_CLOSE = 0x0010;

        delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        public static List<string> GetAllWindowsText()
        {
            List<string> l = new List<string>();
            EnumWindows((hWnd, lParam) =>
            {
                if (IsWindowVisible(hWnd) && GetWindowTextLength(hWnd) != 0)
                {
                    l.Add(GetWindowText(hWnd));
                }
                return true;
            }, IntPtr.Zero);
            return l;
        }

        public static string GetWindowText(IntPtr hWnd)
        {
            int len = GetWindowTextLength(hWnd) + 1;
            StringBuilder sb = new StringBuilder(len);
            len = GetWindowText(hWnd, sb, len);
            return sb.ToString(0, len);
        }

        public static void KillWindowsByName(string NameOfWindow)
        {
            try
            {
                IntPtr windowPtr = FindWindowByCaption(IntPtr.Zero, NameOfWindow);
                if (windowPtr == IntPtr.Zero)
                {
                    return;
                }

                SendMessage(windowPtr, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            }
            catch { }
        }
    }
}