using NLog;
using NLog.Targets;
using System;
using NlogManager = NLog.LogManager;

namespace CoreProject.LogManager
{
    public class Commander
    {
        public Commander(string connectionstring)
        {
            try
            {
                SetDatabaseConfig(connectionstring);
            }
            catch (Exception ex)
            {
                WriteLog(LogLevel.Error, "Connection Error", ex);
            }
        }
        Logger logger = NlogManager.GetCurrentClassLogger();       

        public void WriteLog(LogLevel loglevel, string Message, Exception ex = null)
        {
            logger.Log(loglevel, ex, Message);
        }

        public static void SetDatabaseConfig(string connectionstring)
        {
            var databaseTarget = (DatabaseTarget)NlogManager.Configuration.FindTargetByName("db");
            databaseTarget.ConnectionString = connectionstring;
            NlogManager.ReconfigExistingLoggers();

        }      

    }
}
