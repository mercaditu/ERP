﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Cognitivo.Product {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
    public sealed partial class TransferSetting : global::System.Configuration.ApplicationSettingsBase {
        
        private static TransferSetting defaultInstance = ((TransferSetting)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new TransferSetting())));
        
        public static TransferSetting Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool movebytruck {
            get {
                return ((bool)(this["movebytruck"]));
            }
            set {
                this["movebytruck"] = value;
            }
        }
    }
}
