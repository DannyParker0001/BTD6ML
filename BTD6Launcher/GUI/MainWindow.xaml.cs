using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.IO;

using Btd6Launcher.Windows.PE;
using Btd6Launcher.uiBackend;
using System.Runtime.InteropServices;
using Btd6Launcher.Windows.IO;
using Btd6Launcher.Steam;

using Newtonsoft.Json;

namespace Btd6Launcher
{
    /// <summary>
    /// All the GUI code.
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        Config BTD6Config = new Config();

        
        public MainWindow()
        {
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string roamingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            string configDirectory = roamingDirectory + @"\BloonsModLauncher";

            
            string btd6Dir = SteamUtils.GetGameDir(SteamUtils.BTD6AppID, SteamUtils.BTD6Name);
            BTD6Config.modDir = btd6Dir + "\\mods";

            if (!(Directory.Exists(BTD6Config.modDir)))
            {
                Directory.CreateDirectory(BTD6Config.modDir);
            }

            //
            // Mod file structure is as follows:
            //
            // +-- Mods
            // |    +-- ModName
            // |    |    +-- ModName.dll
            // |    |    +-- ModName.btd6modinfo (Json)
            // |
            // |    +-- ModName2
            // |    |   +-- ModName2.dll
            // |    |   +-- ModName2.btd6modinfo (Json)
            // |    
            // ...
            // 
            // Why not just use .json?
            // To allow mod creators to use .json files without confusing the launcher.
            //
            InitializeComponent();

            string[] modDirs = Directory.GetFiles(btd6Dir, "*.btd6modinfo", SearchOption.AllDirectories);

            ModInfo testInfo = new ModInfo();
            testInfo.name = "Test Mod";
            testInfo.type = ModType.AheadOfTime;
            testInfo.version = new Version(1, 0, 0);
            testInfo.shortDescription = "A mod for testing";
            testInfo.longDescription = "A mod that's used for testing the GUI of the moad loader.\n The mod can also be used to test the API!";
            testInfo.icon = "icon.png";
            testInfo.authors = new List<string> { "Danny Parker" };
            testInfo.contact = new List<string> { "Danny Parker#0001 on discord" };
            testInfo.lastUpdated = DateTime.Now;

            string TestModInfo = JsonConvert.SerializeObject(testInfo);

            //File.WriteAllText(@"C:\SteamLibrary\steamapps\common\BloonsTD6\Mods\TestMod\TestMod.btd6modinfo", TestModInfo);

            List<ModInfo> ModList = new List<ModInfo>();
            ModInfo modinfo = null;

            for(int i = 0; i < modDirs.Length; i++)
            {
                string json = File.ReadAllText(modDirs[i]);

                    modinfo = JsonConvert.DeserializeObject<ModInfo>(json);

                if(modinfo != null)
                {
                    ModList.Add(modinfo);
                }
            }

            for(int i = 0; i < ModList.Count; i++)
            {
                addMod(ModList[i]);
            }


        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DumpShit();
        }
        void DumpShit()
        {
            unsafe
            {
                //IntPtr filePtr = IO.LoadLibrary("C:\\SteamLibrary\\steamapps\\common\\BloonsTD6\\GameAssembly.dll");

                //byte* peFile = (byte*)(filePtr);

                //Dictionary<string, UInt64> MemoryExports = PE.dumpSymbolsFromMemory(peFile);
                //// Works!

                //byte[] Bin = File.ReadAllBytes("C:\\SteamLibrary\\steamapps\\common\\BloonsTD6\\GameAssembly.dll");


                //Dictionary<string, UInt64> FileExports;

                //fixed (byte* pBin = Bin)
                //{
                //    FileExports = PE.dumpSymbolsFromFile(pBin);
                //}


                int x = 5;
            }

        }

