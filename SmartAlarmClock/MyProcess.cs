using System.Diagnostics;

namespace SmartAlarmClock
{
    /// <summary>
    /// Класс для "заглушки", используемый в SortingView
    /// </summary>
    public class MyProcess
    {
        public MyProcess()
            : base()
        {
        }

        public int Id { get; set; }
        public string ProcessName { get; set; }
        public string Description { get; set; }

        public static implicit operator MyProcess(Process p)
        {
            return new MyProcess() { Id = p.Id, ProcessName = p.ProcessName, Description = p.MainModule.FileVersionInfo.FileDescription };
        }
    }
}
