﻿#pragma checksum "..\..\..\..\..\Systems\Pathfinder\PathfinderControl.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "840113D0350B0358D866DF06AAA134DA"
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
    /// PathfinderControl
    /// </summary>
    public partial class PathfinderControl : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 9 "..\..\..\..\..\Systems\Pathfinder\PathfinderControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Pathfinder_SingleAttack_Button;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\..\..\..\Systems\Pathfinder\PathfinderControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider DamageSlider;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\..\..\..\Systems\Pathfinder\PathfinderControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox DamageDescriptor_TextBox;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\..\..\..\Systems\Pathfinder\PathfinderControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox DamageValue_TextBox;
        
        #line default
        #line hidden
        
        
        #line 48 "..\..\..\..\..\Systems\Pathfinder\PathfinderControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox RollDice_TextBox;
        
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
            System.Uri resourceLocater = new System.Uri("/XMLCharSheets;component/systems/pathfinder/pathfindercontrol.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Systems\Pathfinder\PathfinderControl.xaml"
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
            this.Pathfinder_SingleAttack_Button = ((System.Windows.Controls.Button)(target));
            
            #line 10 "..\..\..\..\..\Systems\Pathfinder\PathfinderControl.xaml"
            this.Pathfinder_SingleAttack_Button.Click += new System.Windows.RoutedEventHandler(this.Pathfinder_SingleAttack_Button_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 28 "..\..\..\..\..\Systems\Pathfinder\PathfinderControl.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.AreaOfEffect_Button_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.DamageSlider = ((System.Windows.Controls.Slider)(target));
            return;
            case 4:
            this.DamageDescriptor_TextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 33 "..\..\..\..\..\Systems\Pathfinder\PathfinderControl.xaml"
            this.DamageDescriptor_TextBox.KeyDown += new System.Windows.Input.KeyEventHandler(this.DamageBox_TextBox_KeyUp);
            
            #line default
            #line hidden
            return;
            case 5:
            this.DamageValue_TextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 38 "..\..\..\..\..\Systems\Pathfinder\PathfinderControl.xaml"
            this.DamageValue_TextBox.KeyUp += new System.Windows.Input.KeyEventHandler(this.DamageBox_TextBox_KeyUp);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 43 "..\..\..\..\..\Systems\Pathfinder\PathfinderControl.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.FortitudeSave_Button_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 44 "..\..\..\..\..\Systems\Pathfinder\PathfinderControl.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ReflexSave_Button_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 45 "..\..\..\..\..\Systems\Pathfinder\PathfinderControl.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.WillSave_Button_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.RollDice_TextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 48 "..\..\..\..\..\Systems\Pathfinder\PathfinderControl.xaml"
            this.RollDice_TextBox.KeyDown += new System.Windows.Input.KeyEventHandler(this.RollDice_TextBox_KeyDown);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

