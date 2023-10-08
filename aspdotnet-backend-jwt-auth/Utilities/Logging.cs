using System;
using System.IO;

namespace JWTBackendAuth.Utilities
{
    public enum LogLevel
    {
        Info,
        Warning,
        Error
    }

    public static class Logger
    {
        private static readonly string logFolderPath = "./logs";
        private static readonly object lockObject = new object();

        private static string GetLogFilePath()
        {
            // Check if the logs directory exists
            if (!Directory.Exists(logFolderPath))
            {
                Directory.CreateDirectory(logFolderPath);
            }

            // Return the file path based on today's date
            string todayDate = DateTime.Now.ToString("yyyy-MM-dd");
            return Path.Combine(logFolderPath, $"{todayDate}.log");
        }

        public static void Log(LogLevel level, string message)
        {
            lock (lockObject)
            {
                string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] - {message}";
                string logFilePath = GetLogFilePath();

                try
                {
                    using (StreamWriter writer = new StreamWriter(logFilePath, true))
                    {
                        writer.WriteLine(logMessage);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error writing to log file: {ex.Message}");
                }
            }
        }
    }
}
