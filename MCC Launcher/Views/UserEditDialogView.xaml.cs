using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Mvvm;
using MCC_Launcher.ViewModels;
using static MCC_Launcher.ViewModels.UserEditDialogViewModel;

namespace MCC_Launcher.Views
{
    /// <summary>
    /// Interaction logic for UserEditDialogView.xaml
    /// </summary>
    public partial class UserEditDialogView : UserControl
    {
        public UserEditDialogView()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            //if (DataContext is DialogContext context && context.EditedUser != null)
            //{
            //    context.EditedUser.Pw = ((PasswordBox)sender).Password;
            //}

            if (this.DataContext is UserEditDialogViewModel vm && vm._context?.EditedUser != null)
            {
                vm._context.EditedUser.Pw = ((PasswordBox)sender).Password;
            }
        }
    }
}
