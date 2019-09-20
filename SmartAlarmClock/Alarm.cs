using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace SmartAlarmClock
{
    /// <summary>
    /// Представляет класс реализующий событие
    /// </summary>
    [Serializable]
    public class Alert
    {
        public bool ShowMessage { get; set; }
        public bool TurnComputerOff { get; set; }
        public bool KillProcess { get; set; }
        public bool LaunchProgram { get; set; }
        public bool PlaySound { get; set; }
        public bool KillByName { get; set; }
        public bool ForceShutdown { get; set; }

        public string MessageText { get; set; }
        public string LaunchString { get; set; }
        public string LaunchParametrsString { get; set; }
        public string KillProcessName { get; set; }
        public string KillWindowName { get; set; }
        public string SoundFilename { get; set; }

        public DateTime AlertTime { get; set; }
        public PowerMode PowerMode { get; set; }

        public bool isUsed = false;

        public Alert()
        {
            ShowMessage = true;
        }

        int GetYearsCount(TimeSpan t)
        {
            return t.Days / 365;
        }

        int GetMonthCount(TimeSpan t)
        {
            return t.Days % 365 / 31;
        }

        public string UniversalNumberEncode(int i, string s1, string s234, string s567890)
        {
            if (i >= 10 && i <= 20)
                return i + s567890;

            switch (i % 10)
            {
                case 0:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    return i + s567890;
                case 1:
                    return i + s1;
            }

            return i + s234;
        }

        string YearsInForm(int i)
        {
            return UniversalNumberEncode(i, " год", " года", " лет");
        }

        string MonthInForm(int i)
        {
            return UniversalNumberEncode(i, " месяц", " месяца", " месяцев");
        }

        string DayInForm(int i)
        {
            return UniversalNumberEncode(i, " день", " дня", " дней");
        }

        public string TimeToAlarm
        {
            get
            {
                //посчитать сколько осталось до выполнения
                TimeSpan t = AlertTime.Subtract(DateTime.Now);
                if (t.Ticks < 0)
                    t = new TimeSpan(0);

                if (GetYearsCount(t) > 0)
                    return YearsInForm(GetYearsCount(t));

                if (GetMonthCount(t) > 0)
                    return MonthInForm(GetMonthCount(t));

                string s = "";

                if (t.Days > 0)
                    s = DayInForm(t.Days) + " ";

                s += new DateTime(t.Ticks).ToLongTimeString();

                return s;
            }
        }

        public string AlarmTimeString
        {
            get
            {
                return AlertTime.ToString();
            }
        }

        void twiceMsg(object o)
        {
            try
            {
                try
                {
                    MainForm.mp.Stop();
                }
                catch { }

                MainForm.mp = new System.Windows.Media.MediaPlayer();
                MainForm.mp.Open(new Uri(SoundFilename));
                MainForm.mp.Play();
                MessageBox.Show(MessageText, "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MainForm.mp.Stop();
            }
            catch { }
        }

        void msg(object o)
        {
            MessageBox.Show(MessageText, "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void Do()
        {
            if (!isUsed)
            {
                isUsed = true;
                try
                {
                    if (PlaySound && ShowMessage)
                    {
                        Thread t = new Thread(new ParameterizedThreadStart(twiceMsg));
                        t.Start();
                    }
                    else
                    {

                        try
                        {
                            if (PlaySound)
                            {
                                try
                                {
                                    MainForm.mp.Stop();
                                }
                                catch { }

                                MainForm.mp = new System.Windows.Media.MediaPlayer();
                                MainForm.mp.Open(new Uri(SoundFilename));
                                MainForm.mp.Play();
                            }
                        }
                        catch { }

                        try
                        {
                            if (ShowMessage)
                            {
                                Thread t = new Thread(new ParameterizedThreadStart(msg));
                                t.Start();
                            }
                        }
                        catch { }
                    }
                }
                catch { }

                try
                {
                    if (KillProcess)
                    {
                        if (KillByName)
                        {
                            var p = Process.GetProcessesByName(KillProcessName);
                            foreach (var item in p)
                            {
                                item.Kill();
                            }
                        }
                        else
                        {
                            WindowsManagerAPI.KillWindowsByName(KillWindowName);
                        }
                    }
                }
                catch { }

                try
                {
                    if (LaunchProgram)
                        System.Diagnostics.Process.Start(LaunchString, LaunchParametrsString);
                }
                catch { }

                try
                {
                    if (TurnComputerOff)
                    {
                        switch (PowerMode)
                        {
                            case PowerMode.TurnOff:
                                PowerAPI.Shutdown(ForceShutdown);
                                break;
                            case PowerMode.Restart:
                                PowerAPI.Restart(ForceShutdown);
                                break;
                            case PowerMode.Lock:
                                PowerAPI.Lock();
                                break;
                            case PowerMode.Sleep:
                                Application.SetSuspendState(PowerState.Suspend, ForceShutdown, false);
                                break;
                            case PowerMode.Hibernate:
                                Application.SetSuspendState(PowerState.Hibernate, ForceShutdown, false);
                                break;
                            default:
                                break;
                        }
                    }
                }
                catch { }
            }
        }
    }
}
