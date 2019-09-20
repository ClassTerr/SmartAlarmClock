using System;
using System.Runtime.InteropServices;

namespace SmartAlarmClock
{
    /// <summary>
    /// Класс, реализующий общие обьявления функций API для работы с питанием компьютера
    /// </summary>
    class PowerAPI
    {
        #region Методы для выключения компьютера

        [DllImport("advapi32.dll", EntryPoint = "InitiateSystemShutdownEx")]
        private static extern int InitiateSystemShutdown(string lpMachineName, string lpMessage, int dwTimeout, bool bForceAppsClosed, bool bRebootAfterShutdown);

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall,
        ref TokPriv1Luid newst, int len, IntPtr prev, IntPtr relen);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetCurrentProcess();

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr phtok);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LookupPrivilegeValue(string host, string name, ref long pluid);

        [DllImport("user32.dll", EntryPoint = "LockWorkStation")]
        private static extern bool LockWorkStation();

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct TokPriv1Luid
        {
            public int Count;
            public long Luid;
            public int Attr;
        }

        private const int SE_PRIVILEGE_ENABLED = 0x00000002;
        private const int TOKEN_QUERY = 0x00000008;
        private const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;
        private const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";

        private static void SetPriv()
        {
            TokPriv1Luid tkp;
            IntPtr htok = IntPtr.Zero;

            if (OpenProcessToken(GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref htok))
            {
                tkp.Count = 1;
                tkp.Attr = SE_PRIVILEGE_ENABLED;
                tkp.Luid = 0;
                LookupPrivilegeValue(null, SE_SHUTDOWN_NAME, ref tkp.Luid);
                AdjustTokenPrivileges(htok, false, ref tkp, 0, IntPtr.Zero, IntPtr.Zero);
            }
        }

        private static int halt(bool RSh, bool Force)
        {
            SetPriv();
            return InitiateSystemShutdown(null, null, 0, Force, RSh);
        }

        internal static int Lock()
        {
            if (LockWorkStation())
                return 1;
            else
                return 0;
        }

        internal static int Shutdown(bool Force)
        {
            return halt(false, Force);
        }

        internal static int Restart(bool Force)
        {
            return halt(true, Force);
        }
        #endregion
    }
}
