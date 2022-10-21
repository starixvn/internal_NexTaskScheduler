namespace NexTaskScheduler.Core
{
    using System;

    public interface ILogServiceProvider
    {
        void WriteLogFileInfo(string logMessage);

        void WriteLogFileException(Exception exception);
    }
}