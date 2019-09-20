using SmartAlarmClock.Properties;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SmartAlarmClock
{
    /// <summary>
    /// Окно настроек приложения
    /// </summary>
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            checkBox2.Checked = IsInAutorun;
            checkBox1.Checked = Settings.Default.RestoreLast;
            numericUpDown1.Value = Settings.Default.MaxComboBoxListCapacity;
        }

        private bool IsInAutorun
        {
            get
            {
                return MainForm.IsInAutorun;
            }
            set
            {
                MainForm.IsInAutorun = value;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите сбросить всё в программе?", "Осторожно с этим!",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
            {
                Settings.Default.Reset();
                Settings.Default.Save();
                Application.Restart();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы уверены?", "Вопрос",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
            {
                int i = 1;
                while (MainForm.IsSettingExists("memoryTextBox" + i))
                {
                    MainForm.SetSettingsValue("memoryTextBox" + i, new List<string>());
                    i++;
                }

                Settings.Default["LastAlert"] = new Alert();
                Settings.Default.Save();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Settings.Default.RestoreLast = checkBox1.Checked;
            Settings.Default.MaxComboBoxListCapacity = (int)numericUpDown1.Value;
            IsInAutorun = checkBox2.Checked;
            Settings.Default.Save();
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            IsInAutorun = false;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            IsInAutorun = true;
        }
    }
}
