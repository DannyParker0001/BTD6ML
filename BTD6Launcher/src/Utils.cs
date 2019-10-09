using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Btd6Launcher.Utilities
{
    class Utils
    {
        // Takes any quotation marks out of a string.
        public static string StripQuotes(string str)
        {
            return str.Replace("\"", "");
        }

        public static string UnixToWindowsPath(string UnixPath)
        {
            return UnixPath.Replace("/", "\\");
        }
    }
}
