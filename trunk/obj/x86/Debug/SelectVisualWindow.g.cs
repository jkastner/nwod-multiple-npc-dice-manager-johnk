﻿#pragma checksum "..\..\..\SelectVisualWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "1CDD506D948735616FCEC5BC18095F31"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18052
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
    /// SelectVisualWindow
    /// </summary>
    public partial class SelectVisualWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 28 "..\..\..\SelectVisualWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox TrimList_TextBox;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\SelectVisualWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox SearchedDisplayItems_ListBox;
        
        #line default
        #line hidden
        
        
        #line 47 "..\..\..\SelectVisualWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox TeamSelection_ListBox;
        
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
            System.Uri resourceLocater = new System.Uri("/XMLCharSheets;component/selectvisualwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\SelectVisualWindow.xaml"
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
            
            #line 6 "..\..\..\SelectVisualWindow.xaml"
            ((XMLCharSheets.SelectVisualWindow)(target)).Loaded += new System.Windows.RoutedEventHandler(this.WindowLoaded);
            
            #line default
            #line hidden
            
            #line 7 "..\..\..\SelectVisualWindow.xaml"
            ((XMLCharSheets.SelectVisualWindow)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.SelectVisualWindow_KeyDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.TrimList_TextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 28 "..\..\..\SelectVisualWindow.xaml"
            this.TrimList_TextBox.KeyUp += new System.Windows.Input.KeyEventHandler(this.TrimList_TextBox_KeyUp);
            
            #line default
            #line hidden
            return;
            case 3:
            this.SearchedDisplayItems_ListBox = ((System.Windows.Controls.ListBox)(target));
            return;
            case 4:
            this.TeamSelection_ListBox = ((System.Windows.Controls.ListBox)(target));
            return;
            case 5:
            
            #line 75 "..\..\..\SelectVisualWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.OK_Button_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 77 "..\..\..\SelectVisualWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Cancel_Button_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

