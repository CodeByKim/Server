using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Util
{
    public static class Logger
    {
        public static void Info(string message)
        {
            var time = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            var header = $"{time} [INFO]";

            Console.WriteLine($"{header} {message}");
        }

        public static void Warnning(string message)
        {
            var time = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            var header = $"{time} [WARN]";

            Console.WriteLine($"{header} {message}");
        }

        public static void Error(string message)
        {
            var time = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            var header = $"{time} [ERROR]";

            Console.WriteLine($"{header} {message}");
        }
    }
}
