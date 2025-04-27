using DevExpress.Mvvm;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Mvvm.Xpf;
using DevExpress.Xpf.Core;
using MCC_Launcher.Models;
using MCC_Launcher.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.ApplicationServices;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using static MCC_Launcher.ViewModels.LoginViewModel;

namespace MCC_Launcher.ViewModels
{
    public class UserManagementViewModel : ViewModelBase
    {
        LauncherService Launcherservice = new LauncherService();
        IMessageBoxService MessageBoxService { get { return GetService<IMessageBoxService>(); } }
        public ObservableCollection<Models.User> UserInfoList { get; set; } = new ObservableCollection<Models.User>();
        public IDialogService DialogService => this.GetService<IDialogService>();

        public UserViewModel SelectedUser
        {
            get => GetValue<UserViewModel>();
            set => SetValue(value);
        } // 선택된 사용자 정보 바인딩용


        public ObservableCollection<UserViewModel> Users { get; set; } = new();



        // 사용자관리 
        public Models.User UserInfo
        {
            get => GetValue<Models.User>();
            set => SetValue(value);
        } // 사용자 정보 선택하면 다이얼로그 다시 띄워서 수정하게 하기 
        public DelegateCommand AddUserCommand { get; set; }
        public DelegateCommand UserDeleteCommand { get; set; }
        public DelegateCommand UserUpdateCommand { get; set; }

        public class UserResultContext
        {
            public Models.User? NewUser { get; set; }

        }
        private UserResultContext _context;
        public object Parameter
        {
            get => null!;
            set
            {
                if (value is UserResultContext context)
                    _context = context;
            }
        }

        public UserManagementViewModel()
        {
            // 사용자 정보 초기화
            //UserInfo = new UserInfo();
            //// 사용자 목록 초기화
            //UserInfoList = new ObservableCollection<UserInfo>();

            LoadUserList();
            AddUserCommand = new DelegateCommand(AddUser);
            UserUpdateCommand = new DelegateCommand(UpdateUser);
            UserDeleteCommand = new DelegateCommand(DeleteUser);
        }
        //추가, 삭제,변경
        public void LoadUserList()
        {
            using var context = new LauncherDbContext();

            var userList = context.Users
                .Include(u => u.Role)
                .Select(u => new UserViewModel
                {
                    UserId = u.UserId,
                    UserName = u.Name,
                    RoleName = u.Role != null ? u.Role.RoleName : "",
                    Activated = u.Activated,
                    RoleId = u.RoleId
                }).ToList();

            Users.Clear();
            foreach (var user in userList)
                Users.Add(user);
        }



        public void AddUser()
        {
           
            var targetUser = new UserViewModel();
            var context = new UserEditDialogViewModel.DialogContext
            {
                EditedUser = targetUser,
                Mode = UserEditMode.추가
            };


            var result = DialogService.ShowDialog(
                dialogCommands: null,
                documentType: "UserEditDialogView",
                title: "사용자 추가"
                , viewModel: null,
                //, viewModel: loginViewModel,
                //parameter: resultContext,
                parameter: context,
                parentViewModel: null
            );
            if (context != null)

            {

            }
            else
            {
                // 취소했을때 
                //DialogService.Close();
            }
            LoadUserList();

        }

        public void DeleteUser()
        {
            if (SelectedUser == null)
            {
                //messageser
                return; // 선택된 사용자 정보가 없으면 종료
            }
            var context = new UserEditDialogViewModel.DialogContext
            {
                EditedUser = this.SelectedUser, // 
                //userviewmodel? 
               Mode = UserEditMode.삭제
            };
            var result = DialogService.ShowDialog(
                 dialogCommands: null,
                 documentType: "UserEditDialogView",
                 title: "변경"
                 , viewModel: null,
                 //, viewModel: loginViewModel,
                 //parameter: resultContext,
                 parameter: context,

                 parentViewModel: null


             );
            LoadUserList();
        }

        public void UpdateUser()
        {
            if (SelectedUser == null)
            {
                //messageser
                return; // 선택된 사용자 정보가 없으면 종료
            }

            // 업데이트는 하는게 똑같음 

            //var targetUser = new UserInfo();
            var context = new UserEditDialogViewModel.DialogContext
            {
                EditedUser = this.SelectedUser, // 
                //userviewmodel? 
                 Mode = UserEditMode.변경
            };


            var result = DialogService.ShowDialog(
                 dialogCommands: null,
                 documentType: "UserEditDialogView",
                 title: "삭제"
                 , viewModel: null,
                 //, viewModel: loginViewModel,
                 //parameter: resultContext,
                 parameter: context,

                 parentViewModel: null


             );
            LoadUserList();

        }



    }
}
