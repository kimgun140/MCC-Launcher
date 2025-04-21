using DevExpress.Mvvm;
using MCC_Launcher.Models;
using MCC_Launcher.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Data;
using static MCC_Launcher.ViewModels.LoginViewModel;

namespace MCC_Launcher.ViewModels
{
    public class RolePermissionManagementViewModel : ViewModelBase, ISupportParameter
    {
        LauncherService _launcherService = new LauncherService();
        IMessageBoxService MessageBoxService { get { return GetService<IMessageBoxService>(); } }
        public ObservableCollection<Role> Roles { get; set; } = new();//역할들 
        // 권한 설치, 실행 추가 가능  
        public ObservableCollection<ProgramsEntity> Programs { get; set; } = new();//프로그램

        public ObservableCollection<PermissionViewModel> AvailablePermissions { get; set; } = new();// 


        public string NewPermissionName
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public DelegateCommand AddPermissionCommand { get; set; }
        //권한도 목록으로 만들어서 바인딩되게 만들어야겠다.
        // 프로그램 , 역할이 선택되었을때 
        public Role SelectedRole
        {
            get => GetValue<Role>();
            set
            {
                SetValue(value);
                LoadAssignedPermissions();

            }
        }
        public ProgramsEntity SelectedProgram
        {
            get => GetValue<ProgramsEntity>();
            set
            {
                SetValue(value);
                LoadAssignedPermissions();
                //여기에 넣어야지
            }
        }

        public Permission SelectedPermission
        {
            // 이거 말고 객체를 그대로 받아옴
            get => GetValue<Permission>();
            set => SetValue(value);
        }

        public DelegateCommand SaveCommand { get; set; }


        //public class DialogContext
        //{
        //    public UserViewModel EditedUser { get; set; }
        //    public UserEditMode Mode { get; set; } // 추가, 삭제, 변경

        //}


        //private DialogContext _context;

        //public object Parameter
        //{
        //    get => null!;
        //    set
        //    {
        //        if (value is DialogContext context)
        //            _context = context;
        //    }
        //}

        public RolePermissionManagementViewModel()
        {
            LoadPrograms();
            LoadRoles();
            LoadPermissions();
            SaveCommand = new DelegateCommand(SaveChanges);
            AddPermissionCommand = new DelegateCommand(AddPermission);

            ShowCommand = new DelegateCommand(() =>
            {
                //MessageBoxService.Show("Hello, World!", "Greeting", MessageBoxButton.OK, MessageBoxImage.Information);
                CurrentViewModel = new RoleEditDialogViewModel();
                //CurrentViewModel.Parameter = new LoginResultContext { AuthenticatedUser = AuthenticatedUser };
            });
        }

        public void LoadPrograms()
        {
            Programs = _launcherService.LoadProgramsEntities();
        }

        public void LoadRoles()
        {
            Roles = _launcherService.LoadRoles();
        }

        public void LoadPermissions()
        {
            AvailablePermissions = _launcherService.LoadPermissions();

        }

        private void SaveChanges()
        // 저장버튼 누르면 역할에 권한 설정하기 
        {
            if (SelectedRole != null && SelectedProgram != null)
                _launcherService.SavePermissionChanges(SelectedProgram, SelectedRole, AvailablePermissions);
            // 역할, 프로그램 권한을 선택안하면 기본값이 그대로 되게

        }

        //AvailablePermissions = LauncherService.LoadAssignedPermissions(SelectedRole.RoleId, SelectedProgram.ProgramCode);
        private void LoadAssignedPermissions()
        {
            if (SelectedRole != null && SelectedProgram != null)
            {
                
                AvailablePermissions = _launcherService.LoadAssignedPermissions(SelectedRole.RoleId, SelectedProgram.ProgramCode);
                RaisePropertyChanged(nameof(AvailablePermissions));

            }
        }

       public ViewModelBase CurrentViewModel
        {
            get => GetValue<ViewModelBase>();
            set => SetValue(value);
        }
        public DelegateCommand ShowCommand { get; set; }


        private void AddPermission()
        {
            if (!string.IsNullOrWhiteSpace(NewPermissionName))
            {
                var success = _launcherService.AddPermission(NewPermissionName);
                if (success)
                {
                    LoadPermissions(); // 또는 AvailablePermissions 재로드
                    NewPermissionName = string.Empty;
                }
                else
                {
                    MessageBoxService.Show("중복되었거나 저장에 실패했습니다.");
                }
            }
        }


    }
}
