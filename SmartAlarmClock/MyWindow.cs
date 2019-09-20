
namespace SmartAlarmClock
{
    /// <summary>
    /// Класс для "заглушки", используемый в WindowsList
    /// </summary>
    public class MyWindow
    {
        public string Title { get; set; }

        public MyWindow(string s)
        {
            Title = s;
        }
    }
}
