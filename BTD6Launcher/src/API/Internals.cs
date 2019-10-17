using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using System.Security.Cryptography;
using Btd6Launcher.Memory;
using Btd6Launcher.uiBackend;

//
// The API internals is used for initialising the API
// Example1: Finding methods offsets
// Example2: Getting handle to BTD6 to read/write memory.
//

namespace Btd6Launcher.API.Internals
{

    class UnityMethods
    {
        //
        // Methods for initialising the il2cpp VM
        //

        [DllImport("GameAssembly.dll")]
        public static extern void il2cpp_set_commandline_arguments(int argc, char[][] argv, char[] basedir);

        [DllImport("GameAssembly.dll")]
        public static extern void il2cpp_set_config_dir(char[] config_path);

        [DllImport("GameAssembly.dll")]
        public static extern void il2cpp_set_data_dir(char[] data_path);

        [DllImport("GameAssembly.dll")]
        public static extern void il2cpp_debugger_set_agent_options(char[] options);

        [DllImport("GameAssembly.dll")]
        public static extern void il2cpp_init(char[] domain_name);

        [DllImport("GameAssembly.dll")]
        public static extern void il2cpp_set_config(char[] executable_path);

        //
        // Methods used by the API
        //

        [DllImport("GameAssembly.dll")]
        public static extern void il2cpp_domain_get();

        [DllImport("GameAssembly.dll")]
        public static extern void il2cpp_domain_get_assemblies();

        [DllImport("GameAssembly.dll")]
        public static extern void il2cpp_assembly_get_image();

        [DllImport("GameAssembly.dll")]
        public static extern void il2cpp_class_from_name();

        [DllImport("GameAssembly.dll")]
        public static extern void il2cpp_class_get_method_from_name();
    }

    class AheadOfTime
    {
        // C# uses UTF-16, Unity uses UTF-8, Using bytes instead of chars.
        // 70 75 62 6C 69 63 2E 61 70 70 2D 63 61 74 65 67 6F 72 79 2E 67 61 6D 65 73
        static readonly byte[] BTD6VerisonInfoSignature = {
            0x70, 0x75, 0x62, 0x6C, 0x69, 0x63, 0x2E,   // public.
            0x61, 0x70, 0x70, 0x2D, 0x63, 0x61, 0x74, 0x65, 0x67, 0x6F, 0x72, 0x79, 0x2E, // app-category.
            0x67, 0x61, 0x6D, 0x65, 0x73 }; // games
        // public.app-category.games

        public static string getBTD6MD5(string btd6dir)
        {
            // These are IDisposable, so aparenretly you've got to use these monkey looking using statments
            using (MD5 md5 = MD5.Create())
            {
                using (FileStream stream = File.OpenRead(btd6dir + "\\BloonsTD6_Data\\globalgamemanagers"))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-","").ToLowerInvariant();
                }
            }
        }
        public static unsafe Version getBTD6Version(string btd6Dir)
        {
            // GlobalGameManagers contains version string.
            // Can move by a few bytes in updates, instead scan for it.

            Version v;

            byte[] file = File.ReadAllBytes(btd6Dir + "\\BloonsTD6_Data\\globalgamemanagers");

            Int64 VersionStructOffset = 0;

            fixed (byte* pFile = file)
            {
                // The signature is only a few bytes away, the next period should always be the peroid in the version (12.2)
                VersionStructOffset = MemoryUtils.AOBScan(pFile, file.Length, BTD6VerisonInfoSignature);

                if (VersionStructOffset == 0)
                {
                    throw new Exception("Error, failed to find BTD6's version");
                }

                // Now we scan for the peroid (12.2) the .

                for (Int64 i = VersionStructOffset + BTD6VerisonInfoSignature.Length; i < file.Length; i++)
                {
                    if (pFile[i] == 0x2E)
                    {
                        byte[] versionBytes = MemoryUtils.BytesBetweenNull(pFile, file.Length, i);

                        int periodPos = 0;
                        for(int k = 0; k < versionBytes.Length; k++)
                        {
                            if(versionBytes[k] == 0x2e)
                            {
                                periodPos = k;
                                break;
                            }
                        }

                        byte[] majorVersionBytes = new byte[periodPos];
                        byte[] minorVersionBytes = new byte[versionBytes.Length - (periodPos + 1)];

                        Buffer.BlockCopy(versionBytes, 0, majorVersionBytes, 0, periodPos);
                        Buffer.BlockCopy(versionBytes, periodPos + 1, minorVersionBytes, 0, versionBytes.Length - (periodPos + 1));

                        string majorVersionString = Encoding.UTF8.GetString(majorVersionBytes);
                        string minorVersionString = Encoding.UTF8.GetString(minorVersionBytes);

                        

                        v = new Version(int.Parse(majorVersionString), int.Parse(minorVersionString));

                        return v;
                    }
                }
            }

            throw new Exception("Error, unable to get BTD6 Version");
            
        }
        public static unsafe void Initialise(Config config)
        {
            // Not doing MD5, ensure that people can mod all files without fucking up the launcher.

            Version version = getBTD6Version(config.btd6Dir);

            Console.WriteLine("BTD6 Version: " + version.Major + "." + version.Minor);

            if(config.originalGameAssemblyVersion != version)
            {

            }

            


        }
        // Copy BTD6 files to ML directory

        // Use our own UnityEngine.dll to be able to prevent the game from launching.

        // Use the Il2cpp methods, Unity will maintain this for us, no modifications should have to be made.


    }

    class Runtime
    {


        // Initialises the exported methods & runtime.
        public unsafe void initialise(byte* pBin, string sModDir)
        {
            // Initialisation
            // Array of char arrays.
            char[] utf8string = UTF8Encoding.UTF8.GetChars(UTF8Encoding.UTF8.GetBytes(sModDir + "\\BloonsTD6.exe"));
            char[][] args = { utf8string };


            UnityMethods.il2cpp_set_commandline_arguments(1, args, null);





        // Callable methods
    }

    }
}
