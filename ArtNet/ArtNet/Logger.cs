using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtNet
{
    public static class Logger
    {

        public static void Log(string msg, int lvl) {
            Console.WriteLine(lvl + ": " + msg);

        }
    }

    public static class LogLevel {

        public const int INFO = 1;

        public const int WARNING = 0;

        public const int ERROR = -1;
    }
}
