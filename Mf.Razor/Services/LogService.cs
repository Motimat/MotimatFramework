using Mf.Razor.Entity.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mf.Razor.Services
{
    public class LogService:IFrontLogger
    {
        private int indexer = 1;
        public event EventHandler LogEvent;
        private List<LogModel> LogComments { get; set; } = new List<LogModel>();

        public virtual void Log(string content, LogMode mode, TimeSpan time)
        {
            LogComments.Add(new LogModel { CreatedDate = DateTime.Now,index = indexer,LogContent = content,LogMode = mode,Time = time});
            indexer++;
            LogEvent?.Invoke(null, EventArgs.Empty);
        }

        public virtual void Log(string content, LogMode mode)
        {
            LogComments.Add(new LogModel { CreatedDate = DateTime.Now, index = indexer, LogContent = content, LogMode = mode, Time = null });
            indexer++;
            LogEvent?.Invoke(null, EventArgs.Empty);
        }

        public virtual void Log(string content)
        {
            LogComments.Add(new LogModel { CreatedDate = DateTime.Now, index = indexer, LogContent = content, LogMode = LogMode.Info, Time = null });
            indexer++;
            LogEvent?.Invoke(null, EventArgs.Empty);
        }


        public virtual List<LogModel> GetAllLogs()
        {
            return LogComments.ToList();
        }

        public virtual void ClearLogs()
        {
            LogComments = new List<LogModel>();
            indexer = 0;
            Log("Clear Logs");
        }
    }
}
