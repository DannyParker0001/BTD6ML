using System;
using System.Runtime.InteropServices;

namespace Btd6Launcher.Windows.IO
{

    static class IO
    {
        private static class WinAPI
        {
            //EntryPoint = "LoadLibraryA"
            [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern IntPtr LoadLibrary(string dllToLoad);
        }


        public static IntPtr LoadLibrary(string path)
        {
            IntPtr Handle = WinAPI.LoadLibrary(path);
            if (Handle == IntPtr.Zero)
            {
                throw new Exception("Error load library. WinAPI error code: " + Marshal.GetLastWin32Error());
            }

            return Handle;
        }


    }



}