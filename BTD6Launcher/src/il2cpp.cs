using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Btd6Launcher.il2cpp
{
    
    class il2cpp
    {
    }

    class Runtime
    {
        public unsafe class Methods
        {
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



            /*
                [DllImport("GameAssembly.dll")]
                public static extern void name();
             */


        }

        private unsafe class InitMethod
        {
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

        }

        // Initialises the exported methods & runtime.
        public unsafe void initialise(byte* pBin, string sModDir)
        {
            // Initialisation
            // Array of char arrays.
            char[] utf8string = UTF8Encoding.UTF8.GetChars(UTF8Encoding.UTF8.GetBytes(sModDir + "\\BloonsTD6.exe"));
            char[][] args = { utf8string };


            InitMethod.il2cpp_set_commandline_arguments(1, args, null);





        // Callable methods
    }

    }
}
