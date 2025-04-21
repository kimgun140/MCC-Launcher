using DevExpress.Mvvm;
using MCC_Launcher.Models;
using MCC_Launcher.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.ApplicationServices;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using static MCC_Launcher.ViewModels.UserManagementViewModel;

namespace MCC_Launcher.ViewModels
{
    public class UserEditDialogViewModel : ViewModelBase, ISupportParameter
    {
        LauncherService LauncherService = new LauncherService();
        IMessageBoxService MessageBoxService { get { return GetService<IMessageBoxService>(); } }
        public class DialogContext
        {
            public UserViewModel EditedUser { get; set; }
            public UserEditMode Mode { get; set; } // 추가, 삭제, 변경

        }
        public ObservableCollection<Role> Roles { get; set; } = new();
        public Role SelectedRole
        {
            get => GetValue<Role>();
            set => SetValue(value);
        }

        public string SelectedRoleName
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public UserViewModel EditedUser
        //편집용 
        {
            get => GetValue<UserViewModel>();

            set => SetValue(value);
        }

        private DialogContext _context;
        protected ICurrentDialogService CurrentDialogService => GetService<ICurrentDialogService>();

        public object Parameter
        {
            get => null!;
            set
            {

                // 여기서 mode에 따라서 바꿔주기 
                if (value is DialogContext context)
                {
                    _context = context;

                    // 복사본으로 작업 (원본 직접 수정 방지)
                    if (context.EditedUser != null)
                        EditedUser = new UserViewModel
                        {
                            UserId = context.EditedUser.UserId,
                            UserName = context.EditedUser.UserName,
                            Activated = context.EditedUser.Activated,

                            RoleName = context.EditedUser.RoleName,

                            RoleId = context.EditedUser.RoleId

                        };
                    _context.EditedUser = EditedUser;
                    _context.Mode = context.Mode;
                    //SelectedRole = Roles.FirstOrDefault(r => r.RoleId == EditedUser.RoleId);
                    SelectedRole = Roles.FirstOrDefault(r => r.RoleId == EditedUser.RoleId);
                    // Roleid가 0이다  

                    //RaisePropertiesChanged(nameof(EditedUser), nameof(SelectedRole));
                }

                RaisePropertyChanged(nameof(Parameter));
            }
        }

        public DelegateCommand ConfirmCommand => new DelegateCommand(OnConfirm);
        public DelegateCommand CancelCommand => new DelegateCommand(() => CurrentDialogService.Close());
        //public DelegateCommand CancelCommand => new DelegateCommand(() => CurrentDialogService.Close(MessageResult.Cancel));
        public UserEditDialogViewModel()
        {
            // 예시 데이터 로딩
            using var context = new LauncherDbContext();
            var rolesFromDb = context.Roles.ToList();
            foreach (var role in rolesFromDb)
            {
                Roles.Add(role);
            }
        }

        private void OnConfirm()
        {
            if (_context == null)
                return;

            _context.EditedUser.UserId = EditedUser.UserId;
            _context.EditedUser.UserName = EditedUser.UserName;
            _context.EditedUser.Activated = EditedUser.Activated;
            _context.EditedUser.RoleName = SelectedRole.RoleName;
            //여기를 넣어 달라고해야겠네 
            _context.EditedUser.RoleId = SelectedRole.RoleId;

            switch (_context.Mode)
            // 이렇게 짜면 saveruser에서 검사하니까 ㄱㅊ구나 
            {
                case UserEditMode.추가:
                case UserEditMode.변경:
                    //MessageBoxService.Show("User updated successfully.");
                    if (LauncherService.SaveUser(_context.EditedUser))
                    {
                        // 성공적으로 저장된 경우, 같은 메서드 사용하니까 
                        MessageBoxService.Show("User saved successfully.");
                        CurrentDialogService.Close();

                    }
                    else
                    {
                        // 저장 실패 시 처리
                        MessageBoxService.Show("Failed to save user.");
                    }
                    break;

                case UserEditMode.삭제:
                    if (LauncherService.UserDelete(_context.EditedUser))
                    {
                        MessageBoxService.Show("User deleted successfully.");
                        CurrentDialogService.Close();

                        return;
                    }
                    else
                    {
                        MessageBoxService.Show("사용자 삭제에 실패했습니다.");
                    }
                    break;
            }

            //if (_context.Mode == UserEditMode.삭제)
            //{
            //    if (LauncherService.UserDelete(_context.EditedUser))
            //    {
            //        MessageBoxService.Show("User deleted successfully.");
            //        CurrentDialogService.Close();

            //        return;
            //    }
            //    //이것도 bool로 바꿔야할듯 
            //}
            //else if (_context.Mode == UserEditMode.추가 || _context.Mode == UserEditMode.변경)
            //    if (LauncherService.SaveUser(_context.EditedUser))
            //    {
            //        // 성공적으로 저장된 경우, 같은 메서드 사용하니까 
            //        MessageBoxService.Show("User saved successfully.");
            //        CurrentDialogService.Close();

            //    }
            //    else
            //    {
            //        // 저장 실패 시 처리
            //        MessageBoxService.Show("Failed to save user.");
            //    }




        }



    }
}
