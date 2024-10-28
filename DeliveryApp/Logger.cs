namespace DeliveryApp
{
    public class Logger
    {
        private readonly string _logPath;

        public Logger(string logPath)
        {
            _logPath = logPath;
        }

        public void LogInfo(string message) => Log("INFO", message);

        public void LogError(string message) => Log("ERROR", message);

        private void Log(string level, string message)
        {
            var logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}";
            File.AppendAllText(_logPath, logEntry + Environment.NewLine);
        }
    }
}
