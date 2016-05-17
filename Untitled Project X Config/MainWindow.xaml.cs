using Microsoft.Win32;
using System;
using System.IO;
using System.Security;
using System.Security.Principal;
using System.Windows;

namespace Untitled_Project_X_Config
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public UnXIni config = null;

        public MainWindow()
        {
            InitializeComponent();

            PopulateProperties();   // Will check the Applicaiton Properties and Populate them if Necessary
            CheckUnX();             // Makes sure we can write to tsfix.ini. Some systems may need to run as admin.

            if (File.Exists(Properties.Settings.Default.FFXX2Path + Path.AltDirectorySeparatorChar + "UnX.dll"))
            {
                var dllVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(Properties.Settings.Default.FFXX2Path + Path.AltDirectorySeparatorChar + "UnX.dll").ProductVersion;
                if (dllVersion == null)
                {
                    var result = MessageBox.Show(String.Format("Could not read Untitled Project X Version" + Environment.NewLine +
                            "It is possible that the Untitle Project X DLL is Invalid or Corrupt." + Environment.NewLine + Environment.NewLine +
                            "Do you wish to continue anyways?"),
                            "Untitled Project X Config", MessageBoxButton.YesNo,
                            MessageBoxImage.Warning, MessageBoxResult.No);
                    if (result == MessageBoxResult.No)
                        if (Application.Current != null) Application.Current.Shutdown();//*/
                }
                else
                {
                    if (Properties.Settings.Default.UnXVersion != dllVersion.Substring(0, Properties.Settings.Default.UnXVersion.Length))
                    {
                        var result = MessageBox.Show(string.Format("Unexpected Untitle Project X Version" + Environment.NewLine +
                            "Expected Version: {0}" + Environment.NewLine +
                            "Current Version: {1}" + Environment.NewLine + Environment.NewLine +
                            "Please make sure that Untitled Project X and this appliaction are up-to-date." + Environment.NewLine + Environment.NewLine +
                            "Do you wish to continue anyways? Be aware not all settings may work.", Properties.Settings.Default.UnXVersion, dllVersion.Substring(0, Properties.Settings.Default.UnXVersion.Length)),
                            "Untitled Project X Config", MessageBoxButton.YesNo,
                            MessageBoxImage.Warning, MessageBoxResult.No);
                        if (result == MessageBoxResult.No)
                            if (Application.Current != null) Application.Current.Shutdown();
                    }
                }
            }
            else
            {
                if (File.Exists(Properties.Settings.Default.FFXX2Path + Path.AltDirectorySeparatorChar + "UnX.ini"))
                {
                    MessageBox.Show("Could not find Untitled Project X DLL or INI. Exiting.", "Untitled Project X Config", MessageBoxButton.OK, MessageBoxImage.Error);
                    if (Application.Current != null) Application.Current.Shutdown();
                }
            }

            if (File.Exists(Properties.Settings.Default.FFXX2Path + Path.AltDirectorySeparatorChar + "UnX.ini"))
            {
                config = new UnXIni(Properties.Settings.Default.FFXX2Path);
                if (config.Version != Properties.Settings.Default.UnXVersion)
                {
                    var result = MessageBox.Show(string.Format("Unexpected Untitled Project X INI Version" + Environment.NewLine +
                        "Expected Version: {0}" + Environment.NewLine +
                        "Current Version: {1}" + Environment.NewLine + Environment.NewLine +
                        "If the DLL is up to date it should be safe to continute. However, it is recommended to launch Final Fantasy X/X-2 HD Remaster once before continuing." + Environment.NewLine + Environment.NewLine +
                        "Do you wish to continue anyways?", Properties.Settings.Default.UnXVersion, config.Version),
                        "Untitled Project X Config", MessageBoxButton.YesNo,
                        MessageBoxImage.Warning, MessageBoxResult.Yes);
                    if (result == MessageBoxResult.No)
                        if (Application.Current != null) Application.Current.Shutdown();
                }
            }
            else
                if (Application.Current != null) Application.Current.Shutdown();

            DataContext = config;
        }

        private void PopulateProperties()
        {

            if (string.IsNullOrEmpty(Properties.Settings.Default.FFXX2Path))
            {
                string steamPath = string.Empty;
                try
                {
                    steamPath = GetSteamPath();
                }
                catch (SecurityException)
                {
                    // Don't care about this. They will just need to Browse further down.
                }

                string FFXX2Path = string.Empty;
                if (Directory.Exists(steamPath))
                    FFXX2Path = steamPath + Path.AltDirectorySeparatorChar + "steamapps" + Path.AltDirectorySeparatorChar + "common" + Path.AltDirectorySeparatorChar + "FINAL FANTASY FFX&FFX-2 HD Remaster";

                while (!Directory.Exists(FFXX2Path))
                {
                    var result = MessageBox.Show("Invalid Final Fantasy X/X-2 HD Remaster Path. Cancel to close application.", "Untitled Project X Config", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.OK);
                    if (result == MessageBoxResult.Cancel)
                    {
                        if (Application.Current != null) Application.Current.Shutdown();
                        return;
                    }
                    FFXX2Path = GetFFXX2Path(steamPath);
                }
                Properties.Settings.Default.FFXX2Path = FFXX2Path;
                Properties.Settings.Default.Save();
            }

            var GamePath = Properties.Settings.Default.FFXX2Path;
            while (!File.Exists(GamePath + Path.AltDirectorySeparatorChar + "UnX.ini"))
            {
                var result = MessageBox.Show("Could not find \"UnX.ini\". Press Cancel to close application.", "Untitled Project X Config", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.OK);
                if (result == MessageBoxResult.Cancel)
                {
                    if (Application.Current != null) Application.Current.Shutdown();
                    return;
                }
                else
                    GamePath = GetFFXX2Path();
            }

            if (Properties.Settings.Default.FFXX2Path != GamePath)
            {
                Properties.Settings.Default.FFXX2Path = GamePath;
                Properties.Settings.Default.Save();
            }
        }

        private string GetSteamPath()
        {
            var value = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Valve\Steam", "SteamPath", string.Empty);
            if (value == null)
                return string.Empty;
            else
                return value.ToString();
        }

        private string GetFFXX2Path(string steamPath = null)
        {
            if (steamPath == null)
            {
                steamPath = string.Empty;
                try
                {
                    steamPath = GetSteamPath();
                }
                catch (SecurityException)
                {
                    // Don't care about this. They will just need to Browse.
                }
            }
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.RootFolder = Environment.SpecialFolder.MyComputer;
            dialog.SelectedPath = steamPath;
            dialog.Description = "Browse for Final Fantasy X/X-2 HD Remaster Game Folder";
            var result = dialog.ShowDialog();
            string ToSPath = string.Empty;
            if (result != System.Windows.Forms.DialogResult.Cancel)
                ToSPath = dialog.SelectedPath;
            return ToSPath;
        }

        public static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private void CheckUnX()
        {
            var GamePath = Properties.Settings.Default.FFXX2Path;
            if (!File.Exists(GamePath + Path.AltDirectorySeparatorChar + "UnX.ini")) return;

            FileStream fs = null;
            try
            {
                fs = File.OpenWrite(GamePath + Path.AltDirectorySeparatorChar + "UnX.ini");
            }
            catch (UnauthorizedAccessException)
            {
                if (fs != null)
                {
                    fs.Close();
                    fs = null;
                }

                if (IsAdministrator())
                    MessageBox.Show("Cannot write to \"UnX.ini\". File is most likely read-only.", "Untitled Project X Config", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                    MessageBox.Show("Cannot write to \"UnX.ini\". File may be read-only or Administrator Priveleges may be needed.", "Untitled Project X Config", MessageBoxButton.OK, MessageBoxImage.Error);

                if (Application.Current != null) Application.Current.Shutdown();
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs = null;
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(config.GamePath + Path.AltDirectorySeparatorChar + config.ResourceRoot))
            {
                var dlgResult = MessageBox.Show(string.Format("Could not find the folder \"{0}\" in the Game Path." + Environment.NewLine + Environment.NewLine +
                    "Do you wish to save this value anyways? A bad value may lead to unpredictable behavior.", config.ResourceRoot), "Untitled Project X Config", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
                if (dlgResult == MessageBoxResult.No) return;

            }

            if (!Directory.Exists(config.GamePath + Path.AltDirectorySeparatorChar + config.ResourceRoot + Path.AltDirectorySeparatorChar + "gamepads" + Path.AltDirectorySeparatorChar + config.Gamepad))
            {
                var dlgResult = MessageBox.Show(string.Format("Could not find the folder \"{0}\" in \"{1}\\gamepads\"." + Environment.NewLine + Environment.NewLine +
                    "Do you wish to save this value anyways? A bad value may lead to unpredictable behavior.", config.Gamepad, config.ResourceRoot), "Untitled Project X Config", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
                if (dlgResult == MessageBoxResult.No) return;
            }

            var result = MessageBox.Show("Do you want to make a backup of UnX.ini? This is recommended, especially if you are not on the supported version of Untitled Project X.", "Untitled Project X Config", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning, MessageBoxResult.Yes);
            if (result == MessageBoxResult.Yes)
            {
                var dialog = new System.Windows.Forms.FolderBrowserDialog();
                dialog.Description = "Browse for Backup Location";
                dialog.RootFolder = Environment.SpecialFolder.MyComputer;
                dialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var rst = dialog.ShowDialog(this.GetIWin32Window());
                if (rst == System.Windows.Forms.DialogResult.Cancel) return;

                var BackupFile = dialog.SelectedPath + Path.DirectorySeparatorChar + string.Format("UnX {0:MMddyyyyHHmmss}.ini", DateTime.Now);
                File.Copy(Properties.Settings.Default.FFXX2Path + Path.AltDirectorySeparatorChar + "UnX.ini", BackupFile);
                MessageBox.Show(string.Format("Backup of UnX.ini located at:" + Environment.NewLine + "{0}", BackupFile), "Untitled Project X Config", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (result == MessageBoxResult.Cancel) return;

            config.SaveValues();
            config.ReloadValues();

        }

        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("All changes will be lost. Do you wish to contiue?", "Untitled Project X Config", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
            if (result == MessageBoxResult.No) return;

            config.ReloadValues();
        }
    }
}