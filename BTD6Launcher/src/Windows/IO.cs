using System;
using System.Runtime.InteropServices;
using Btd6Launcher.Windows.NativeMethods;

namespace Btd6Launcher.Windows.IO
{

    static class IO
    {


        public static IntPtr LoadLibrary(string path)
        {
            IntPtr Handle = NMethods.LoadLibrary(path);
            if (Handle == IntPtr.Zero)
            {
                throw new Exception("Error load library. WinAPI error code: " + Marshal.GetLastWin32Error());
            }

            return Handle;
        }


    }



}