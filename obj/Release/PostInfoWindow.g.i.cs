﻿#pragma checksum "..\..\PostInfoWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "089A396A3FEBDF0AB7B6056BC679A65BAE4D321AFC048BAD5BB294551252F2C7"
//------------------------------------------------------------------------------
// <auto-generated>
//     이 코드는 도구를 사용하여 생성되었습니다.
//     런타임 버전:4.0.30319.42000
//
//     파일 내용을 변경하면 잘못된 동작이 발생할 수 있으며, 코드를 다시 생성하면
//     이러한 변경 내용이 손실됩니다.
// </auto-generated>
//------------------------------------------------------------------------------

using KSP_WPF;
using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
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


namespace KSP_WPF {
    
    
    /// <summary>
    /// PostInfoWindow
    /// </summary>
    public partial class PostInfoWindow : MahApps.Metro.Controls.MetroWindow, System.Windows.Markup.IComponentConnector {
        
        
        #line 13 "..\..\PostInfoWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal MahApps.Metro.Controls.MetroAnimatedTabControl TC_Main;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\PostInfoWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ScrollViewer SV_Emotions;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\PostInfoWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel SP_Emotions;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\PostInfoWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ScrollViewer SV_Shares;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\PostInfoWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel SP_Shares;
        
        #line default
        #line hidden
        
        
        #line 25 "..\..\PostInfoWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ScrollViewer SV_UP;
        
        #line default
        #line hidden
        
        
        #line 26 "..\..\PostInfoWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel SP_UP;
        
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
            System.Uri resourceLocater = new System.Uri("/KSP-WPF;component/postinfowindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\PostInfoWindow.xaml"
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
            
            #line 10 "..\..\PostInfoWindow.xaml"
            ((KSP_WPF.PostInfoWindow)(target)).PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.MetroWindow_PreviewKeyDown);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 12 "..\..\PostInfoWindow.xaml"
            ((System.Windows.Controls.Grid)(target)).PreviewGotKeyboardFocus += new System.Windows.Input.KeyboardFocusChangedEventHandler(this.MetroWindow_PreviewGotKeyboardFocus);
            
            #line default
            #line hidden
            return;
            case 3:
            this.TC_Main = ((MahApps.Metro.Controls.MetroAnimatedTabControl)(target));
            return;
            case 4:
            this.SV_Emotions = ((System.Windows.Controls.ScrollViewer)(target));
            
            #line 15 "..\..\PostInfoWindow.xaml"
            this.SV_Emotions.PreviewMouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.ScrollViewer_PreviewMouseWheel);
            
            #line default
            #line hidden
            return;
            case 5:
            this.SP_Emotions = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 6:
            this.SV_Shares = ((System.Windows.Controls.ScrollViewer)(target));
            
            #line 20 "..\..\PostInfoWindow.xaml"
            this.SV_Shares.PreviewMouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.ScrollViewer_PreviewMouseWheel);
            
            #line default
            #line hidden
            return;
            case 7:
            this.SP_Shares = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 8:
            this.SV_UP = ((System.Windows.Controls.ScrollViewer)(target));
            
            #line 25 "..\..\PostInfoWindow.xaml"
            this.SV_UP.PreviewMouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.ScrollViewer_PreviewMouseWheel);
            
            #line default
            #line hidden
            return;
            case 9:
            this.SP_UP = ((System.Windows.Controls.StackPanel)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

