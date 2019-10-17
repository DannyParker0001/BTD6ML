using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Btd6Launcher.Log
{
    class Logger
    {
        public static void initialise(string configPath)
        {
            //FileStream ostrm;
            //StreamWriter writer;
            //TextWriter oldOut = Console.Out;
            //try
            //{
            //    ostrm = new FileStream("./Redirect.txt", FileMode.OpenOrCreate, FileAccess.Write);
            //    writer = new StreamWriter(ostrm);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("Cannot open Redirect.txt for writing");
            //    Console.WriteLine(e.Message);
            //    return;
            //}
            //Console.SetOut(writer);

            FileStream ostream;
            ostream = new FileStream(configPath + "\\log.txt", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter logWriter = new StreamWriter(ostream);
            Console.SetOut(logWriter);  
        }
        //
        // Console.WriteLine("ERROR: Test") should end up in
        //
        
    }
}
