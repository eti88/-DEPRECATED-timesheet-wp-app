﻿

#pragma checksum "C:\Users\etienne\documents\visual studio 2013\Projects\timesheet\timesheet\View\HubPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "D445E123BACBAB042E15481E94CFEBCB"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace timesheet
{
    partial class HubPage : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 54 "..\..\View\HubPage.xaml"
                ((global::Windows.UI.Xaml.FrameworkElement)(target)).Loaded += this.ChartToday_Loader;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 134 "..\..\View\HubPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.AppBarButtonAbout_Click;
                 #line default
                 #line hidden
                break;
            case 3:
                #line 135 "..\..\View\HubPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.ButtonBase)(target)).Click += this.AppBarButtonSettings_Click;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}

