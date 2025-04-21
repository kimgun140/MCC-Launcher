using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using MCC_Launcher.Models;
using MCC_Launcher.Services;
using Microsoft.EntityFrameworkCore;
using static MCC_Launcher.ViewModels.UserEditDialogViewModel;

namespace MCC_Launcher.ViewModels
{
    public class RoleDetailDialogViewModel : ViewModelBase
    {
        protected ICurrentDialogService CurrentDialogService => GetService<ICurrentDialogService>();
        protected IMessageBoxService MessageBoxService => GetService<IMessageBoxService>();
        private readonly LauncherService _launcherService = new LauncherService();
        public string RoleName
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand => new DelegateCommand(() => CurrentDialogService.Close());
        private DialogContext _context;
        public RoleDetailDialogViewModel()
        {
            SaveCommand = new DelegateCommand(OnRoleSave);

        }
        public void OnRoleSave()
        {
            if (string.IsNullOrWhiteSpace(RoleName))
            {
                MessageBoxService.Show("역할 이름을 입력해주세요.");
                return;
            }

            var newRole = new Role { RoleName = RoleName };
            if (_launcherService.SaveRole(newRole))
            {
                MessageBoxService.Show("저장 완료");
                CurrentDialogService.Close();
            }
            else
            {
                MessageBoxService.Show("저장 실패");
            }
        }
    }

}
