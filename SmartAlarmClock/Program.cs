using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;

namespace SmartAlarmClock
{
    static class Program
    {
        #region API
        [DllImport("user32.dll")]
        public static extern int FindWindow(
            string lpClassName, // class name 
            string lpWindowName // window name 
        );
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hwnd, int cmd);
        #endregion
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            IntPtr h = (IntPtr)FindWindow(null, "Smart Alarm Clock");
            if (h == (IntPtr)0)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                var frm = new MainForm();
                if (args.Length > 0 && args[0] == "startup")
                    Application.Run();
                else Application.Run(frm);
            }
            else
            {
                ShowWindow(h, 1);
                SetForegroundWindow(h);
            }
        }
    }
}