        public void createModPanel(string dir, string name, string desc, string imageDir,
        List<string> authors, List<string> contact, DateTime lastUpdated, ulong fileSize, ModType type)
        {
            ModPanel mpone = new ModPanel();
            mpone.modInfo.dir = dir;
            mpone.modInfo.name = name;
            mpone.modInfo.longDescription = desc;
            mpone.modInfo.authors = authors;
            mpone.modInfo.contact = contact;
            mpone.modInfo.lastUpdated = lastUpdated;
            mpone.modInfo.fileSize = fileSize;
            mpone.modInfo.type = type;
            mpone.modInfo.icon = imageDir;
            addModPanel(mpone, ModsDisabledSP);
        }

        void addMod(ModInfo modInfo)
        {
            ModPanel mp = new ModPanel();
            mp.modInfo = modInfo;
            addModPanel(mp, ModsDisabledSP);
        }

        void addModPanel(ModPanel mod, StackPanel destination)
        {
            Brush fontColour = (Brush)(this.FindResource("FontColour"));

            Brush backgroundColour = (Brush)(this.FindResource("ColourBackground"));
            Brush midgroundColour = (Brush)(this.FindResource("ColourMidground"));
            Brush foregroundColour = (Brush)(this.FindResource("ColourForeground"));

            Brush placeholderBackground = (Brush)(this.FindResource("ColourForeground"));

            ControlTemplate buttonTemplate = (ControlTemplate)(this.FindResource("StandardButton"));

            //Brush debug = (Brush)(this.FindResource("placeholderBackground"));

            double fontSize = 18.0f;

            //Without this, where there's nothing the click even goes to the background StackPanel
            //Rather than this dockpanel.
            mod.Background = midgroundColour;
            

            Canvas header = new Canvas();
            header.Background = midgroundColour;
            header.Height = 2;
            DockPanel.SetDock(header, Dock.Top);
            mod.Children.Add(header);

            //Non expanded form
            DockPanel mainDock = new DockPanel();
            DockPanel.SetDock(mainDock, Dock.Top);
            mainDock.Margin = new Thickness(0, 2, 0, 0);
            mainDock.HorizontalAlignment = HorizontalAlignment.Stretch;
            mainDock.Background = midgroundColour;

            CheckBox modChecked = new CheckBox();
            TextBlock modName = new TextBlock();
            Image dropDown = new Image();

            modChecked.VerticalAlignment = VerticalAlignment.Center;
            modChecked.RenderTransformOrigin = new Point(0, 0.5);
            modChecked.Margin = new Thickness(0, 0, 10, 0);

            Transform scaleTransform = new ScaleTransform(1.5, 1.5);
            modChecked.RenderTransform = scaleTransform;

            modChecked.Click += new RoutedEventHandler(this.modToggleVisibility);
            modChecked.Checked += new RoutedEventHandler(this.modEnable);
            modChecked.Unchecked += new RoutedEventHandler(this.modDisable);

            modName.Text = mod.modInfo.name;
            DockPanel.SetDock(modName, Dock.Left);
            modName.Foreground = fontColour;

            dropDown.Source = new BitmapImage(new Uri(@"pack://application:,,,/GUI/Resources/DropDown.png"));
            dropDown.Width = 24;
            dropDown.HorizontalAlignment = HorizontalAlignment.Right;
            dropDown.Margin = new Thickness(0, 0, 5, 0);
            DockPanel.SetDock(dropDown, Dock.Right);

            // Let me be real with you chief, there is no reason why this SHOULD be here
            // But without it being here, the fucking image breaks the second the window
            // is streched to the right 
            // When it breaks is influenced by the image's size. 
            // I dont know WHY it breaks There's fucking nothing on stack overflow about this shit, 
            // so I'll just Keep this here as a work around.

            // Also this was found completely on accident. Lucky me, Fixed a bug accidentally.
            // TODO: Why the fuck does this work?
            dropDown.OpacityMask = placeholderBackground;

            mainDock.Children.Add(modChecked);
            mainDock.Children.Add(modName);
            mainDock.Children.Add(dropDown);


            //These controls will only be visible whilst in expanded form.
            #region Collapsable
            Grid elementGrid = new Grid();
            elementGrid.Background = midgroundColour;

            ColumnDefinition leftColumn = new ColumnDefinition();

            ColumnDefinition rightColumn = new ColumnDefinition();
            rightColumn.Width = new GridLength(100);

            elementGrid.ColumnDefinitions.Add(leftColumn);
            elementGrid.ColumnDefinitions.Add(rightColumn);         

            StackPanel infoDock = new StackPanel();
            DockPanel.SetDock(infoDock, Dock.Left);
            infoDock.Background = midgroundColour;

            int leftPanelWidth = 115;

            DockPanel _description = new DockPanel();
            TextBlock _descriptionKey = new TextBlock();
            _descriptionKey.Text = "Description: ";
            _descriptionKey.Width = leftPanelWidth;
            TextBlock _descriptionValue = new TextBlock();
            _descriptionValue.Text = mod.modInfo.longDescription;
            _descriptionValue.TextWrapping = TextWrapping.Wrap;
            _description.Children.Add(_descriptionKey);
            _description.Children.Add(_descriptionValue);

            DockPanel _type = new DockPanel();
            TextBlock _typeKey = new TextBlock();
            _typeKey.Text = "Type : ";
            _typeKey.Width = leftPanelWidth;
            TextBlock _typeValue = new TextBlock();
            switch (mod.modInfo.type)
            {
                case ModType.AheadOfTime:
                    _typeValue.Text = "Ahead of Time";
                    break;

                case ModType.Runtime:
                    _typeValue.Text = "Runtime";
                    break;

                default:
                    _typeValue.Text = "Invalid Type";
                    break;
            }
            _typeValue.TextWrapping = TextWrapping.Wrap;
            _type.Children.Add(_typeKey);
            _type.Children.Add(_typeValue);

            DockPanel _authors = new DockPanel();
            TextBlock _authorKey = new TextBlock();
            _authorKey.Text = "Authors : ";
            _authorKey.Width = leftPanelWidth;
            TextBlock _authorValue = new TextBlock();

            string authors = "";

            for (int i = 0; i < mod.modInfo.authors.Count; i++)
            {
                if (i != 0)
                {
                    authors += ", ";
                }
                authors += mod.modInfo.authors[i];
            }

            _authorValue.Text = authors;
            _authorValue.TextWrapping = TextWrapping.Wrap;
            _authors.Children.Add(_authorKey);
            _authors.Children.Add(_authorValue);

            DockPanel _lastUpdated = new DockPanel();
            TextBlock _lastUpdatedKey = new TextBlock();
            _lastUpdatedKey.Text = "Last Updated: ";
            _lastUpdatedKey.Width = leftPanelWidth;
            TextBlock _lastUpdatedValue = new TextBlock();
            //TODO: Format time
            _lastUpdatedValue.Text = mod.modInfo.lastUpdated.ToString();
            _lastUpdatedValue.TextWrapping = TextWrapping.Wrap;
            _lastUpdated.Children.Add(_lastUpdatedKey);
            _lastUpdated.Children.Add(_lastUpdatedValue);

            DockPanel _modSize = new DockPanel();
            TextBlock _modSizeKey = new TextBlock();
            _modSizeKey.Text = "Mod Size: ";
            _modSizeKey.Width = leftPanelWidth;
            TextBlock _modSizeValue = new TextBlock();
            //TODO: Convert bytes to KB, MB, etc
            _modSizeValue.Text = mod.modInfo.fileSize.ToString() + " bytes";
            _modSizeValue.TextWrapping = TextWrapping.Wrap;
            _modSize.Children.Add(_modSizeKey);
            _modSize.Children.Add(_modSizeValue);

            DockPanel _status = new DockPanel();
            TextBlock _statusKey = new TextBlock();
            _statusKey.Text = "Description: ";
            _statusKey.Width = leftPanelWidth;
            TextBlock _statusValue = new TextBlock();
            _statusValue.Text = mod.modInfo.longDescription;
            _statusValue.TextWrapping = TextWrapping.Wrap;
            _status.Children.Add(_statusKey);
            _status.Children.Add(_statusValue);

            infoDock.Children.Add(_description);
            infoDock.Children.Add(_type);
            infoDock.Children.Add(_authors);
            infoDock.Children.Add(_lastUpdated);
            infoDock.Children.Add(_modSize);
            //infoDock.Children.Add(_status);

            #endregion
            DockPanel rightSideDock = new DockPanel();
            DockPanel buttonDock = new DockPanel();

            rightSideDock.HorizontalAlignment = HorizontalAlignment.Right;
            rightSideDock.MinWidth = 100;
            Grid.SetColumn(rightSideDock, 1);

            DockPanel.SetDock(buttonDock, Dock.Bottom);
            buttonDock.Height = 25;
            buttonDock.HorizontalAlignment = HorizontalAlignment.Right;

            Button dependencies = new Button();
            dependencies.Template = buttonTemplate;
            dependencies.Content = "Dep";
            dependencies.Click += new RoutedEventHandler(this.viewDependancies);
            dependencies.Width = 32;

            Button options = new Button();
            options.Template = buttonTemplate;
            options.Content = "Opt";
            options.Click += new RoutedEventHandler(this.viewOptions);
            options.Width = 32;

            Button delete = new Button();
            delete.Template = buttonTemplate;
            delete.Content = "Del";
            delete.Click += new RoutedEventHandler(this.deleteMod);
            delete.Width = 32;

            buttonDock.Children.Add(dependencies);
            buttonDock.Children.Add(options);
            buttonDock.Children.Add(delete);

            Image icon = new Image();

            icon.Source = new BitmapImage(new Uri(BTD6Config.modDir + "\\" + mod.modInfo.name + "\\" + mod.modInfo.icon));
            
            icon.Width = 96;
            icon.VerticalAlignment = VerticalAlignment.Bottom;
            icon.HorizontalAlignment = HorizontalAlignment.Right;
            DockPanel.SetDock(icon, Dock.Left);

            rightSideDock.Children.Add(buttonDock);
            rightSideDock.Children.Add(icon);

            elementGrid.Children.Add(infoDock);
            elementGrid.Children.Add(rightSideDock);

            elementGrid.Visibility = Visibility.Collapsed;

            mod.Children.Add(mainDock);
            mod.Children.Add(elementGrid);

            mod.MouseLeftButtonUp += new MouseButtonEventHandler(this.modMouseUp);
            mod.MouseLeftButtonDown += new MouseButtonEventHandler(this.modMouseDown);
            mod.MouseMove += new MouseEventHandler(this.modMouseMove);

            mod.DragOver += new DragEventHandler(this.ddPanelVisibility);
            mod.Drop += new DragEventHandler(this.ddPanelDrop);

            mod.AllowDrop = true;
            


            destination.Children.Add(mod);
        }



