﻿#pragma checksum "..\..\..\CharacterCreationControl.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "6A4D2FF2BADB7DFA01D45ED2F6D6985A"
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
using XMLCharSheets;


namespace XMLCharSheets {
    
    
    /// <summary>
    /// CharacterCreationControl
    /// </summary>
    public partial class CharacterCreationControl : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 36 "..\..\..\CharacterCreationControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox CharacterSheetSearcher_TextBox;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\..\CharacterCreationControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox AvailableNPCS_ListBox;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\..\CharacterCreationControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox CharacterName_TextBox;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\..\CharacterCreationControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox ImageSearch_TextBox;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\..\CharacterCreationControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox PictureSearch_ListBox;
        
        #line default
        #line hidden
        
        
        #line 79 "..\..\..\CharacterCreationControl.xaml"
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
            System.Uri resourceLocater = new System.Uri("/XMLCharSheets;component/charactercreationcontrol.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\CharacterCreationControl.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
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
            
            #line 8 "..\..\..\CharacterCreationControl.xaml"
            ((XMLCharSheets.CharacterCreationControl)(target)).Loaded += new System.Windows.RoutedEventHandler(this.CharacterCreationControl_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.CharacterSheetSearcher_TextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 36 "..\..\..\CharacterCreationControl.xaml"
            this.CharacterSheetSearcher_TextBox.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.CharacterSheetSearcher_TextBox_TextChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            this.AvailableNPCS_ListBox = ((System.Windows.Controls.ListBox)(target));
            return;
            case 4:
            this.CharacterName_TextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 38 "..\..\..\CharacterCreationControl.xaml"
            this.CharacterName_TextBox.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.CharacterName_TextBox_TextChanged);
            
            #line default
            #line hidden
            return;
            case 5:
            this.ImageSearch_TextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 42 "..\..\..\CharacterCreationControl.xaml"
            this.ImageSearch_TextBox.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.ImageSearch_TextBox_TextChanged);
            
            #line default
            #line hidden
            return;
            case 6:
            this.PictureSearch_ListBox = ((System.Windows.Controls.ListBox)(target));
            return;
            case 7:
            this.TeamSelection_ListBox = ((System.Windows.Controls.ListBox)(target));
            return;
            case 8:
            
            #line 108 "..\..\..\CharacterCreationControl.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.CreateCharacter_ButtonClicked);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

