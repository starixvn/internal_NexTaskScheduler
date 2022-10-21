namespace NexTaskScheduler.Core
{
    using System;
    using NLog;
    using NLog.Web;

    public class LogServiceProvider : ILogServiceProvider
    {
        private readonly Logger infoLogger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        public void WriteLogFileInfo(string logMessage)
        {
            infoLogger.Error(logMessage);
        }

        public void WriteLogFileException(Exception exception)
        {
            infoLogger.Error(exception);
        }
    }
}