        public void modEnable(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Mod Enabled...");
            // TODO: Implement

        }

        public void modDisable(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Mod Disabled...");
            // TODO: Implement
        }


        public void modMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ModPanel mod = ((ModPanel)(sender));
                mod.isDragged = true;

                Vector diff = mod.originDrag - e.GetPosition(null);

                if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance
                   || Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
                {

                    DataObject dragData = new DataObject();
                    dragData.SetData(mod.modInfo.type.ToString(), mod);

                    DragDrop.DoDragDrop(this, dragData, DragDropEffects.Move);


                }

            }
        }


        public void modMouseUp(object sender, MouseEventArgs e)
        {
            ModPanel mod = (ModPanel)(sender);
            if (mod.isDragged == true)
            {
                Vector diff = mod.originDrag - e.GetPosition(null);
                if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance
                   || Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    //
                    // A drag drop has happened. Don't expand.
                    //
                }
                else
                {
                    toggleCollapsed(sender);
                }


                //TODO: Drag/Drop
                //https://social.msdn.microsoft.com/Forums/vstudio/en-US/d32bb0af-b14f-4e88-ad36-098d11cd375c/dragdrop-elements-within-a-stack-panel?forum=wpf

            }
            else
            {
                toggleCollapsed(sender);
            }
        }

        public void modMouseDown(object sender, MouseEventArgs e)
        {
            //TODO: Implement drag/drop
            ModPanel mod = (ModPanel)(sender);
            mod.originMove = mod.PointToScreen(new Point(0, 0));
            mod.originDrag = e.GetPosition(null);
        }


        private void ddVisibiltyInjected(object sender, DragEventArgs e)
        {

            if (!e.Data.GetDataPresent(ModType.Runtime.ToString()))
            {
                e.Effects = DragDropEffects.None;

            }
            e.Handled = true;
        }

        private void ddVisibiltyPatched(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(ModType.AheadOfTime.ToString()))
            {
                e.Effects = DragDropEffects.None;

            }
            e.Handled = true;
        }

        private void ddVisibiltyDisabled(object sender, DragEventArgs e)
        {
            if (!(e.Data.GetDataPresent(ModType.Runtime.ToString()) ||
                e.Data.GetDataPresent(ModType.AheadOfTime.ToString())))
            {
                e.Effects = DragDropEffects.None;

            }
            e.Handled = true;
        }

        private void ddPanelVisibility(object sender, DragEventArgs e)
        {
            ModPanel mod = (ModPanel)(sender);
            StackPanel modList = (StackPanel)(mod.Parent);
            modList.RaiseEvent(e);
            e.Handled = true;
        }


        private void ddPanelDrop(object sender, DragEventArgs e)
        {
            ModPanel mod = (ModPanel)(sender);
            StackPanel modList = (StackPanel)(mod.Parent);
            modList.RaiseEvent(e);
            e.Handled = true;
        }

        private void ddMovePanel(ModPanel mod, StackPanel modlist, Point mousepos)
        {
            for (int i = 0; i < modlist.Children.Count; i++)
            {
                double height = ((FrameworkElement)(modlist.Children[i])).ActualHeight;

                if (mousepos.Y < modlist.Children[i].TranslatePoint(new Point(0, 0), this).Y)
                {
                    // Move Up
                    modlist.Children.Remove(mod);
                    if (i > 0)
                    {
                        modlist.Children.Insert(i - 1, mod);
                    }
                    else
                    {
                        modlist.Children.Insert(0, mod);
                    }
                    return;
                }
                else if (mousepos.Y < modlist.Children[i].TranslatePoint(new Point(0, 0), this).Y)
                {
                    // Move Down
                    modlist.Children.Remove(mod);
                    modlist.Children.Insert(i + 1, mod);
                    return;
                }
            }
        }



        private void ddSpDropInjected(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(ModType.Runtime.ToString()))
            {
                ModPanel mod = (ModPanel)e.Data.GetData(ModType.Runtime.ToString());
                StackPanel modlist = (StackPanel)mod.Parent;
                CheckBox togglemod = (CheckBox)((DockPanel)(mod.Children[1])).Children[0];
                togglemod.IsChecked = true;

                if (ModsInjectedSP.Children.Contains(mod))
                {
                    ddMovePanel(mod, modlist, e.GetPosition(this));
                }
                else
                {
                    modlist.Children.Remove(mod);
                    ModsInjectedSP.Children.Add(mod);
                    ddMovePanel(mod, ModsInjectedSP, e.GetPosition(this));
                }
            }
        }

        private void ddSpDropPatched(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(ModType.AheadOfTime.ToString()))
            {
                ModPanel mod = (ModPanel)e.Data.GetData(ModType.Runtime.ToString());
                StackPanel modlist = (StackPanel)mod.Parent;
                CheckBox togglemod = (CheckBox)((DockPanel)(mod.Children[1])).Children[0];
                togglemod.IsChecked = true;

                if (ModsPatchedSP.Children.Contains(mod))
                {
                    ddMovePanel(mod, modlist, e.GetPosition(this));
                }
                else
                {
                    modlist.Children.Remove(mod);
                    ModsPatchedSP.Children.Add(mod);
                    ddMovePanel(mod, ModsPatchedSP, e.GetPosition(this));
                }
            }
        }

        private void ddSpDropDisabled(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(ModType.Runtime.ToString()) || e.Data.GetDataPresent(ModType.AheadOfTime.ToString()))
            {
                ModPanel mod = null;
                if (e.Data.GetDataPresent(ModType.Runtime.ToString()))
                {
                    mod = (ModPanel)e.Data.GetData(ModType.Runtime.ToString());
                }
                else if (e.Data.GetDataPresent(ModType.AheadOfTime.ToString()))
                {
                    mod = (ModPanel)e.Data.GetData(ModType.AheadOfTime.ToString());
                }

                if(mod == null)
                {
                    return;
                }
                
                StackPanel modlist = (StackPanel)mod.Parent;
                CheckBox togglemod = (CheckBox)((DockPanel)(mod.Children[1])).Children[0];
                togglemod.IsChecked = false;

                if (ModsDisabledSP.Children.Contains(mod))
                {
                    ddMovePanel(mod, modlist, e.GetPosition(this));
                }
                else
                {
                    modlist.Children.Remove(mod);
                    ModsDisabledSP.Children.Add(mod);
                    ddMovePanel(mod, ModsDisabledSP, e.GetPosition(this));
                }
            }
        }

        private void ddDpDropInjected(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(ModType.Runtime.ToString()))
            {
                ModPanel mod = (ModPanel)e.Data.GetData(ModType.Runtime.ToString());
                CheckBox togglemod = (CheckBox)((DockPanel)(mod.Children[1])).Children[0];
                togglemod.IsChecked = true;
                StackPanel modlist = (StackPanel)mod.Parent;
                modlist.Children.Remove(mod);
                ModsInjectedSP.Children.Insert(0, mod);
            }
        }

        private void ddDpDropPatched(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(ModType.AheadOfTime.ToString()))
            {
                ModPanel mod = (ModPanel)e.Data.GetData(ModType.AheadOfTime.ToString());
                CheckBox togglemod = (CheckBox)((DockPanel)(mod.Children[1])).Children[0];
                togglemod.IsChecked = true;
                StackPanel modlist = (StackPanel)mod.Parent;
                modlist.Children.Remove(mod);
                ModsPatchedSP.Children.Insert(0, mod);
            }
        }
        private void ddDpDropDisabled(object sender, DragEventArgs e)
        {

            if (e.Data.GetDataPresent(ModType.Runtime.ToString()))
            {
                ModPanel mod = (ModPanel)e.Data.GetData(ModType.Runtime.ToString());
                StackPanel modlist = (StackPanel)mod.Parent;
                modlist.Children.Remove(mod);
                CheckBox togglemod = (CheckBox)((DockPanel)(mod.Children[1])).Children[0];
                togglemod.IsChecked = false;
                ModsDisabledSP.Children.Insert(0, mod);
            }
            else
            {
                ModPanel mod = (ModPanel)e.Data.GetData(ModType.AheadOfTime.ToString());
                StackPanel modlist = (StackPanel)mod.Parent;
                modlist.Children.Remove(mod);
                CheckBox togglemod = (CheckBox)((DockPanel)(mod.Children[1])).Children[0];
                togglemod.IsChecked = false;
                ModsDisabledSP.Children.Insert(0, mod);
            }

        }






        public void toggleCollapsed(object sender)
        {
            ModPanel mod = (ModPanel)(sender);
            Grid detailGrid = (Grid)(mod.Children[2]);
            DockPanel mainDock = (DockPanel)(mod.Children[1]);
            Image dropDownImage = (Image)(mainDock.Children[2]);


            if (detailGrid.Visibility == Visibility.Visible)
            {
                detailGrid.Visibility = Visibility.Collapsed;
                Transform scaleTransform = new ScaleTransform(1.0, 1.0);

                dropDownImage.RenderTransform = scaleTransform;
            }
            else
            {
                detailGrid.Visibility = Visibility.Visible;
                Transform scaleTransform = new ScaleTransform(1.0, -1.0);
                dropDownImage.RenderTransformOrigin = new Point(0, 0.5);

                dropDownImage.RenderTransform = scaleTransform;
            }
        }

        public void modToggleVisibility(object sender, RoutedEventArgs e)
        {
            CheckBox chkbox = (CheckBox)sender;
            DockPanel mdp = (DockPanel)chkbox.Parent;
            ModPanel mp = (ModPanel)mdp.Parent;
            StackPanel sp = (StackPanel)mp.Parent;

            if (chkbox.IsChecked == true)
            {
                //Enabling a mod

                //Remove from disabled stackpanel
                sp.Children.Remove(mp);

                if (mp.modInfo.type == ModType.Runtime)
                {
                    ModsInjectedSP.Children.Add(mp);
                    ModsInjectedDP.Visibility = Visibility.Visible;
                }
                else
                {
                    ModsPatchedSP.Children.Add(mp);
                }
            }
            else
            {
                //Disabling a mod


                //Remove from enabled stackpanel
                sp.Children.Remove(mp);

                ModsDisabledSP.Children.Add(mp);
            }




        }

        public void viewDependancies(object sender, RoutedEventArgs e)
        {
            //TODO: Implement dependency window.
            MessageBox.Show("TODO: Implement dependency window...");
        }

        public void viewOptions(object sender, RoutedEventArgs e)
        {
            //TODO: Implement mod options
            MessageBox.Show("TODO: Implement mod options...");
        }

        public void deleteMod(object sender, RoutedEventArgs e)
        {
            //TODO: Implement deleting of mod
            MessageBox.Show("TODO: Implement mod deleting...");
        }

        public void FlushCache(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("TODO: Figure out if cache is even worth it");
        }

        public void DeleteMods(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("TODO: Move mods to a backup folder");
        }

        public void ImportMod(object sender, RoutedEventArgs e)
        {

        }

        public void OpenModsFolder(object sender, RoutedEventArgs e)
        {

        }

        private void DocWebBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            string ApplicationDir = Directory.GetCurrentDirectory();
            DocWebBrowser.Navigate(new Uri("file:///" + ApplicationDir + "/doc/doc.html"));
        }

        private void DevWriteMod_Click(object sender, RoutedEventArgs e)
        {
            //
            // TODO: This needs a lot of quality of life improvments
            //

            ModInfo mi = new ModInfo();

            
            if(DevModListBox.Text == null)
            {
                throw new Exception("Error, Project not selected");
            }

            if (DevModListBox.Text == "{ Create New Project }")
            {
                mi.name = DevModNameTextbox.Text;
            }
            else
            {
                mi.name = DevModListBox.Text;
            }

            if (DevModType.SelectedItem == DevModTypeRuntime)
            {
                mi.type = ModType.Runtime;
            }
            else if (DevModType.SelectedItem == DevModTypeAOT)
            {
                mi.type = ModType.AheadOfTime;
            }
            else
            {
                throw new Exception("Error, No mod type selected");
            }

            if(DevModShortDescription == null)
            {
                throw new Exception("Error, No mod short description was provided");
            }
            else
            {
                mi.shortDescription = DevModShortDescription.Text;
            }

            if(DevModLongDescription == null)
            {
                throw new Exception("Error, no mod long descirption was provided");
            }
            else
            {
                mi.longDescription = DevModLongDescription.Text;
            }

            if(DevModAuthors.Text == null)
            {
                throw new Exception("Error, no mod authors were provided");
            }
            else
            {
                string[] Authors = DevModAuthors.Text.Split('\n');
                mi.authors = Authors.ToList();
            }

            if(DevModContact.Text == null)
            {
                throw new Exception("Error, no mod contacts were provided");
            }
            else
            {
                string[] Contact = DevModContact.Text.Split('\n');
                mi.contact = Contact.ToList();
            }

            if(DevModIconPath == null)
            {
                throw new Exception("Error, no Icon path provided");
            }
            else
            {
                mi.icon = DevModIconPath.Text;
            }

            // TODO: Dependencies and extensions...

            // TODO: File size, last updated...

            string OutputJson = JsonConvert.SerializeObject(mi);

            if(!(Directory.Exists(BTD6Config.modDir + "\\" + mi.name)))
            {
                Directory.CreateDirectory(BTD6Config.modDir + "\\" + mi.name);
            }

            File.WriteAllText(BTD6Config.modDir + "\\" + mi.name + "\\" + mi.name + ".btd6modinfo", OutputJson);

        }
    }
}
