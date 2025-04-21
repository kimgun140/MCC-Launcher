using DevExpress.Mvvm;
using MCC_Launcher.Views;

namespace MCC_Launcher.ViewModels
{
    public class RoleManagementViewModel : ViewModelBase
    {
        // 이게 다이얼로그니까 
        public ViewModelBase CurrentViewModel
        {
            get => GetValue<ViewModelBase>();
            set => SetValue(value);
        }

        public DelegateCommand ShowRoleCommand { get; }
        public DelegateCommand ShowPermissionCommand { get; }
        public DelegateCommand ShowUserCommand { get; }
        public RoleManagementViewModel()
        {
            ShowRoleCommand = new DelegateCommand(() =>
            {
                CurrentViewModel = new RoleEditDialogViewModel();
               
            });

            ShowPermissionCommand = new DelegateCommand(() =>
            {
                CurrentViewModel = new RolePermissionManagementViewModel();
            });

            ShowUserCommand = new DelegateCommand(() =>
            {
                //MessageBoxService.Show("Hello, World!", "Greeting", MessageBoxButton.OK, MessageBoxImage.Information);
                CurrentViewModel = new UserManagementViewModel();
                //CurrentViewModel.Parameter = new LoginResultContext { AuthenticatedUser = AuthenticatedUser };
            });
        }




    }
}
