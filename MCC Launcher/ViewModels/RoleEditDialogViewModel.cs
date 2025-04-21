using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using MCC_Launcher.Models;
using MCC_Launcher.Services;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;

namespace MCC_Launcher.ViewModels
{
    public class RoleEditDialogViewModel : ViewModelBase, ISupportParameter
    {//역할관리 
        IMessageBoxService MessageBoxService { get { return GetService<IMessageBoxService>(); } }
        LauncherService LauncherService = new LauncherService();

        public IDialogService DialogService => this.GetService<IDialogService>();

        public ObservableCollection<Role> Roles { get; set; } = new();

        public Role SelectedRole
        {
            get => GetValue<Role>();
            set => SetValue(value);
        }
        public string Name
        {
            get => GetValue<string>();
            set => SetValue(value);
        }


        //public Role Roles
        //{
        //    get => GetValue<Role>();
        //    set => SetValue(value);
        //}

        public DelegateCommand OnAddRoleCommand { get; }
        public DelegateCommand OnDeleteCommand { get; }
        public DelegateCommand OnEditCommand { get; }

        public RoleEditDialogViewModel()
        {
            RoleLoad();
            OnAddRoleCommand = new DelegateCommand(OnAddRole);
            OnDeleteCommand = new DelegateCommand(OnDeleteRole);
            OnEditCommand = new DelegateCommand(OnEditRole);

        }
        public void RoleLoad()
        {// 역할 목록 로드 
            //Roles = LauncherService.LoadRoles();


            using var context = new LauncherDbContext();
            var rolesFromDb = context.Roles.ToList();

            Roles.Clear(); // 기존 목록 초기화
            foreach (var role in rolesFromDb)
            {
                Roles.Add(role); // UI 바인딩용으로 추가
            }
        }


        public void OnAddRole()
        {
            // 역할 관리 다이얼로그 열기
            var result = DialogService.ShowDialog(
                    dialogCommands: null,
                    documentType: "RoleDetailDialogView",
                    title: "역할추가",
                    viewModel: null,
                    parameter: null,
                    parentViewModel: null
                );
            RoleLoad();

        }
        public void OnEditRole()
        {
            //using var context = new LauncherDbContext();
            //var existing = context.Roles.Find(updatedRole.RoleId);
            //if (existing != null)
            //{
            //    existing.RoleName = updatedRole.RoleName;
            //    context.SaveChanges();
            //}
        }
        public void OnDeleteRole()
        {
            //using var context = new LauncherDbContext();
            //var existing = context.Roles.Find(role.RoleId);
            //if (existing != null)
            //{
            //    context.Roles.Remove(existing);
            //    context.SaveChanges();
            //}

            if (SelectedRole == null)
            {
                MessageBoxService.Show("삭제할 역할을 선택하세요.");
                return;
            }

            var result = MessageBoxService.Show(
                $"[{SelectedRole.RoleName}] 역할을 삭제하시겠습니까?",
                "삭제 확인",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                if (LauncherService.DeleteRole(SelectedRole.RoleId))
                {
                    MessageBoxService.Show("삭제되었습니다.");
                    RoleLoad(); 
                }
                else
                {
                    MessageBoxService.Show("삭제에 실패했습니다.");
                }
            }
        }
    }
}
