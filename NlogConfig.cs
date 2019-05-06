using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using NLog.Targets;
using System.IO;
using System.Configuration;

namespace Produktionslager
{
    public static class NlogConfig
    {
        public static void Configure(string logFilePath)
        {
            // Create configuration object 
            var config = new LoggingConfiguration();

            // Create targets and add them to the configuration
            var consoleTarget = new ColoredConsoleTarget();
            config.AddTarget("console", consoleTarget);

            var fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);

            // Set target properties 
            //consoleTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} ${message}";
            consoleTarget.Layout = @"${message}";
            fileTarget.FileName = logFilePath;
            fileTarget.Layout = "${message}";

            //  Define rules
            var rule1 = new LoggingRule("*", LogLevel.Debug, consoleTarget);
            config.LoggingRules.Add(rule1);

            var rule2 = new LoggingRule("*", LogLevel.Debug, fileTarget);
            config.LoggingRules.Add(rule2);

            // Activate the configuration
            LogManager.Configuration = config;
        }

        public static string ConfigureNLog()
        {
            string logFolderPath = @"C:\temp\";
            string targetDir = Path.Combine(logFolderPath, DateTime.Now.ToString("yyyyMMdd_HHmmss"));          
            string appName = System.AppDomain.CurrentDomain.FriendlyName;
            string nLogFile = Path.Combine(targetDir, appName.Substring(0, appName.IndexOf(".")) + ".log");
            Configure(nLogFile);
            return nLogFile;
        }
    }


}
