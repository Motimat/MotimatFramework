using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Razor.Entity.Interfaces
{
    public interface IFrontLogger
    {
        void Log(string content, LogMode mode, TimeSpan time);
        void Log(string content, LogMode mode);
        void Log(string content);

        List<LogModel> GetAllLogs();

        void ClearLogs();

        public static event EventHandler LogEvent;
    }


    public class LogModel
    {
        public string LogContent { get; set; }
        public DateTime CreatedDate { get; set; }
        public int index { get; set; }
        public LogMode LogMode { get; set; }
        public TimeSpan? Time { get; set; }
    }
    public enum LogMode
    {
        Info = 0,
        Error = 1
    }
}
