﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18063
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.0.30319.17929.
// 
namespace DiGit.Model {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
    public partial class DiGitConfig {
        
        private DiGitConfigRepository[] repositoriesField;
        
        private DiGitConfigSettings settingsField;
        
        private DiGitConfigCommand[] commandsField;
        
        private string verField;
        
        private bool isBetaUserField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Repository", IsNullable=false)]
        public DiGitConfigRepository[] Repositories {
            get {
                return this.repositoriesField;
            }
            set {
                this.repositoriesField = value;
            }
        }
        
        /// <remarks/>
        public DiGitConfigSettings Settings {
            get {
                return this.settingsField;
            }
            set {
                this.settingsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Command", IsNullable=false)]
        public DiGitConfigCommand[] Commands {
            get {
                return this.commandsField;
            }
            set {
                this.commandsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ver {
            get {
                return this.verField;
            }
            set {
                this.verField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool isBetaUser {
            get {
                return this.isBetaUserField;
            }
            set {
                this.isBetaUserField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class DiGitConfigRepository {
        
        private string pathField;
        
        private bool isActiveField;
        
        private string valueField;
        
        public DiGitConfigRepository() {
            this.isActiveField = true;
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string path {
            get {
                return this.pathField;
            }
            set {
                this.pathField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(true)]
        public bool isActive {
            get {
                return this.isActiveField;
            }
            set {
                this.isActiveField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class DiGitConfigSettings {
        
        private DiGitConfigSettingsShowHideHotkey showHideHotkeyField;
        
        private DiGitConfigSettingsVisualSettings visualSettingsField;
        
        /// <remarks/>
        public DiGitConfigSettingsShowHideHotkey ShowHideHotkey {
            get {
                return this.showHideHotkeyField;
            }
            set {
                this.showHideHotkeyField = value;
            }
        }
        
        /// <remarks/>
        public DiGitConfigSettingsVisualSettings VisualSettings {
            get {
                return this.visualSettingsField;
            }
            set {
                this.visualSettingsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class DiGitConfigSettingsShowHideHotkey {
        
        private string modifiersField;
        
        private string keyField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string modifiers {
            get {
                return this.modifiersField;
            }
            set {
                this.modifiersField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string key {
            get {
                return this.keyField;
            }
            set {
                this.keyField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class DiGitConfigSettingsVisualSettings {
        
        private double _bubblesOpacityField;
        
        public DiGitConfigSettingsVisualSettings() {
            this._bubblesOpacityField = 0.83D;
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute(0.83D)]
        public double _bubblesOpacity {
            get {
                return this._bubblesOpacityField;
            }
            set {
                this._bubblesOpacityField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class DiGitConfigCommand {
        
        private string headerField;
        
        private string fileNameField;
        
        private string argumentsField;
        
        private string windowStyleField;
        
        private string optionsField;
        
        private string helpTextField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string header {
            get {
                return this.headerField;
            }
            set {
                this.headerField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string fileName {
            get {
                return this.fileNameField;
            }
            set {
                this.fileNameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string arguments {
            get {
                return this.argumentsField;
            }
            set {
                this.argumentsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string windowStyle {
            get {
                return this.windowStyleField;
            }
            set {
                this.windowStyleField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string options {
            get {
                return this.optionsField;
            }
            set {
                this.optionsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string helpText {
            get {
                return this.helpTextField;
            }
            set {
                this.helpTextField = value;
            }
        }
    }
}
