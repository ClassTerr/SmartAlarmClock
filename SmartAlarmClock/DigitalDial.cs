using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SmartAlarmClock
{
    /// <summary>
    /// Реализует класс элемента управления для отрисовки циферблата цифровых часов    
    /// </summary>
    public class DigitalDial : PictureBox
    {
        public DigitalDial()
        {
            bool designMode = (LicenseManager.UsageMode == LicenseUsageMode.Designtime);
            if (!designMode)
            {
                Timer t = new Timer();
                t.Tick += TimerTick;
                t.Interval = 100;
                t.Start();
            }

            Font = new System.Drawing.Font("Moire ExtraBold", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            Controls.Add(TextLabel);
            Draw();
        }

        [Category("New")]
        [Browsable(true)]
        [Description("Задает шрифт, которым будут рисоваться цифры циферблата")]
        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                TextLabel.Font = value;
                base.Font = value;
            }
        }


        public Image ImageToDraw = null;
        public Brush TextBrush = Brushes.Black;
        public Label TextLabel = new Label() { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };

        public void Draw()
        {
            TextLabel.Text = DateTime.Now.ToLongTimeString() + "\n" + DateTime.Now.ToShortDateString();
        }

        public void TimerTick(object s, EventArgs e)
        {
            if (!DesignMode)
                Draw();
        }
    }
}
