﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18063
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DiGit.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("DiGit.xml")]
        public string ConfigFilePath {
            get {
                return ((string)(this["ConfigFilePath"]));
            }
            set {
                this["ConfigFilePath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("O:\\dbMotion-Development\\dbMotion 2005\\Dev\\TalB\\DiGit\\Info")]
        public string InfoUrl {
            get {
                return ((string)(this["InfoUrl"]));
            }
            set {
                this["InfoUrl"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Users.xml")]
        public string RegistrationFile {
            get {
                return ((string)(this["RegistrationFile"]));
            }
            set {
                this["RegistrationFile"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("DiGitVersionInfo{0}.xml")]
        public string InfoBaseFileName {
            get {
                return ((string)(this["InfoBaseFileName"]));
            }
            set {
                this["InfoBaseFileName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("5")]
        public int ReadInfoDelaySec {
            get {
                return ((int)(this["ReadInfoDelaySec"]));
            }
            set {
                this["ReadInfoDelaySec"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public int WriteInfoDelaySec {
            get {
                return ((int)(this["WriteInfoDelaySec"]));
            }
            set {
                this["WriteInfoDelaySec"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool UserRegistered {
            get {
                return ((bool)(this["UserRegistered"]));
            }
            set {
                this["UserRegistered"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("5")]
        public double CheckUpdateIntervalHr {
            get {
                return ((double)(this["CheckUpdateIntervalHr"]));
            }
            set {
                this["CheckUpdateIntervalHr"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("30")]
        public int MenuPathLength {
            get {
                return ((int)(this["MenuPathLength"]));
            }
            set {
                this["MenuPathLength"] = value;
            }
        }
    }
}