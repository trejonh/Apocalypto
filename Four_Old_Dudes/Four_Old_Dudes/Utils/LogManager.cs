
using System;
using System.IO;

namespace Four_Old_Dudes.Utils
{
    public static class LogManager
    {
        private static readonly string BaseFileLocation = Environment.CurrentDirectory + @"\Logs";
        private static readonly string LogFile = BaseFileLocation + @".\log-"+DateTime.Today.ToString("MM-dd-yyyy")+".htm";
        private static StreamWriter _log;
        private static readonly string _head = "<html>\r\n<head>\r\n<link rel=\'stylesheet\'" +
                                      " href=\'https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-alpha.6/css/bootstrap.min.css\' " +
                                      "integrity=\'sha384-rwoIResjU2yc3z8GV/NPeZWAv56rSmLldC3R/AZzGRnGxQQKnKkoFVhFQhNUwEyJ\' crossorigin=" +
                                      "\'anonymous\'>\r\n<script src=\'https://code.jquery.com/jquery-3.1.1.slim.min.js\' " +
                                      "integrity=\'sha384-A7FZj7v+d/sdmMqp/nOQwliLvUsJfDHW+k9Omg/a/EheAdgtzNs3hpfag6Ed950n\' " +
                                      "crossorigin=\'anonymous\'></script>\r\n<script " +
                                      "src=\'https://cdnjs.cloudflare.com/ajax/libs/tether/1.4.0/js/tether.min.js\' " +
                                      "integrity=\'sha384-DztdAPBWPRXSA/3eYEEUWrWCy7G5KFbe8fFjk5JAIxUYHKkDx6Qin1DkWx51bBrb\' " +
                                      "crossorigin=\'anonymous\'></script>\r\n" +
                                      "<script src=\'https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0-alpha.6/js/bootstrap.min.js\' " +
                                      "integrity=\'sha384-vBWWzlZJ8ea9aCX4pEW3rVHjgjt7zpkNpZk+02D9phzyeVkE+jo0ieGizqPLForn\' " +
                                      "crossorigin=\'anonymous\'></script>\r\n<style>\r\n</style>\r\n</head>\r\n\r\n" +
                                      "<link rel='stylesheet' href='.\\log.css'><body>";

        private static readonly string _close = "\r\n<script='.\\log.js'></script>\r\n</body>\r\n</html>";

        public static void InitLogFile()
        {
            if(!Directory.Exists(BaseFileLocation))
                Directory.CreateDirectory(BaseFileLocation);
            if (File.Exists(LogFile))
                File.Delete(LogFile);
            var logFs = new FileStream(LogFile,FileMode.OpenOrCreate,FileAccess.Write);
            _log = new StreamWriter(logFs);
            _log.WriteLine(_head);
        }

        public static void LogError(string error)
        {
            const string eleO = "<error class=\'text-danger\'>";
            const string eleC = "</error>";
            _log.WriteLine(eleO + error + eleC);
        }

        public static void LogWarning(string warning)
        {
            const string eleO = "<warning class=\'text-warning\'>";
            const string eleC = "</warning>";
            _log.WriteLine(eleO + warning + eleC);
        }

        public static void Log(string log)
        {
            const string eleO = "<info class=\'text-info\'>";
            const string eleC = "</info>";
            _log.WriteLine(eleO+log+eleC);
        }

        public static void CloseLog()
        {
            _log.Write(_close);
            _log.Close();
        }
    }
}
