using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Reflection;

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
        #region "INI File"
        public string GamePath { get; private set; }
        private string IniPath
        {
            get
            {
                return GamePath + Path.AltDirectorySeparatorChar + "UnX.ini";
            }
        }
        private Ini.IniFile ConfigFile = null;
        #endregion

        #region Private Variables
        // UnX.Display
        private bool _DisableDPIScaling = true;
        private bool _EnableFullscreen = false;

        // UnX.Render
        private bool _FlipMode = false;

        // UnX.Texture
        private string _ResourceRoot = "UnX_Res";
        private bool _Dump = false;
        private bool _Inject = false;
        private string _Gamepad = "PlayStation_Glossy";

        // UnX.Input
        private bool _ManageCursor = true;
        private float _CursorTimeout = 0.0f;
        private int _GamepadSlot = -1;
        private bool _KeysActivateCursor = false;
        private bool _FourFingerSalute = true;
        private bool _SpecialKeys = true;

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
        [Description("Fixes DPI scaling problems in Windows 8 and 10. Recommended: Checked")]
        [IniInfo("DisableDPIScaling", "UnX.Display")]
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

        [Description("Enable Exclusive Full Screen (EXPERIMENTAL) Recommended: Unchecked")]
        [IniInfo("EnableFullscreen", "UnX.Display")]
        public bool EnableFullscreen
        {
            get { return _EnableFullscreen; }
            set
            {
                if (value == _EnableFullscreen) return;
                _EnableFullscreen = value;
                OnPropertyChanged();
            }
        }

        // UnX.Render
        [Description("Enable Flip Presentation Model. This increase performance in Window mode and increased compatiblity with some Screen Capture software. Recommended: Unchecked due to unresolved issues with the game engine.")]
        [IniInfo("FlipMode", "UnX.Render")]
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
        [Description("The directory where Untitled Project X will look for textures or put textures in for Injecting and Dumping. Directory must be in the game directory. Recommended: UnX_Res")]
        [IniInfo("ResourceRoot", "UnX.Textures")]
        public string ResourceRoot
        {
            get { return _ResourceRoot; }
            set
            {
                if (value == _ResourceRoot) return;
                _ResourceRoot = value;
                OnPropertyChanged();
                OnPropertyChanged("ResourceRootColor");
                OnPropertyChanged("Gamepads");
                OnPropertyChanged("GamepadIconsColor");
            }
        }

        [Description("Enable texture dumping. Note this will have an adverse effect on performance. Recommended: Unchecked")]
        [IniInfo("Dump", "UnX.Textures")]
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

        [Description("Enable texture injecting. Recommended: Only check when injecting textures other than Gamepad Buttons")]
        [IniInfo("Inject", "UnX.Textures")]
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

        [Description("Gamepad Button Set to use. Recommended: Personal Preference")]
        [IniInfo("Gamepad", "UnX.Textures")]
        public string Gamepad
        {
            get { return _Gamepad; }
            set
            {
                if (value == _Gamepad) return;
                _Gamepad = value;
                OnPropertyChanged();
                OnPropertyChanged("GamepadColor");
            }
        }

        // UnX.Input
        [Description("Hide the mouse cursor intelligently. Recommended: Checked")]
        [IniInfo("ManageCursor", "UnX.Input")]
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

        [Description("Time (in seconds) before an inactive mouse cursor is hidden. Recommend: 0.5")]
        [IniInfo("CursorTimeout", "UnX.Input")]
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

        [Description("XInput controller to use when checking if a controller is present. If you do not have an XInput controller, set to -1 to disable hot-plug detection. When hot-plugging is disabled, cursor will always hide itself even if no controller is connected.")]
        [IniInfo("GamepadSlot", "UnX.Input")]
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

        [Description("Unhide the cursor in response to keyboard input. Recommended: Unchecked")]
        [IniInfo("KeysActivateCursor", "UnX.Input")]
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

        [Description("Enable ability to open the Escape Menu with gamepad. Recommended: Checked")]
        [IniInfo("FourFingerSalute", "UnX.Input")]
        public bool FourFingerSalute
        {
            get { return _FourFingerSalute; }
            set
            {
                if (_FourFingerSalute == value) return;
                _FourFingerSalute = value;
                OnPropertyChanged();
            }
        }

        [Description("Enable ability to toggle boosters with gamepad. Recommended: Checked")]
        [IniInfo("SpecialKeys", "UnX.Input")]
        public bool SpecialKeys
        {
            get { return _SpecialKeys; }
            set
            {
                if (_SpecialKeys == value) return;
                _SpecialKeys = value;
                OnPropertyChanged();
            }
        }

        // UnX.Language
        [Description("General Voiceover Language")]
        [IniInfo("Voice", "UnX.Language")]
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

        [Description("Sound Effects Language")]
        [IniInfo("SoundEffects", "UnX.Language")]
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

        [Description("Full Motion Video Language")]
        [IniInfo("Video", "UnX.Language")]
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
        [IniInfo("Version", "UnX.System")]
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
        [IniInfo("Injector", "UnX.System")]
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

        #region "Read/Save Value Methods"
        private void LoadValues()
        {
            if (ConfigFile == null)
                ConfigFile = new Ini.IniFile(IniPath);
            
            // UnX.Display
            DisableDPIScaling = GetIniBooleanValue(GetKey(nameof(DisableDPIScaling)), GetSection(nameof(DisableDPIScaling)), true);
            EnableFullscreen = GetIniBooleanValue(GetKey(nameof(EnableFullscreen)), GetSection(nameof(EnableFullscreen)), false);

            // UnX.Render
            FlipMode = GetIniBooleanValue(GetKey(nameof(FlipMode)), GetSection(nameof(FlipMode)), true);

            // UnX.Textures
            ResourceRoot = ConfigFile.Read(GetKey(nameof(ResourceRoot)), GetSection(nameof(ResourceRoot)));
            Dump = GetIniBooleanValue(GetKey(nameof(Dump)), GetSection(nameof(Dump)), false);
            Inject = GetIniBooleanValue(GetKey(nameof(Inject)), GetSection(nameof(Inject)), false);
            Gamepad = ConfigFile.Read(GetKey(nameof(Gamepad)), GetSection(nameof(Gamepad)));

            // UnX.Input
            ManageCursor = GetIniBooleanValue(GetKey(nameof(ManageCursor)), GetSection(nameof(ManageCursor)), true);
            CursorTimeout = GetIniFloatValue(GetKey(nameof(CursorTimeout)), GetSection(nameof(CursorTimeout)), 0.5f);
            GamepadSlot = GetIniIntegerValue(GetKey(nameof(GamepadSlot)), GetSection(nameof(GamepadSlot)), -1);
            KeysActivateCursor = GetIniBooleanValue(GetKey(nameof(KeysActivateCursor)), GetSection(nameof(KeysActivateCursor)), false);
            FourFingerSalute = GetIniBooleanValue(GetKey(nameof(FourFingerSalute)), GetSection(nameof(FourFingerSalute)), true);
            SpecialKeys = GetIniBooleanValue(GetKey(nameof(SpecialKeys)), GetSection(nameof(SpecialKeys)), true);

            // UnX.Language
            Voice = GetIniEnumValue<UnXLanguageCodes>(GetKey(nameof(Voice)), GetSection(nameof(Voice)), UnXLanguageCodes.JP);
            SoundEffects = GetIniEnumValue<UnXLanguageCodes>(GetKey(nameof(SoundEffects)), GetSection(nameof(SoundEffects)), UnXLanguageCodes.JP);
            Video = GetIniEnumValue<UnXLanguageCodes>(GetKey(nameof(Video)), GetSection(nameof(Video)), UnXLanguageCodes.JP);

            // UnX.System
            Version = ConfigFile.Read(GetKey(nameof(Version)), GetSection(nameof(Version)));
            Injector = ConfigFile.Read(GetKey(nameof(Injector)), GetSection(nameof(Injector)));
        }

        public void SaveValues()
        {
            if (ConfigFile == null)
                ConfigFile = new Ini.IniFile(IniPath);

            // UnX.Display
            ConfigFile.Write(GetKey(nameof(DisableDPIScaling)), GetIniFormattedString(DisableDPIScaling), GetSection(nameof(DisableDPIScaling)));
            ConfigFile.Write(GetKey(nameof(EnableFullscreen)), GetIniFormattedString(EnableFullscreen), GetSection(nameof(EnableFullscreen)));

            // UnX.Render
            ConfigFile.Write(GetKey(nameof(FlipMode)), GetIniFormattedString(FlipMode), GetSection(nameof(FlipMode)));

            // UnX.Textures
            ConfigFile.Write(GetKey(nameof(ResourceRoot)), GetIniFormattedString(ResourceRoot), GetSection(nameof(ResourceRoot)));
            ConfigFile.Write(GetKey(nameof(Dump)), GetIniFormattedString(Dump), GetSection(nameof(Dump)));
            ConfigFile.Write(GetKey(nameof(Inject)), GetIniFormattedString(Inject), GetSection(nameof(Inject)));
            ConfigFile.Write(GetKey(nameof(Gamepad)), GetIniFormattedString(Gamepad), GetSection(nameof(Gamepad)));

            // UnX.Input
            ConfigFile.Write(GetKey(nameof(ManageCursor)), GetIniFormattedString(ManageCursor), GetSection(nameof(ManageCursor)));
            ConfigFile.Write(GetKey(nameof(CursorTimeout)), GetIniFormattedString(CursorTimeout), GetSection(nameof(CursorTimeout)));
            ConfigFile.Write(GetKey(nameof(GamepadSlot)), GetIniFormattedString(GamepadSlot), GetSection(nameof(GamepadSlot)));
            ConfigFile.Write(GetKey(nameof(KeysActivateCursor)), GetIniFormattedString(KeysActivateCursor), GetSection(nameof(KeysActivateCursor)));
            ConfigFile.Write(GetKey(nameof(FourFingerSalute)), GetIniFormattedString(FourFingerSalute), GetSection(nameof(FourFingerSalute)));
            ConfigFile.Write(GetKey(nameof(SpecialKeys)), GetIniFormattedString(SpecialKeys), GetSection(nameof(SpecialKeys)));

            // UnX.Language
            ConfigFile.Write(GetKey(nameof(Voice)), GetIniFormattedString(Voice), GetSection(nameof(Voice)));
            ConfigFile.Write(GetKey(nameof(SoundEffects)), GetIniFormattedString(SoundEffects), GetSection(nameof(SoundEffects)));
            ConfigFile.Write(GetKey(nameof(Video)), GetIniFormattedString(Video), GetSection(nameof(Video)));
        }
        #endregion

        #region "Get INI Value Methods"
        private string GetIniFormattedString(object val)
        {
            if (val is string)
                return (string)val;
            else if (val is bool)
                return ((bool)val).ToString().ToLower();
            else if (val is float)
                return ((float)val).ToString("F1");
            else if (val is int)
                return ((int)val).ToString();
            else if (val is UnXLanguageCodes)
                return AbbreviationAttribute.Get((UnXLanguageCodes)val);
            else
                return val.ToString();
        }

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

        #region "Public Derived Properties"
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

        public System.Windows.Media.Brush GamepadColor
        {
            get
            {
                if (Directory.Exists(GamePath + Path.AltDirectorySeparatorChar + _ResourceRoot + Path.AltDirectorySeparatorChar + _Gamepad))
                    return System.Windows.Media.Brushes.Green;
                else
                    return System.Windows.Media.Brushes.Red;
            }
        }

        public List<int> GamepadSlots
        {
            get
            {
                return new List<int>(new int[] { -1, 0, 1, 2, 3});
            }
        }

        public List<string> Gamepads
        {
            get
            {
                if (!Directory.Exists(GamePath + Path.AltDirectorySeparatorChar + _ResourceRoot + Path.AltDirectorySeparatorChar + "gamepads"))
                    return new List<string>(new string[] { "Could not find \"gamepads\" directory. Resource Root likely invalid." });

                var directories = new List<string>();
                foreach (var dir in Directory.GetDirectories(GamePath + Path.AltDirectorySeparatorChar + _ResourceRoot + Path.AltDirectorySeparatorChar + "gamepads"))
                {
                    var di = new DirectoryInfo(dir);
                    directories.Add(di.Name);
                    di = null;
                }
                return directories;
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
            sb.AppendLine(string.Format("EnableFullscreen={0}", _EnableFullscreen.ToString()));

            sb.AppendLine("[UnX.Render]");
            sb.AppendLine(string.Format("FlipMode={0}", _FlipMode.ToString()));

            sb.AppendLine("[UnX.Textures]");
            sb.AppendLine(string.Format("ResourceRoot={0}", _ResourceRoot.ToString()));
            sb.Append(string.Format("Dump={0}", _Dump.ToString()));
            sb.Append(string.Format("Inject={0}", _Inject.ToString()));
            sb.AppendLine(string.Format("Gamepad={0}", _Gamepad.ToString()));

            sb.AppendLine("[UnX.Input]");
            sb.AppendLine(string.Format("ManageCursor={0}", _ManageCursor.ToString()));
            sb.AppendLine(string.Format("CursorTimeout={0}", _CursorTimeout.ToString("F1")));
            sb.AppendLine(string.Format("GamepadSlot={0}", _GamepadSlot.ToString()));
            sb.AppendLine(string.Format("KeysActivateCursor={0}", _KeysActivateCursor.ToString()));
            sb.AppendLine(string.Format("FourFingerSalute={0}", _FourFingerSalute.ToString()));
            sb.AppendLine(string.Format("SpecialKeys={0}", _SpecialKeys.ToString()));

            sb.AppendLine("[UnX.Language]");
            sb.AppendLine(string.Format("Voice={0}", _Voice.ToString()));
            sb.AppendLine(string.Format("SoundEffects={0}", _SoundEffects.ToString()));
            sb.AppendLine(string.Format("Video={0}", _Video.ToString()));

            sb.AppendLine("[UnX.System]");
            sb.AppendLine(string.Format("Version={0}", _Version.ToString()));
            sb.Append(string.Format("Injector={0}", _Injector.ToString()));

            return sb.ToString();
        }

        #region "IniInfo Attribute"
        private string GetKey(string Property)
        {
            var prop = typeof(UnXIni).GetProperty(Property);
            if (prop == null) return string.Empty;
            var attr = prop.GetCustomAttribute<IniInfo>();
            if (attr == null) return string.Empty;
            return attr.Key;
        }

        private string GetSection(string Property)
        {
            var prop = typeof(UnXIni).GetProperty(Property);
            if (prop == null) return string.Empty;
            var attr = prop.GetCustomAttribute<IniInfo>();
            if (attr == null) return string.Empty;
            return attr.Section;
        }

        [AttributeUsage(AttributeTargets.Property)]
        private sealed class IniInfo : Attribute
        {
            public string Key { get; set; }
            public string Section { get; set; }

            public IniInfo(string Key, string Section)
            {
                this.Key = Key;
                this.Section = Section;
            }
        }
        #endregion
    }
}
