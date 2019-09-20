using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace SmartAlarmClock
{
    /// <summary>
    /// Окно выбора процесса для завершения
    /// </summary>
    public partial class ProcessListForm : Form
    {
        public ProcessListForm()
        {
            InitializeComponent();
            UpdateList();
        }

        public void UpdateList()
        {
            sortingView1.Items.Clear();

            var l = Process.GetProcesses();

            foreach (var item in l)
            {
                try
                {
                    sortingView1.Items.Add((MyProcess)item);
                }
                catch { }
            }

            label1.Text = "Процессов: " + sortingView1.Items.Count;
        }

        public MyProcess SelectedProcess { get; set; }

        private void button3_Click(object sender, EventArgs e)
        {
            UpdateList();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (sortingView1.list.SelectedIndex == -1)
                MessageBox.Show("Выберите процесс для вставки!", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                SelectedProcess = sortingView1.list.SelectedItem as MyProcess;
                DialogResult = System.Windows.Forms.DialogResult.OK;
                Close();
            }
        }
    }
}
