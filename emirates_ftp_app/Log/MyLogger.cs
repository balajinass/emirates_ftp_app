using NLog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Web;

namespace emirates_ftp_app.Log
{
    internal class MyLogger : ILog
    {
        //SingleTon pattern example. Only one instance of this class can be instanciated
        private static MyLogger instance; // SingleTon design pattern. single instance of this class.
        private static Logger logger; // static variable to hold a sigle instance of the nLog logger.

        //SingleTon design pattern. - private constracor
        private MyLogger()
        {

        }

        //SingleTon design pattern. - this function creates an instance of the class if it has not yet been instanciate
        // exists int he program, then send them the reference to the original.

        public static MyLogger GetInstance()
        {
            if (instance == null)
                instance = new MyLogger();
            return instance;
        }

        private ILogger GetLogger(string theLogger)
        {
            if (MyLogger.logger == null)
                MyLogger.logger = LogManager.GetLogger(theLogger);
            return MyLogger.logger;
        }

        public void Debug(string message, string arg = null)
        {
            if (arg == null)
                GetLogger("myAppLoggerRules").Debug(message);
            else
                GetLogger("myAppLoggerRules").Debug(message, arg);
        }

        public void Error(string message, string arg = null)
        {
            if (arg == null)
                GetLogger("myAppLoggerRules").Error(message);
            else
                GetLogger("myAppLoggerRules").Error(message, arg);
        }

        public void Info(string message, string arg = null)
        {
            if (arg == null)
                GetLogger("myAppLoggerRules").Info(message);
            else
                GetLogger("myAppLoggerRules").Info(message, arg);
        }

        public void Warning(string message, string arg = null)
        {
            if (arg == null)
                GetLogger("myAppLoggerRules").Warn(message);
            else
                GetLogger("myAppLoggerRules").Warn(message, arg);
        }
    }
}
