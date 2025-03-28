using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace MCC_Launcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 코드에서 테마 지정
            //ThemeManager.ApplicationThemeName = "Office2019DarkGray"; // 어두운 테마
            //ThemeManager.ApplicationThemeName = "Win11Dark"; // 어두운 테마

            ThemeManager.ApplicationThemeName = "VS2019Dark"; // 어두운 테마
        }
    }
}
