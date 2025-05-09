﻿using DevExpress.Mvvm;
using MCC_Launcher.Utilities;
using MCC_Launcher.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Documents;
using System.Collections.Generic;
using System.IO;

using System.Drawing;
using Microsoft.Win32;
using Microsoft.Xaml.Behaviors;
using DevExpress.Mvvm.UI;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using MCC_Launcher.Models;
using DevExpress.Xpf.Editors;
using MCC_Launcher.Views;
using DevExpress.Mvvm.Xpf;
using System.Linq;
using DevExpress.Xpf.Core;
using Microsoft.EntityFrameworkCore.Query.Internal;



namespace MCC_Launcher.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly LauncherService launcherService = new LauncherService();
        public IDialogService LoginDialogService => this.GetService<IDialogService>("LoginDialog");
        public IDialogService RolePermissionManagementDialogService => this.GetService<IDialogService>("RolePermissionManagementDialog");
        public IDialogService UserManagementDialogService => this.GetService<IDialogService>("UserManagementDialog");


        protected ICurrentDialogService CurrentDialogService { get { return GetService<ICurrentDialogService>(); } }


        public ObservableCollection<Program> Programs { get; set; } = new ObservableCollection<Program>();

        // lggedInUser = new UserInfo(); 이걸로 확인하면 될듯? 
        public bool IsAdminUser
        {
            // 다이얼로그 열릴때 새걸로 들어가서 null
            get => GetValue<bool>();
            set => SetValue(value);
        }


        public UserInfo LoggedInUser
        {
            get => GetValue<UserInfo>();
            set
            {
                SetValue(value);
                IsAdmin();
                //RaisePropertiesChanged(nameof(IsAdminUser));


            }

        }





        //protected IDispatcherService DispatcherService { get { return this.GetService<IDispatcherService>(); } }
        //하 이거이거 

        //public void SomeMethod()
        //{
        //    DispatcherService?.BeginInvoke(() =>
        //    {
        //        ShouldResetScroll = true;
        //        //RaisePropertiesChanged(nameof(ShouldResetScroll));


        //    });
        //}
        public bool ShouldResetScroll
        {
            get => GetValue<bool>();
            set => SetValue(value);
        }

        public string SelectedOption
        // 선택된 옵션 이름 보여주기 
        {
            get => GetValue<string>();
            set => SetValue(SelectedOption, value);
        }
        public bool ProgressBarVisibility
        {
            get => GetValue<bool>();
            set => SetValue(value);

        }
        public int ProgressBarValue
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        public Program SelectedProgram
        {
            get => GetValue<Program>();

            set => SetValue(value);

        }
        public string PatchNote
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public VersionInfo SelectedVersion
        {
            get => GetValue<VersionInfo>();
            //set => SetValue(value, changedCallback: InstallOrRun);
            set
            {
                SetValue(value, changedCallback: InstallOrRun);

                //RaisePropertiesChanged(nameof(SelectedVersion));
                RaisePropertiesChanged(nameof(PatchNote));
                //변경된거 수동으로 알리기
                //PatchNotes = null;//전체 목록 
                //RaisePropertiesChanged(nameof(PatchNotes));
            }

        }
        public string ButtonContent
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public bool Flag
        {
            get => GetValue<bool>();
            set => SetValue(value);

        }
        public ObservableCollection<string> PatchNotes
        {
            get => GetValue<ObservableCollection<string>>();
            set => SetValue(value);
        }



        IMessageBoxService MessageBoxService { get { return GetService<IMessageBoxService>(); } }

        public DelegateCommand RolePermissionManagementCommand { get; set; }

        public DelegateCommand LogoutCommand { get; set; }
        public DelegateCommand ShowLoginCommand { get; set; }
        public DelegateCommand<object> SelectedProgramCommand { get; set; }
        public DelegateCommand LaunchProgramCommand { get; set; }
        public DelegateCommand DeleteProgramCommand { get; set; }
        public DelegateCommand RepairProgramCommand { get; set; }
        public DelegateCommand backupCommand { get; set; }
        public DelegateCommand OptionsImportCommand { get; set; }
        public DelegateCommand OptionCompatibleCommand { get; set; }
        public DelegateCommand LoadPatchNotesCommand { get; set; }

        public DelegateCommand LoadVersionsCommand { get; set; }

        public DelegateCommand CancelCommand { get; set; }
        public DelegateCommand ShowUserManagementCommand { get; set; }
        public DelegateCommand RoleManagementCommand { get; set; }
        public MainViewModel()
        {

            CreateAnonymousUser();

            IsAdmin();
            launcherService.Connection();
            LoadPrograms();
            SelectedProgramCommand = new DelegateCommand<object>(SetSeletedProgram);
            LaunchProgramCommand = new DelegateCommand(LaunchSelectedVersion);
            DeleteProgramCommand = new DelegateCommand(DeleteProgram);
            RepairProgramCommand = new DelegateCommand(RepairProgram);
            backupCommand = new DelegateCommand(OptionExport);
            OptionsImportCommand = new DelegateCommand(OptionImport);
            LoadVersionsCommand = new DelegateCommand(LoadBackupOption);
            LogoutCommand = new DelegateCommand(Logout);

            ShowLoginCommand = new DelegateCommand(OpenLoginDialog);


            CancelCommand = new DelegateCommand(Cancel);

            RolePermissionManagementCommand = new DelegateCommand(RolePermissionManagementDialog);
            ShowUserManagementCommand = new DelegateCommand(UserManagementDialog);
            RoleManagementCommand = new DelegateCommand(ShowRoleManagementDialog);




        }
        private void SetSeletedProgram(object param)
        {
            if (param is Program seleted)
            {
                SelectedProgram = seleted;

            }
            //여기에서 프로그램 코드 넣기 
            var dbProgram = launcherService.GetProgramByName(SelectedProgram.ProgramName);
            if (dbProgram != null)
            {
                SelectedProgram.ProgramCode = dbProgram.ProgramCode;
                // 필요한 경우 AllowAnonymousInstall, AllowAnonymousRun도 함께 복사
                //SelectedVersion.AllowAnonymousInstall = dbProgram.AllowAnonymousInstall;
                //SelectedVersion.AllowAnonymousRun = dbProgram.AllowAnonymousRun;
            }

            PatchNote = null;
            RaisePropertiesChanged(nameof(PatchNote));
            AllPatchNotes();
            //Messenger.Default.Send("ScrollToTop", "ScrollToTop");

        }

        private async void LaunchSelectedVersion()
        //
        {
            // 여기서 먼저 검사 여기서 통과하면 다음 진행 
            try
            {
                // 프로그램을 비교하려면 해당 프로그램 이름이랑 비교를 해야겠네 이름 가져오고 그거랑 비교하기 
                if (!LoggedInUser.Activated)
                {// actived 확인 

                    MessageBoxService.Show("이용 기간이 만료되었습니다. 이용기간을 연장해주세요 ");
                    return;
                }

                if (SelectedVersion == null)
                    return;
                //SelectedProgram.programcode가 있어야함 
                launcherService.anonymous(SelectedProgram.ProgramCode, SelectedVersion);
                //비로그인 사용가능 여부 업데이트 

                //선택된 프로그램의 정보를 db 접근해서 업데이트해야함 


                // 설치 여부 확인 
                bool isInstalled = launcherService.IsProgramInstalled(SelectedProgram.FolderPath, SelectedVersion.Path);
                string installPath = launcherService.GetInstalledversionPath(SelectedProgram.FolderPath, SelectedVersion.Path);

                bool hasExecutePermission = launcherService.HasPermission(LoggedInUser, SelectedProgram.ProgramCode, "Execute");
                bool hasInstallPermission = launcherService.HasPermission(LoggedInUser, SelectedProgram.ProgramCode, "Install");

                bool allowAnonymousExecute = SelectedVersion.AllowAnonymousRun;
                bool allowAnonymousInstall = SelectedVersion.AllowAnonymousInstall;

                bool canExecute = allowAnonymousExecute || hasExecutePermission;
                bool canInstall = allowAnonymousInstall || hasInstallPermission;

                // ✅ 실행 조건
                if (isInstalled && canExecute)
                {
                    var result = MessageBoxService.Show($"{SelectedProgram.ProgramName} 프로그램을 실행합니다.", "실행", MessageBoxButton.OK);
                    if (result == MessageBoxResult.OK)
                    {
                        var versionname = Path.GetFileName(SelectedVersion.Path);
                        var FolderPath = launcherService.GetInstalledProgramFolderPath(SelectedProgram.FolderPath);
                        launcherService.SaveLastUsedVersion(FolderPath, versionname);
                        launcherService.RunProgram(SelectedProgram.FolderPath, SelectedVersion.Path, SelectedVersion.MainExecutable);
                    }

                    return;
                }

                // ✅ 설치 조건
                if (!isInstalled && canInstall)
                {
                    var result = MessageBoxService.Show($"{SelectedProgram.ProgramName} 프로그램을 설치하시겠습니까?", "설치", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        var progress = new Progress<int>(value => ProgressBarValue = value);
                        await launcherService.InstallProgram(progress, SelectedVersion.Path, SelectedProgram.FolderPath, SetProgressBarVisibility);
                        GenerateToken();
                    }

                    return;
                }

                // ❌ 둘 다 불가능한 경우
                MessageBoxService.Show("설치 또는 실행 권한이 없습니다.", "권한 부족", MessageBoxButton.OK, MessageBoxImage.Warning);
                InstallOrRun(); // UI 버튼 업데이트

            }
            catch (Exception e)
            {
                MessageBox.Show("parmeter path2= 실행파일 이없음 ", e.Message);
            }
        }



        private void SetProgressBarVisibility(bool isVisible)
        //값 바꿔주기 
        {
            ProgressBarVisibility = isVisible;
        }

        private void LoadPrograms()
        {

            Programs = launcherService.LoadPrograms();

            if (Programs.Count == 0)
            {
                //Task.Delay(10000);
                Programs = launcherService.LoadPrograms();
            }
            InstallOrRun();
            //InsertData();

        }
        private void InstallOrRun()
        //버전선택할때 동작 
        {
            if (SelectedVersion == null)
            {
                // SelectedVersion이 null인 경우 처리 처음 선택되지않은 순간
                ButtonContent = "설치";
                return;
            }

            bool isInstalled = launcherService.IsProgramInstalled(SelectedProgram.FolderPath, SelectedVersion.Path);
            SelectedVersion = launcherService.LoadMetaData(SelectedVersion.Path, SelectedVersion);

            PatchNote = SelectedVersion.PatchNote;

            Flag = isInstalled;
            ButtonContent = Flag ? "실행" : "설치";

        }

        private void AllPatchNotes()
        {

            if (SelectedProgram == null)
                return;

            var versioninfo = launcherService.AllPatchNotes(SelectedProgram.FolderPath);
            // version에 각각 들어 있는데 이걸 각각 표시 해주는게 programs에 들어있는 것들이니까 

            foreach (var version in SelectedProgram.Versions)
            {
                string installFolder = Path.Combine(SelectedProgram.FolderPath, version.Path);
                version.isInstalled = Directory.Exists(installFolder);
            }

            PatchNotes = versioninfo.PatchNotes;

        }
        public void DeleteProgram()
        // 삭제
        {
            if (SelectedProgram == null || SelectedVersion == null)
                return;
            //MessageBox.Show("프로그램 삭제");
            var result = MessageBoxService.ShowMessage(
                         messageBoxText: $"{SelectedProgram.ProgramName}프로그램을 삭제하시겠습니까 .",
                         caption: "삭제",
                         button: MessageButton.YesNoCancel,
                         icon: MessageIcon.Question);
            if (result == MessageResult.Yes)
            {
                string fullInstallPath = launcherService.GetInstalledversionPath(SelectedProgram.FolderPath, SelectedVersion.Path);
                //프로그램 이름, 버전이름으로 설치된 폴더 경로 만들기 
                launcherService.deleteDirectory(fullInstallPath);
                InstallOrRun();
            }

        }
        private void RepairProgram()
        {
            if (SelectedProgram == null || SelectedVersion == null)
                return;
            //선택된 프로그램 버전을 삭제하고 다시 설치
            var result = MessageBoxService.ShowMessage(
                     messageBoxText: $"{SelectedProgram.ProgramName}프로그램을 재설치하시겠습니까?",
                     caption: "재설치",
                     button: MessageButton.YesNo,
                     icon: MessageIcon.Question);
            if (result == MessageResult.Yes)
            {
                //삭제, 재설치
                DeleteProgram();
                LaunchSelectedVersion();
            }
        }
        private void OptionExport()
        {
            if (SelectedProgram == null || SelectedVersion == null)
            {
                return;
            }
            //
            //MessageBox.Show("설정 백업");
            var result = MessageBoxService.Show(
                          messageBoxText: $"{SelectedProgram.ProgramName}옵션을 Export하시겠습니까?.",
                          caption: "옵션 백업",
                          button: MessageBoxButton.YesNo,
                          MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                //fileCopyManager.OptionFolderBackup(SelectedProgram.FolderPath, SelectedVersion.Path);
                launcherService.OptionExport(SelectedProgram.FolderPath, SelectedVersion.Path);


            }


        }

        private void OptionImport()
        {//호환성체크 
            //설치 프로그램 폴더
            string installedPath = launcherService.GetInstalledProgramFolderPath(SelectedProgram.FolderPath);
            // 버전 선택안하면 에러
            string fileversionname = Path.GetFileName(SelectedVersion.Path);

            var record = launcherService.LatestRunVersionRecord(SelectedProgram.FolderPath, SelectedVersion.Path);
            string installPath = launcherService.GetInstalledversionPath(SelectedProgram.FolderPath, SelectedVersion.Path);
            if (record == null)
            // 선택안했을때 null  
            {
                return;

            }
            var backupOption = record;
            string selectedVersion = Path.GetFileName(SelectedVersion.Path);

            if (backupOption.CurrentVersion != selectedVersion)
            {
                var result = MessageBoxService.Show(
                    "선택한 옵션을 적용하시겠습니까?\n기존 옵션에 덮어쓰기 됩니다.",
                    "옵션 마이그레이션",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    var defs = launcherService.LoadCompatibility(SelectedProgram.FolderPath, SelectedVersion.Path);
                    var newOption = launcherService.ConvertUserOption(backupOption, defs, fileversionname);
                    launcherService.SaveUpdatedUserOption(installPath, SelectedProgram.ProgramName, fileversionname, newOption);

                    //MessageBoxService.Show($"{selectedVersion}이 {backupOption.CurrentVersion}로 마이그레이션 됐습니다.");
                    MessageBoxService.Show($" {backupOption.CurrentVersion}로 옵션이 적용되었습니다.");

                }
            }
            else
            {
                var result = MessageBoxService.Show(
                    "선택한 버전과 동일한 버전의 옵션파일입니다. 덮어쓰기 하겠습니까?",
                    "옵션 덮어쓰기",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var defs = launcherService.LoadCompatibility(SelectedProgram.FolderPath, SelectedVersion.Path);
                    var newOption = launcherService.ConvertUserOption(backupOption, defs, fileversionname);
                    launcherService.SaveUpdatedUserOption(installPath, SelectedProgram.ProgramName, fileversionname, newOption);
                    // 버전폴더안에 위치 
                }
            }
        }



        private void LoadBackupOption()
        {//software version 불러오기

            // 1. 백업 폴더 선택 (폴더 선택 대화상자)
            string selectedBackupFolder = launcherService.ImportOptionFolder(SelectedProgram.FolderPath, SelectedVersion.Path);
            if (string.IsNullOrEmpty(selectedBackupFolder))
                return;

            // 2. 백업 옵션 불러오기 (폴더 기준으로 옵션 여러 파일을 읽음)
            var groupedOptions = launcherService.LoadUserOptionsGrouped(selectedBackupFolder);

            // 3. 현재 설치된 프로그램 경로 및 정보
            string installedPath = launcherService.GetInstalledProgramFolderPath(SelectedProgram.FolderPath);
            string targetVersion = Path.GetFileName(SelectedVersion.Path);
            string programCode = Path.GetFileName(SelectedProgram.FolderPath);
            string installVersionPath = launcherService.GetInstalledversionPath(SelectedProgram.FolderPath, SelectedVersion.Path);

            // 4. 호환성 스키마 불러오기
            var compatibilityList = launcherService.LoadCompAbilityList(SelectedProgram.FolderPath);

            // 5. 사용자에게 마이그레이션 여부 확인
            var result = MessageBoxService.Show(
                "선택한 백업 옵션을 현재 버전에 적용하시겠습니까?\n기존 옵션이 덮어쓰기 됩니다.",
                "옵션 마이그레이션",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            // 6. 마이그레이션 처리
            var migratedOptions = launcherService.ConvertOptionsGroupedByCategory(
                groupedOptions, compatibilityList, targetVersion);

            // 7. 마이그레이션된 옵션 저장
            launcherService.SaveMigratedOptionsGrouped(
                migratedOptions, installVersionPath, programCode, targetVersion);

            // 8. 완료 메시지
            MessageBoxService.Show("옵션 마이그레이션 및 저장이 완료되었습니다.");

        }

        public void InsertData()
        {
            using (var context = new LauncherDbContext())
            {
                // Roles
                var adminRole = new Role { RoleId = 1, RoleName = "Admin" };
                var userRole = new Role { RoleId = 2, RoleName = "User" };

                // Permissions
                var installPermission = new Permission { PermissionID = 1, PermissionName = "Install" };
                var executePermission = new Permission { PermissionID = 2, PermissionName = "Execute" };

                // Programs
                var programs = new List<ProgramsEntity>
        {
            new ProgramsEntity { ProgramCode = 1, Name = "AudioServer" },
            new ProgramsEntity { ProgramCode = 2, Name = "ProgramA" },
            new ProgramsEntity { ProgramCode = 3, Name = "ProgramB" },
            new ProgramsEntity { ProgramCode = 4, Name = "ProgramC" },
            new ProgramsEntity { ProgramCode = 5, Name = "ProgramD" },
            new ProgramsEntity { ProgramCode = 6, Name = "ProgramE" },
            new ProgramsEntity { ProgramCode = 7, Name = "ProgramG" },
        };

                // Program Versions
                var programVersions = new List<ProgramVersionEntity>
        {
            new ProgramVersionEntity { VersionId = 1, VersionName = "v1.0.0", ProgramCode = 1 },
            new ProgramVersionEntity { VersionId = 2, VersionName = "v1.1.0", ProgramCode = 1 },
            new ProgramVersionEntity { VersionId = 3, VersionName = "v2.0.0", ProgramCode = 2 },
        };

                // Users
                var adminUser = new UserInfo { UserId = "admin", Name = "관리자", Password = "admin123", Activated = true };
                var guestUser = new UserInfo { UserId = "guest", Name = "일반사용자", Password = "guest123", Activated = true };

                // Role Mappings
                var userRoles = new List<UserRole>
        {
            new UserRole { UserId = "admin", RoleId = 1 },
            new UserRole { UserId = "guest", RoleId = 2 },
        };

                var rolePermissions = new List<RolePermission>
        {
            new RolePermission { RoleId = 1, PermissionID = 1 }, // Admin - Install
            new RolePermission { RoleId = 1, PermissionID = 2 }, // Admin - Execute
            new RolePermission { RoleId = 2, PermissionID = 2 }, // User - Execute only
        };

                // Insert
                context.Roles.AddRange(adminRole, userRole);
                context.Permissions.AddRange(installPermission, executePermission);
                context.Programs.AddRange(programs);
                context.ProgramVersion.AddRange(programVersions);
                context.Users.AddRange(adminUser, guestUser);
                context.UserRoles.AddRange(userRoles);
                context.RolePermissions.AddRange(rolePermissions);

                context.SaveChanges();

            }
        }//더미데이터넣기


        private void OpenLoginDialog()
        {

            var resultContext = new LoginViewModel.LoginResultContext();
            resultContext.AuthenticatedUser = LoggedInUser;
            //다이얼로그가 열릴때 초기화된게 들어가서 저기에 넣으면 안됨 동작 순서를 바꾸던가, 아니면 초기값을 넣어줘야할듯

            var result = LoginDialogService.ShowDialog(

                 dialogCommands: null,

                 documentType: "LoginView",
                 title: "로그인",
                 viewModel: null,
                 //, viewModel: loginViewModel,
                 parameter: resultContext,
                 parentViewModel: null


             );
            if (resultContext != null)
            {
                Cancel();

                LoggedInUser = resultContext.AuthenticatedUser;
                // 이후 프로그램 실행/설치 등 권한 확인 가능
            }

        }
        public void GenerateToken()
        {
            //토큰만들기 
            JwtService jwtService = new JwtService();
            jwtService.GenerateToken(LoggedInUser);
        }
        public void Logout()
        {
            // 로그아웃 처리
            CreateAnonymousUser();
            MessageBoxService.Show("로그아웃 되었습니다.");
            //MessageBox.Show("로그아웃 되었습니다.", "로그아웃", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        public void Cancel()
        {
            //CurrentDialogService.Close(MessageResult.OK);

        }
        public void UserManagementDialog()
        {
            //var resultContext = new UserManagementViewModel.LoginResultContext();
            var result = UserManagementDialogService.ShowDialog(
                 dialogCommands: null,
                 documentType: "UserManagementView",
                 title: "로그인"
                 , viewModel: null,
                 //, viewModel: loginViewModel,
                 //parameter: resultContext,
                 parameter: null,

                 parentViewModel: null


             );
        }

        public void IsAdmin()
        // 관리자만 사용가능하게 
        {
            // 어드민인지 확인 
            if (LoggedInUser.UserRoles.Any(r => r.Role.RoleName == "Admin"))
            //로그아웃때 에러 
            {
                // 어드민 권한이 있음
                IsAdminUser = true;
                //return true;
            }
            else
            {
                // 어드민 권한이 없음
                IsAdminUser = false;
                //return false;
            }
        }
        public UserInfo CreateAnonymousUser()
        {
            // 비로그인 사용자로 초기화 , 프로그램 첫 로드시, 로그아웃할때 사용 

            return LoggedInUser = new UserInfo
            {
                UserId = "Anonymous",
                Activated = true,
                UserRoles = new List<UserRole>
        {
            new UserRole
            {
                Role = new Role { RoleName = "Anonymous" }
            }
        }
            };
        }

        public void RolePermissionManagementDialog()
        {
            //권한 관리 다이얼로 ㅡ


            //var resultContext = new RolePermissionManagementViewModel.DialogContext();
            //resultContext.EditedUser = LoggedInUser;
            //다이얼로그가 열릴때 초기화된게 들어가서 저기에 넣으면 안됨 동작 순서를 바꾸던가, 아니면 초기값을 넣어줘야할듯
            var result = RolePermissionManagementDialogService.ShowDialog(
                 dialogCommands: null,
                 documentType: "RolePermissionManagementView",
                 title: "권한관리",
                 viewModel: null,
                 //, viewModel: loginViewModel,
                 //parameter: resultContext,
                 parameter: null,
                 parentViewModel: null


             );

        }
        public void ShowRoleManagementDialog()
        {
            // 역할 관리 다이얼로그 열기
            var result = RolePermissionManagementDialogService.ShowDialog(
                dialogCommands: null,
                documentType: "RoleManagementView",
                title: "관리자 ",
                viewModel: null,
                parameter: null,
                parentViewModel: null
            );

        }
    }
}



