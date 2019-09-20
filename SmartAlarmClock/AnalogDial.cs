using SmartAlarmClock.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SmartAlarmClock
{
    /// <summary>
    /// Реализует класс элемента управления для отрисовки циферблата аналоговых часов
    /// </summary>
    public class AnalogDial : PictureBox
    {
        bool designMode = (LicenseManager.UsageMode == LicenseUsageMode.Designtime);
        public AnalogDial()
        {
            ImageToDraw = Resources.ClockDial;
            Image = ImageToDraw;
            this.SizeMode = PictureBoxSizeMode.Zoom;
            SecondPen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
            MinutePen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
            HourPen.EndCap = System.Drawing.Drawing2D.LineCap.Round;

            if (!designMode)
            {
                Timer t = new Timer();
                t.Tick += TimerTick;
                t.Interval = 41;
                t.Start();
                Draw();
            }
        }

        public Image ImageToDraw = null;

        public int CenterDotRadius = 5;
        public Brush CenterDotBrush = Brushes.Black;

        public Pen SecondPen = new Pen(Color.Black, 1.5F);
        public Pen SecondBackPen = new Pen(Color.Black, 1.5F);
        public Pen MinutePen = new Pen(Color.Black, 3);
        public Pen HourPen = new Pen(Color.Black, 6);

        public int SecondLength = 100;
        public int SecondBackLength = 20;
        public int MinuteLength = 80;
        public int HourLength = 60;

        public void Draw()
        {
            Bitmap b = new Bitmap(ImageToDraw);
            Graphics g = Graphics.FromImage(b);
            
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;


            DateTime time = DateTime.Now;

            Point center = new Point(b.Width / 2, b.Height / 2);

            double SecondAngle = time.Second * Math.PI / 30 + time.Millisecond * Math.PI / 30000;
            double MinuteAngle = time.Minute * Math.PI / 30 + time.Second * Math.PI / 1800;
            double HourAngle = time.Hour * Math.PI / 6 + time.Minute * Math.PI / 360 + time.Second * Math.PI / 21600;//охлол

            PointF SecondPt = new PointF(
                (float)(Math.Sin(SecondAngle) * SecondLength) + center.X,
                (float)(-Math.Cos(SecondAngle) * SecondLength) + center.Y);

            PointF SecondBackPt = new PointF(
                (float)(Math.Sin(SecondAngle + Math.PI) * SecondBackLength) + center.X,
                (float)(-Math.Cos(SecondAngle + Math.PI) * SecondBackLength) + center.Y);

            PointF MinutePt = new PointF(
                (float)(Math.Sin(MinuteAngle) * MinuteLength) + center.X,
                (float)(-Math.Cos(MinuteAngle) * MinuteLength) + center.Y);

            PointF HourPt = new PointF(
                (float)(Math.Sin(HourAngle) * HourLength) + center.X,
                (float)(-Math.Cos(HourAngle) * HourLength) + center.Y);


            Rectangle CenterDot = new Rectangle(
                center.X - CenterDotRadius, center.Y - CenterDotRadius, 
                CenterDotRadius * 2, CenterDotRadius * 2);

            g.DrawLine(HourPen, center, HourPt);
            g.DrawLine(MinutePen, center, MinutePt);
            g.DrawLine(SecondPen, center, SecondPt);
            g.DrawLine(SecondBackPen, center, SecondBackPt);

            g.FillEllipse(CenterDotBrush, CenterDot);

            g.Flush();
            Image = b;
        }

        public void TimerTick(object s, EventArgs e)
        {
            if (!DesignMode)
                Draw();
        }
    }
}
