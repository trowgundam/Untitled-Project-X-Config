using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Untitled_Project_X_Config
{
    public enum UnXLanguageCodes
    {
        [Description("English"), Abbreviation("us")]
        US = 0,
        [Description("Japanese"), Abbreviation("jp")]
        JP = 1
    }
    public class UnXIni : INotifyPropertyChanged
    {
        public string GamePath { get; private set; }
        private string IniPath
        {
            get
            {
                return GamePath + Path.AltDirectorySeparatorChar + "UnX.ini";
            }
        }
        private Ini.IniFile ConfigFile = null;

        #region Private Variables
        // UnX.Display
        private bool _DisableDPIScaling = true;

        // UnX.Render
        private bool _FlipMode = true;

        // UnX.Texture
        private string _ResourceRoot = "UnX_Res";
        private bool _Dump = false;
        private bool _Inject = false;
        private string _GamepadIcons = "PlayStation-Glossy.dds";

        // UnX.Input
        private bool _ManageCursor = true;
        private float _CursorTimeout = 0.5f;
        private int _GamepadSlot = -1;
        private bool _KeysActivateCursor = false;

        // UnX.Language
        private UnXLanguageCodes _Voice = UnXLanguageCodes.JP;
        private UnXLanguageCodes _SoundEffects = UnXLanguageCodes.JP;
        private UnXLanguageCodes _Video = UnXLanguageCodes.JP;

        // UnX.System
        private string _Version = string.Empty;
        private string _Injector = string.Empty;
        #endregion

        #region Public Properties
        // UnX.Display
        public bool DisableDPIScaling
        {
            get { return _DisableDPIScaling; }
            set
            {
                if (value == _DisableDPIScaling) return;
                _DisableDPIScaling = value;
                OnPropertyChanged();
            }
        }

        // UnX.Render
        public bool FlipMode
        {
            get { return _FlipMode; }
            set
            {
                if (value == _FlipMode) return;
                _FlipMode = value;
                OnPropertyChanged();
            }
        }

        // UnX.Textures
        public string ResourceRoot
        {
            get { return _ResourceRoot; }
            set
            {
                if (value == _ResourceRoot) return;
                _ResourceRoot = value;
                OnPropertyChanged();
                OnPropertyChanged("ResourceRootColor");
                OnPropertyChanged("GamepadIconsColor");
            }
        }

        public bool Dump
        {
            get { return _Dump; }
            set
            {
                if (value == _Dump) return;
                _Dump = value;
                OnPropertyChanged();
            }
        }

        public bool Inject
        {
            get { return _Inject; }
            set
            {
                if (value == _Inject) return;
                _Inject = value;
                OnPropertyChanged();
            }
        }

        public string GamepadIcons
        {
            get { return _GamepadIcons; }
            set
            {
                if (value == _GamepadIcons) return;
                _GamepadIcons = value;
                OnPropertyChanged();
                OnPropertyChanged("GamepadIconsColor");
            }
        }

        // UnX.Input
        public bool ManageCursor
        {
            get { return _ManageCursor; }
            set
            {
                if (_ManageCursor == value) return;
                _ManageCursor = value;
                OnPropertyChanged();
            }
        }

        public float CursorTimeout
        {
            get { return _CursorTimeout; }
            set
            {
                if (_CursorTimeout == value) return;
                _CursorTimeout = value;
                OnPropertyChanged();
            }
        }

        public int GamepadSlot
        {
            get { return _GamepadSlot; }
            set
            {
                if (_GamepadSlot == value) return;
                _GamepadSlot = value;
                OnPropertyChanged();
            }
        }

        public bool KeysActivateCursor
        {
            get { return _KeysActivateCursor; }
            set
            {
                if (_KeysActivateCursor == value) return;
                _KeysActivateCursor = value;
                OnPropertyChanged();
            }
        }

        // UnX.Language
        public UnXLanguageCodes Voice
        {
            get { return _Voice; }
            set
            {
                if (_Voice == value) return;
                _Voice = value;
                OnPropertyChanged();
            }
        }

        public UnXLanguageCodes SoundEffects
        {
            get { return _SoundEffects; }
            set
            {
                if (_SoundEffects == value) return;
                _SoundEffects = value;
                OnPropertyChanged();
            }
        }

        public UnXLanguageCodes Video
        {
            get { return _Video; }
            set
            {
                if (_Video == value) return;
                _Video = value;
                OnPropertyChanged();
            }
        }

        // UnX.System
        public string Version {
            get { return _Version; }
            private set
            {
                if (value == _Version) return;
                _Version = value;
                OnPropertyChanged("Version");
                OnPropertyChanged("UnXVersionColor");
            }
        }
        public string Injector {
            get { return _Injector; }
            private set
            {
                if (value == _Injector) return;
                _Injector = value;
                OnPropertyChanged("Injector");
            }
        }
        #endregion

        #region Constructors
        public UnXIni()
        {
            GamePath = string.Empty;
        }

        public UnXIni(string path)
        {
            SetPath(path);
            LoadValues();
        }
        #endregion

        public void ReloadValues()
        {
            LoadValues();
        }

        public void LoadIni(string GamePath)
        {
            SetPath(GamePath);
            LoadValues();
        }

        private void SetPath(string path)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException("Cannot find Final Fantasy X/X-2 HD Remaster Directory");
            GamePath = path;

            if (!File.Exists(IniPath))
                throw new FileNotFoundException("Cannot find UnX.ini");

            FileStream fs = null;
            try
            {
                fs = File.OpenWrite(IniPath);
            }
            catch (UnauthorizedAccessException uae)
            {
                throw new UnauthorizedAccessException("Cannot open UnX.ini for writing.", uae);
            }
            catch (Exception ex)
            {
                throw new Exception("Error opening UnX.ini.", ex);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs = null;
                }
            }

            GamePath = path;
        }

        private void LoadValues()
        {
            if (ConfigFile == null)
                ConfigFile = new Ini.IniFile(IniPath);
            
            // UnX.Display
            DisableDPIScaling = GetIniBooleanValue("DisableDPIScaling", "UnX.Display", true);

            // UnX.Render
            FlipMode = GetIniBooleanValue("FlipMode", "UnX.Render", true);

            // UnX.Textures
            ResourceRoot = ConfigFile.Read("ResourceRoot", "UnX.Textures");
            Dump = GetIniBooleanValue("Dump", "UnX.Textures", false);
            Inject = GetIniBooleanValue("Inject", "UnX.Textures", false);
            GamepadIcons = ConfigFile.Read("GamepadIcons", "UnX.Textures");

            // UnX.Input
            ManageCursor = GetIniBooleanValue("ManageCursor", "UnX.Input", true);
            CursorTimeout = GetIniFloatValue("CursorTimeout", "UnX.Input", 0.5f);
            GamepadSlot = GetIniIntegerValue("GamepadSlot", "UnX.Input", -1);
            KeysActivateCursor = GetIniBooleanValue("KeysActivateCursor", "UnX.Input", false);

            // UnX.Language
            Voice = GetIniEnumValue<UnXLanguageCodes>("Voice", "UnX.Language", UnXLanguageCodes.JP);
            SoundEffects = GetIniEnumValue<UnXLanguageCodes>("SoundEffects", "UnX.Language", UnXLanguageCodes.JP);
            Video = GetIniEnumValue<UnXLanguageCodes>("Video", "UnX.Language", UnXLanguageCodes.JP);

            // UnX.System
            Version = ConfigFile.Read("Version", "UnX.System");
            Injector = ConfigFile.Read("Injector", "UnX.System");
        }

        #region "Get INI Value Methods"
        private bool GetIniBooleanValue(string Key, string Section, bool defValue)
        {
            bool result;
            if (!bool.TryParse(ConfigFile.Read(Key, Section), out result))
                return defValue;
            else
                return result;
        }

        private float GetIniFloatValue(string Key, string Section, float defValue)
        {
            float result;
            if (!float.TryParse(ConfigFile.Read(Key, Section), out result))
                return defValue;
            else
                return result;
        }

        private int GetIniIntegerValue(string Key, string Section, int defValue)
        {
            int result;
            if (!int.TryParse(ConfigFile.Read(Key, Section), out result))
                return defValue;
            else
                return result;
        }

        private T GetIniEnumValue<T>(string Key, string Section, T defValue) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum) throw new ArgumentException("T must be an enumerated type");

            string langCode;
            langCode = ConfigFile.Read(Key, Section);

            
            foreach (T val in Enum.GetValues(typeof(T)))
            {
                if (AbbreviationAttribute.Get(val) == langCode)
                    return val;
            }

            return defValue;
        }
        #endregion

        #region "Public Color Properties"
        public System.Windows.Media.Brush UnXVersionColor
        {
            get
            {
                if (_Version != Properties.Settings.Default.UnXVersion)
                    return System.Windows.Media.Brushes.Red;
                else
                    return System.Windows.SystemColors.WindowTextBrush;
            }
        }

        public System.Windows.Media.Brush ResourceRootColor
        {
            get
            {
                if (Directory.Exists(GamePath + Path.AltDirectorySeparatorChar + _ResourceRoot))
                    return System.Windows.Media.Brushes.Green;
                else
                    return System.Windows.Media.Brushes.Red;
            }
        }

        public System.Windows.Media.Brush GamepadIconsColor
        {
            get
            {
                if (File.Exists(GamePath + Path.AltDirectorySeparatorChar + _ResourceRoot + Path.AltDirectorySeparatorChar + _GamepadIcons))
                    return System.Windows.Media.Brushes.Green;
                else
                    return System.Windows.Media.Brushes.Red;
            }
        }
        #endregion

        #region "INotifyPropertyChanged Items"
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("[UnX.Display]");
            sb.AppendLine(string.Format("DisableDPIScaling={0}", _DisableDPIScaling.ToString()));

            sb.AppendLine("[UnX.Render]");
            sb.AppendLine(string.Format("FlipMode={0}", _FlipMode.ToString()));

            sb.AppendLine("[UnX.Textures]");
            sb.AppendLine(string.Format("ResourceRoot={0}", _ResourceRoot.ToString()));
            sb.Append(string.Format("Dump={0}", _Dump.ToString()));
            sb.Append(string.Format("Inject={0}", _Inject.ToString()));
            sb.AppendLine(string.Format("GamepadIcons={0}", _GamepadIcons.ToString()));

            sb.AppendLine("[UnX.Input]");
            sb.AppendLine(string.Format("ManageCursor={0}", _ManageCursor.ToString()));
            sb.AppendLine(string.Format("CursorTimeout={0}", _CursorTimeout.ToString("F1")));
            sb.AppendLine(string.Format("GamepadSlot={0}", _GamepadSlot.ToString()));
            sb.AppendLine(string.Format("KeysActivateCursor={0}", _KeysActivateCursor.ToString()));

            sb.AppendLine("[UnX.Language]");
            sb.AppendLine(string.Format("Voice={0}", _Voice.ToString()));
            sb.AppendLine(string.Format("SoundEffects={0}", _SoundEffects.ToString()));
            sb.AppendLine(string.Format("Video={0}", _Video.ToString()));

            sb.AppendLine("[UnX.System]");
            sb.AppendLine(string.Format("Version={0}", _Version.ToString()));
            sb.Append(string.Format("Injector={0}", _Injector.ToString()));

            return sb.ToString();
        }

        public void SaveValues()
        {
            if (ConfigFile == null)
                ConfigFile = new Ini.IniFile(IniPath);

            // UnX.Display
            ConfigFile.Write("DisableDPIScaling", _DisableDPIScaling.ToString().ToLower(), "UnX.Display");

            // UnX.Render
            ConfigFile.Write("FlipMode", _FlipMode.ToString().ToLower(), "UnX.Render");

            // UnX.Textures
            ConfigFile.Write("ResourceRoot", _ResourceRoot.ToString(), "UnX.Textures");
            ConfigFile.Write("Dump", _Dump.ToString().ToLower(), "UnX.Textures");
            ConfigFile.Write("Inject", _Inject.ToString().ToLower(), "UnX.Textures");
            ConfigFile.Write("GamepadIcons", _GamepadIcons.ToString(), "UnX.Textures");

            // UnX.Input
            ConfigFile.Write("ManageCursor", _ManageCursor.ToString().ToLower(), "UnX.Input");
            ConfigFile.Write("CursorTimeout", _CursorTimeout.ToString("F1"), "UnX.Input");
            ConfigFile.Write("GamepadSlot", _GamepadSlot.ToString(), "UnX.Input");
            ConfigFile.Write("KeysActivateCursor", _KeysActivateCursor.ToString().ToLower(), "UnX.Input");

            // UnX.Language
            ConfigFile.Write("Voice", AbbreviationAttribute.Get(_Voice), "UnX.Language");
            ConfigFile.Write("SoundEffects", AbbreviationAttribute.Get(_SoundEffects), "UnX.Language");
            ConfigFile.Write("Video", AbbreviationAttribute.Get(_Video), "UnX.Language");
        }
    }
}
