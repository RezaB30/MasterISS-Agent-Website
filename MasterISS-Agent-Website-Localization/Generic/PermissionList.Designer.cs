﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MasterISS_Agent_Website_Localization.Generic {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class PermissionList {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal PermissionList() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("MasterISS_Agent_Website_Localization.Generic.PermissionList", typeof(PermissionList).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to İş Emri Atama.
        /// </summary>
        public static string AssignWorkOrder {
            get {
                return ResourceManager.GetString("AssignWorkOrder", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ödeme Temsilcisi.
        /// </summary>
        public static string PaymentManager {
            get {
                return ResourceManager.GetString("PaymentManager", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Satış Temsilcisi.
        /// </summary>
        public static string SaleManager {
            get {
                return ResourceManager.GetString("SaleManager", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kurulum Temsilcisi.
        /// </summary>
        public static string SetupManager {
            get {
                return ResourceManager.GetString("SetupManager", resourceCulture);
            }
        }
    }
}
