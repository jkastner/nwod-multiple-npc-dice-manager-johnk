﻿#pragma checksum "..\..\..\..\..\Systems\NWoD\NWoDControl.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "97632095216C40E8D1A1DE04AB3F476C"
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


namespace CombatAutomationTheater {
    
    
    /// <summary>
    /// NWoDControl
    /// </summary>
    public partial class NWoDControl : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 12 "..\..\..\..\..\Systems\NWoD\NWoDControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Do_Bashing_Button;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\..\..\..\Systems\NWoD\NWoDControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Do_Lethal_Button;
        
        #line default
        #line hidden
        
        
        #line 52 "..\..\..\..\..\Systems\NWoD\NWoDControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Do_Aggrivated_Button;
        
        #line default
        #line hidden
        
        
        #line 85 "..\..\..\..\..\Systems\NWoD\NWoDControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Blood_Buff_Button;
        
        #line default
        #line hidden
        
        
        #line 96 "..\..\..\..\..\Systems\NWoD\NWoDControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Blood_Heal_Button;
        
        #line default
        #line hidden
        
        
        #line 106 "..\..\..\..\..\Systems\NWoD\NWoDControl.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Refill_Vitae_Button;
        
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
            System.Uri resourceLocater = new System.Uri("/XMLCharSheets;component/systems/nwod/nwodcontrol.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\Systems\NWoD\NWoDControl.xaml"
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
            this.Do_Bashing_Button = ((System.Windows.Controls.Button)(target));
            
            #line 13 "..\..\..\..\..\Systems\NWoD\NWoDControl.xaml"
            this.Do_Bashing_Button.Click += new System.Windows.RoutedEventHandler(this.Do_Bashing_Button_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.Do_Lethal_Button = ((System.Windows.Controls.Button)(target));
            
            #line 32 "..\..\..\..\..\Systems\NWoD\NWoDControl.xaml"
            this.Do_Lethal_Button.Click += new System.Windows.RoutedEventHandler(this.Do_Lethal_Button_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.Do_Aggrivated_Button = ((System.Windows.Controls.Button)(target));
            
            #line 53 "..\..\..\..\..\Systems\NWoD\NWoDControl.xaml"
            this.Do_Aggrivated_Button.Click += new System.Windows.RoutedEventHandler(this.Do_Aggrivated_Button_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 71 "..\..\..\..\..\Systems\NWoD\NWoDControl.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Roll_Resolve_And_Composure);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 74 "..\..\..\..\..\Systems\NWoD\NWoDControl.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Roll_Resolve_and_Resistance);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 77 "..\..\..\..\..\Systems\NWoD\NWoDControl.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Roll_Composure_and_Resistance);
            
            #line default
            #line hidden
            return;
            case 7:
            this.Blood_Buff_Button = ((System.Windows.Controls.Button)(target));
            
            #line 86 "..\..\..\..\..\Systems\NWoD\NWoDControl.xaml"
            this.Blood_Buff_Button.Click += new System.Windows.RoutedEventHandler(this.Blood_Buff_Button_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.Blood_Heal_Button = ((System.Windows.Controls.Button)(target));
            
            #line 97 "..\..\..\..\..\Systems\NWoD\NWoDControl.xaml"
            this.Blood_Heal_Button.Click += new System.Windows.RoutedEventHandler(this.Blood_Heal_Button_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.Refill_Vitae_Button = ((System.Windows.Controls.Button)(target));
            
            #line 107 "..\..\..\..\..\Systems\NWoD\NWoDControl.xaml"
            this.Refill_Vitae_Button.Click += new System.Windows.RoutedEventHandler(this.Refill_Vitae_Button_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

