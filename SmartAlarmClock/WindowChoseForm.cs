using System;
using System.Windows.Forms;

namespace SmartAlarmClock
{
    /// <summary>
    /// Окно, позволяющее выбрать другие окна для закрытия
    /// </summary>
    public partial class WindowChoseForm : Form
    {
        public WindowChoseForm()
        {
            InitializeComponent();

            UpdateList();
        }

        public void UpdateList()
        {
            var l = WindowsManagerAPI.GetAllWindowsText();
            windowsList1.listView1.Items.Clear();

            foreach (var item in l)
            {
                windowsList1.listView1.Items.Add(new MyWindow(item));
            }

            label1.Text = "Окон: " + l.Count;
        }

        public string SelectedWindowText { get; set; }

        private void button3_Click(object sender, EventArgs e)
        {
            UpdateList();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (windowsList1.listView1.SelectedIndex == -1)
                    MessageBox.Show("Выберите процесс для вставки!", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                {
                    SelectedWindowText = (windowsList1.listView1.SelectedItem as MyWindow).Title;
                    DialogResult = System.Windows.Forms.DialogResult.OK;
                    Close();
                }
            }
            catch { }
        }
    }
}
