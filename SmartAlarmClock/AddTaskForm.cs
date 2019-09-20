using System;
using System.IO;
using System.Windows.Forms;

namespace SmartAlarmClock
{
    public partial class AddTaskForm : Form
    {
        public AddTaskForm()
        {
            InitializeComponent();
            dateTimePicker1.Value = DateTime.Now.AddHours(1);

            radioButton1.Checked = radioButton7.Checked = true;
        }

        public AddTaskForm(bool isEditing)
        {
            InitializeComponent();
            if (isEditing)
            {
                button5.Text = "Изменить";
                button5.Image = SmartAlarmClock.Properties.Resources.Edit;
            }
            else dateTimePicker1.Value = DateTime.Now.AddHours(1);
        }

        public Alert CurrentAlarm
        {
            get
            {
                Alert a = new Alert();
                a.AlertTime = dateTimePicker1.Value;
                a.ForceShutdown = checkBox3.Checked;
                a.KillByName = radioButton1.Checked;
                a.KillProcess = checkBox5.Checked;
                a.KillProcessName = memoryTextBox3.Text;
                a.KillWindowName = memoryTextBox4.Text;
                a.LaunchParametrsString = memoryTextBox2.Text;
                a.LaunchProgram = checkBox6.Checked;
                a.LaunchString = memoryTextBox1.Text;
                a.MessageText = memoryTextBox6.Text;
                a.PlaySound = checkBox1.Checked;
                a.KillByName = radioButton1.Checked;

                a.PowerMode = PowerMode.Lock;
                if (radioButton3.Checked)
                    a.PowerMode = PowerMode.TurnOff;
                if (radioButton4.Checked)
                    a.PowerMode = PowerMode.Restart;
                if (radioButton5.Checked)
                    a.PowerMode = PowerMode.Hibernate;
                if (radioButton6.Checked)
                    a.PowerMode = PowerMode.Sleep;

                a.ShowMessage = checkBox2.Checked;
                a.SoundFilename = memoryTextBox5.Text;
                a.TurnComputerOff = checkBox4.Checked;

                return a;
            }
            set
            {
                dateTimePicker1.Value = value.AlertTime;
                checkBox3.Checked = value.ForceShutdown;
                radioButton1.Checked = value.KillByName;
                checkBox5.Checked = value.KillProcess;
                memoryTextBox3.Text = value.KillProcessName;
                memoryTextBox4.Text = value.KillWindowName;
                memoryTextBox2.Text = value.LaunchParametrsString;
                checkBox6.Checked = value.LaunchProgram;
                memoryTextBox1.Text = value.LaunchString;
                memoryTextBox6.Text = value.MessageText;
                checkBox1.Checked = value.PlaySound;
                checkBox2.Checked = value.ShowMessage;
                memoryTextBox5.Text = value.SoundFilename;
                checkBox4.Checked = value.TurnComputerOff;
                radioButton1.Checked = value.KillByName;
                radioButton2.Checked = !value.KillByName;

                switch (value.PowerMode)
                {
                    case PowerMode.TurnOff:
                        radioButton3.Checked = true;
                        break;
                    case PowerMode.Restart:
                        radioButton4.Checked = true;
                        break;
                    case PowerMode.Lock:
                        radioButton7.Checked = true;
                        break;
                    case PowerMode.Sleep:
                        radioButton6.Checked = true;
                        break;
                    case PowerMode.Hibernate:
                        radioButton5.Checked = true;
                        break;
                }
            }
        }

        #region Player
        private void OpenSoundFile(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                memoryTextBox5.Text = openFileDialog1.FileName;
            }
        }

        System.Windows.Media.MediaPlayer mp = new System.Windows.Media.MediaPlayer();

        private void Play(object sender, EventArgs e)
        {
            if (File.Exists(memoryTextBox5.Text))
            {
                try
                {
                    if (mp.Source == null || mp.Source.LocalPath != memoryTextBox5.Text)
                        mp.Open(new Uri(memoryTextBox5.Text));
                    mp.Play();
                    mp.MediaEnded += delegate(object s, EventArgs args)
                    {
                        mp.Stop();
                    };
                }
                catch
                {
                    MessageBox.Show("Ошибка воспроизведения!");
                }
            }
            else MessageBox.Show("Файла не существует!");
        }

        private void Stop(object sender, EventArgs e)
        {
            mp.Stop();
        }

        private void Pause(object sender, EventArgs e)
        {
            mp.Pause();
        }
        #endregion

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Stop(null, null);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            memoryTextBox1.SaveParametrs();
            memoryTextBox2.SaveParametrs();
            memoryTextBox3.SaveParametrs();
            memoryTextBox4.SaveParametrs();
            memoryTextBox5.SaveParametrs();
            memoryTextBox6.SaveParametrs();
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                memoryTextBox1.Text = openFileDialog2.FileName;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                var p = new ProcessListForm();
                if (p.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    memoryTextBox3.Text = p.SelectedProcess.ProcessName;
                    radioButton1.Checked = true;
                }
            }
            catch { }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                var f = new WindowChoseForm();
                if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    memoryTextBox4.Text = f.SelectedWindowText;
                    radioButton2.Checked = true;
                }
            }
            catch { }
        }
    }
}
