using Microsoft.Win32;
using SmartAlarmClock.Properties;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SmartAlarmClock
{
    /// <summary>
    /// Главное окно приложения
    /// </summary>
    public partial class MainForm : Form
    {
        #region API
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hwnd, int cmd);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        #endregion
        public MainForm()
        {
            InitializeComponent();
            
            if (Settings.Default.IsFirstStart)
            {
                IsInAutorun = true;
                Settings.Default.IsFirstStart = false;
                Settings.Default.Save();
            }

            RegisterSetting("Events", new SortedList<long, Alert>());
            RegisterSetting("LastAlert", new Alert());

            FillList();

            Timer t = new Timer() { Enabled = true, Interval = 1000 };
            t.Tick += delegate(object s, EventArgs e)
            {
                FillList();
            };

            list1.List1.MouseDoubleClick += delegate(object sender, System.Windows.Input.MouseButtonEventArgs e)
            {
                if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
                    EditClick(null, new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, e.ClickCount, 0, 0, 0));
            };

            ShowWindow(Handle, 0);
        }

        public static string ProgramName = "Smart Alarm Clock";
        public static string RegPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        public static string Args = "startup";
        public static System.Windows.Media.MediaPlayer mp = new System.Windows.Media.MediaPlayer();

        public static bool IsInAutorun
        {
            get
            {
                try
                {
                    RegistryKey regKey = Registry.CurrentUser.OpenSubKey(RegPath, true);
                    if (regKey != null)
                        return regKey.GetValue("Smart Alarm Clock") != null;
                }
                catch { }
                return false;
            }
            set
            {
                if (value)
                {
                    try
                    {
                        RegistryKey regKey = Registry.CurrentUser.OpenSubKey(RegPath, true);
                        if (regKey != null)
                            regKey.SetValue("Smart Alarm Clock", Application.ExecutablePath + " " + Args);
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        RegistryKey regKey = Registry.CurrentUser.OpenSubKey(RegPath, true);
                        if (regKey != null)
                            regKey.DeleteValue("Smart Alarm Clock", false);
                    }
                    catch { }
                }
            }
        }

        public static void RegisterSetting(string name, object DefaultValue)
        {
            try
            {
                if (IsSettingExists(name))
                    return;

                //try
                //{
                //    if (Settings.Default[name] != null)
                //        Settings.Default.Properties.Remove(name);
                //}
                //catch { }

                SettingsProperty sp = new SettingsProperty(name);
                sp.DefaultValue = DefaultValue;
                sp.PropertyType = DefaultValue.GetType();
                sp.Provider = Settings.Default.Providers["LocalFileSettingsProvider"];
                sp.Attributes.Add(typeof(UserScopedSettingAttribute), new UserScopedSettingAttribute());
                sp.SerializeAs = SettingsSerializeAs.Binary;

                Settings.Default.Properties.Add(sp);
            }
            catch { }
        }

        public static bool IsSettingExists(string name)
        {
            try
            {
                Settings.Default[name].ToString();
                return true;
            }
            catch { return false; }
        }

        public static void SetSettingsValue(string name, object value)
        {
            if (!IsSettingExists(name))
            {
                RegisterSetting(name, value);
            }

            Settings.Default[name] = value;
        }
        
        public void FillList()
        {
            try
            {
                SortedList<long, Alert> ev = Settings.Default["Events"] as SortedList<long, Alert>;
                SortedList<long, Alert> nev = new SortedList<long, Alert>();
                var l = list1.List1;
                var j = l.SelectedItem;
                l.Items.Clear();

                foreach (var item in ev)
                {
                    if (item.Key - DateTime.Now.Ticks <= 0)
                    {
                        item.Value.Do();
                    }
                    else
                    {
                        l.Items.Add(item.Value);
                        nev.Add(item.Key, item.Value);
                    }
                }
                if (nev != ev)
                {
                    Settings.Default["Events"] = nev;
                    Settings.Default.Save();
                }
                l.SelectedItem = j;
            }
            catch { }
        }

        private void SettingsClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                SettingsForm sf = new SettingsForm();
                sf.ShowDialog(this);
            }
        }

        private void ButtonAddTaskClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                try
                {
                    AddTaskForm frm = new AddTaskForm();
                    try
                    {
                        if (Settings.Default.RestoreLast)
                        {
                            frm.CurrentAlarm = Settings.Default["LastAlert"] as Alert;
                            frm.CurrentAlarm.isUsed = false;
                        }
                    }
                    catch { }

                    frm.dateTimePicker1.Value = DateTime.Now.AddHours(1);

                    if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        var ev = Settings.Default["Events"] as SortedList<long, Alert>;
                        Alert a = frm.CurrentAlarm;
                        ev.Add(a.AlertTime.Ticks, a);

                        Settings.Default["Events"] = ev;
                        Settings.Default["LastAlert"] = frm.CurrentAlarm;
                        Settings.Default.Save();

                        list1.List1.SelectedIndex++;
                        FillList();
                    }
                }
                catch { }
            }
        }

        private void RemoveItem(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (list1.List1.SelectedItem != null)
                {
                    int i = list1.List1.SelectedIndex;

                    Alert a = list1.List1.SelectedItem as Alert;
                    list1.List1.Items.Remove(a);
                    var ev = Settings.Default["Events"] as SortedList<long, Alert>;
                    ev.Remove(a.AlertTime.Ticks);
                    Settings.Default["Events"] = ev;
                    Settings.Default.Save();

                    list1.List1.SelectedIndex = Math.Min(i, ev.Count - 1);
                }
            }
        }

        private void EditClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                try
                {
                    if (list1.List1.SelectedItem == null)
                        return;

                    var f = new AddTaskForm(true);
                    var al = list1.List1.SelectedItem as Alert;

                    f.CurrentAlarm = al;

                    if (f.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                    {
                        var cal = f.CurrentAlarm;
                        var ev = Settings.Default["Events"] as SortedList<long, Alert>;

                        int i = list1.List1.SelectedIndex;
                        ev.Remove(al.AlertTime.Ticks);
                        ev.Add(cal.AlertTime.Ticks, cal);
                        list1.List1.SelectedIndex = i;
                    }
                }
                catch { }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Hide();
                e.Cancel = true;
            }
        }
        
        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
            SetForegroundWindow(Handle);
        }

        private void остановитьСигналToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                mp.Stop();
            }
            catch { }
        }

        private void выйтиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.Default.Save();
            Application.Exit();
        }
        
        private void pictureBox3_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Application.Exit();
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                открытьToolStripMenuItem_Click(null, null);
        }
    }
}
