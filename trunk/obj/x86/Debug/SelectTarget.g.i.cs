﻿#pragma checksum "..\..\..\SelectTarget.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "562627DD9FC7E7CCE8A82CDDCFF46934"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace XMLCharSheets {
    
    
    /// <summary>
    /// SelectTarget
    /// </summary>
    public partial class SelectTarget : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 14 "..\..\..\SelectTarget.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox TargetCharacters_ListBox;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\SelectTarget.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox Shared_Attacks_ListBox;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\..\SelectTarget.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox Other_Traits_ListBox;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\..\SelectTarget.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox DamageTypes_ListBox;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/XMLCharSheets;component/selecttarget.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\SelectTarget.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 6 "..\..\..\SelectTarget.xaml"
            ((XMLCharSheets.SelectTarget)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.GetName_KeyDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.TargetCharacters_ListBox = ((System.Windows.Controls.ListBox)(target));
            return;
            case 3:
            this.Shared_Attacks_ListBox = ((System.Windows.Controls.ListBox)(target));
            
            #line 28 "..\..\..\SelectTarget.xaml"
            this.Shared_Attacks_ListBox.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.Shared_Attacks_ListBox_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 4:
            this.Other_Traits_ListBox = ((System.Windows.Controls.ListBox)(target));
            return;
            case 5:
            this.DamageTypes_ListBox = ((System.Windows.Controls.ListBox)(target));
            return;
            case 6:
            
            #line 41 "..\..\..\SelectTarget.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.OK_Button_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 42 "..\..\..\SelectTarget.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Cancel_Button_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
