using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.IO;

namespace Btd6Launcher.uiBackend
{
    public class Config
    {
        public string btd6Dir;
        public string modDir;
        public string configDir; //%USERPROFILE%\AppData\LocalLow

        public string originalGameAssembly;
        public Version originalGameAssemblyVersion;
    }


    public class ModPanel : DockPanel
    {
        public ModInfo modInfo = new ModInfo();

        public Point originMove;
        public Point originDrag;

        public bool isDragged = false;
    }

    public enum ModStatus : byte
    {
        Disabled = 0x0,
        Waiting = 0x1,

        MissingDependencies = 0xF0,

        Injected = 0x2,
        Patched = 0x3,

        Unknown = 0xFF,
    }

    public enum ModType : byte
    {
        AheadOfTime = 0x0,
        Runtime = 0x1
    }


    public class ModInfo
    {
        private Version InfoVersion = new Version(1, 0);
        public Version version; //Major, Minor, Build, Rev

        public string dir;
        public string name;
        public string shortDescription;
        public string longDescription;
        public string icon;

        public List<string> authors;
        public List<string> contact;

        public DateTime lastUpdated;
        public ulong fileSize;

        public ModStatus status;
        public ModType type;

        public List<string> dependencies;
        public List<string> extends;
    }
    public class ModAPI
    {
        private List<ModPanel> waitingForPatch = new List<ModPanel>();

        public void injectMod(ModPanel mod)
        {
            MessageBox.Show("TODO: Inject" + mod.modInfo.name);
        }

        public void addModToPatchList(ModPanel mod)
        {
            waitingForPatch.Add(mod);
        }

        public void removeModFromPatchList(ModPanel mod)
        {
            waitingForPatch.Remove(mod);
        }

        public void patchMods()
        {
            for(int i = 0; i < waitingForPatch.Count; i++)
            {

            }
        }

        public void patchMod()
        {

        }


    }
}
