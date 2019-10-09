using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using Btd6Launcher.Utilities;

namespace Btd6Launcher.Steam
{
    static class SteamUtils
    {
        public const UInt64 BTD6AppID = 960090;
        public const string BTD6Name = "BloonsTD6";

        public static string GetGameDir(UInt64 appid, string gameName)
        {

            //
            // Check if game is installed
            //

            int isGameInstalled = (int)Registry.GetValue(Registry.CurrentUser + "\\Software\\Valve\\Steam\\Apps\\" + appid, "Installed", null);

            if (isGameInstalled != 1)
            {
                throw new Exception("Error, " + gameName + " not installed!");
            }


            //
            // Get game Directory...
            //

            string steamDir = (string)Registry.GetValue(Registry.CurrentUser + "\\Software\\Valve\\Steam", "SteamPath", null);

            if (steamDir == null)
            {
                throw new Exception("Error, Failed to find steam");
            }

            string configFileDir = steamDir + "\\steamapps\\libraryfolders.vdf";

            List<string> SteamLibDirs = new List<string>();
            SteamLibDirs.Add(Utils.UnixToWindowsPath(steamDir)); // This steam Directory is always here.


            string[] configFile = File.ReadAllLines(configFileDir);

            for(int i = 0; i < configFile.Length; i++)
            {
                // To Example lines are
                // 	"ContentStatsID"		"-4535501642230800231"
                // "1"     "C:\\SteamLibrary"
                // So, we scan for the items in quotes, if the first one is numeric, then the second one will be a steam library.

                Regex reg = new Regex("\".*?\"");
                MatchCollection matches = reg.Matches(configFile[i]);


                for(int match = 0; match < matches.Count; match++)
                {
                    if(match == 0)
                    {

                        if (int.TryParse(Utils.StripQuotes(matches[match].Value.ToString()), out int n)){
                           
                            // We dont actually need N, we just need to check if the value is an integer.
                        }
                        else
                        {
                            break;
                        }

                    }

                    if(match == 1)
                    {
                        SteamLibDirs.Add(Utils.UnixToWindowsPath(Utils.StripQuotes(matches[match].Value.ToString())));
                    }
                }

            }

            for(int i = 0; i < SteamLibDirs.Count; i++)
            {
                if(Directory.Exists(SteamLibDirs[i] + "\\steamapps\\common\\" + gameName))
                {
                    return SteamLibDirs[i] + "\\steamapps\\common\\" + gameName;
                }
            }

            throw new Exception("Error, " + gameName + "'s Directory not found!");
            return "";
        }


    }
}
