﻿#pragma checksum "..\..\CreateProfileWnd.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "A1B7CA46E214BED6324D7C26AC46C9CC"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
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


namespace TextCategorization {
    
    
    /// <summary>
    /// CreateProfileWnd
    /// </summary>
    public partial class CreateProfileWnd : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 1 "..\..\CreateProfileWnd.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal TextCategorization.CreateProfileWnd CreateProfileWnd1;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\CreateProfileWnd.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox ProfileNameTb;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\CreateProfileWnd.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CreateBtn;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\CreateProfileWnd.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox InpFileTb;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\CreateProfileWnd.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ChangeFileBtn;
        
        #line default
        #line hidden
        
        
        #line 45 "..\..\CreateProfileWnd.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ProgressBar CreatingProgress;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\CreateProfileWnd.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label LinksLb;
        
        #line default
        #line hidden
        
        
        #line 56 "..\..\CreateProfileWnd.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label AnalizedLb;
        
        #line default
        #line hidden
        
        
        #line 61 "..\..\CreateProfileWnd.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label InvalidLinksLb;
        
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
            System.Uri resourceLocater = new System.Uri("/TextCategorization;component/createprofilewnd.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\CreateProfileWnd.xaml"
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
            this.CreateProfileWnd1 = ((TextCategorization.CreateProfileWnd)(target));
            return;
            case 2:
            this.ProfileNameTb = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.CreateBtn = ((System.Windows.Controls.Button)(target));
            
            #line 26 "..\..\CreateProfileWnd.xaml"
            this.CreateBtn.Click += new System.Windows.RoutedEventHandler(this.CreateBtn_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.InpFileTb = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.ChangeFileBtn = ((System.Windows.Controls.Button)(target));
            
            #line 44 "..\..\CreateProfileWnd.xaml"
            this.ChangeFileBtn.Click += new System.Windows.RoutedEventHandler(this.ChangeFileBtn_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.CreatingProgress = ((System.Windows.Controls.ProgressBar)(target));
            return;
            case 7:
            this.LinksLb = ((System.Windows.Controls.Label)(target));
            return;
            case 8:
            this.AnalizedLb = ((System.Windows.Controls.Label)(target));
            return;
            case 9:
            this.InvalidLinksLb = ((System.Windows.Controls.Label)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

