using MCC_Launcher.Models;
using MCC_Launcher.Services;
using MCC_Launcher.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MCC_Launcher
{
    /// <summary>
    /// Window1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Window1 : Window 
    {
        public ObservableCollection<ProgramDisplayModel> Programs { get; set; } = new ObservableCollection<ProgramDisplayModel>();
        public UserPermissionInfo LoggedInUserPermissions { get; set; } // 로그인 시 세팅

        public User LoggedInUser { get; set; }

        //public Program SelectedProgram
        //{
        //    get => GetValue<Program>();

        //    set => SetValue(value);

        //}


        //public User LoggedInUsertest
        //{
        //    get => GetValue<User>();
        //    set
        //    {
        //        SetValue(value);
        //        //IsAdmin();
        //        //RaisePropertiesChanged(nameof(IsAdminUser));
        //    }

        //}

        public Window1()
        {
            InitializeComponent();
            this.DataContext = this;
            LoggedInUser = new User {  UserId ="anonymous", RoleId = 13 }; // 예시로 사용자 정보 설정


            LoggedInUserPermissions = LoadUserPermissions(LoggedInUser);

            LoadProgramList();
        }
        // 프로그램 리스트 불러오기
        public List<ProgramEntity> LoadProgramsFromDatabase()//목록 불러오기 
        {
            using var context = new LauncherDbContext();

            return context.Programs
                .Include(p => p.Versions)
                .ToList();
        }

        // ViewModel에서 사용할 예시

        //public void LoadProgramList() //프로그램 목록 표시 
        //{
        //    Programs.Clear();

        //    var programList = LoadProgramsFromDatabase();

        //    foreach (var program in programList)
        //    {
        //        Programs.Add(new ProgramDisplayModel
        //        {
        //            ProgramName = program.ProgramName,
        //            Description = program.Description,
        //            SmbSourcePath = program.SmbSourcePath,
        //            //IconPath = program.IconPath
        //        });
        //    }
        //}
        public void LoadProgramList()//프로그램 필터링 권한 기준  기존 프로그램목록표시에서 변경중 
        {
            Programs.Clear();

            var programList = LoadProgramsFromDatabase();

            foreach (var program in programList)
            {
                var firstVersion = program.Versions.FirstOrDefault();
                if (firstVersion == null)
                    continue;

                // allowed 필터링 (PermissionId 1=Install, 2=Run 라고 가정)
                bool hasPermission = LoggedInUserPermissions.Permissions// 이게 0이네 
                    .Any(p => p.ProgramId == program.ProgramId && (p.PermissionId == 1 || p.PermissionId == 2));
                //테스트라서 1이나 2로만 하는중임 , 그냥 있는지없는지를 확인해야겠다 .
                // 등록된 프로그램에 아무 권한이나 있으면 목록을 표시해야지 

                if (!hasPermission)
                    continue; // 권한 없으면 추가 안함

                Programs.Add(new ProgramDisplayModel
                {
                    ProgramName = program.ProgramName,
                    Description = program.Description,
                    SmbSourcePath = firstVersion.SmbSourcePath
                });
            }
        }
        public UserPermissionInfo LoadUserPermissions(User user)
        {
            using var db = new LauncherDbContext();

            var permissions = db.RoleProgramPermissions
                .Where(rpp => rpp.RoleId == user.RoleId)
                .Select(rpp => new { rpp.ProgramId, rpp.PermissionId })
                .ToList();

            var info = new UserPermissionInfo();
            foreach (var p in permissions)
            {
                info.Permissions.Add((p.ProgramId, p.PermissionId));
            }

            return info;
        }
        
        public bool InstallProgram(ProgramEntity program, ProgramVersionEntity version)
        {
            RegisterService registerService = new RegisterService();
            try
            {
                if (program == null || version == null)
                    return false;

                string sourceFolder = version.SmbSourcePath; // SMB 원본 경로
                string destinationFolder = version.InstallPath; // 로컬 설치 경로

                if (!Directory.Exists(sourceFolder))
                {
                    MessageBox.Show($"스토리지 경로를 찾을 수 없습니다: {sourceFolder}");
                    return false;
                }

                if (!Directory.Exists(destinationFolder))
                {
                    Directory.CreateDirectory(destinationFolder);
                }

                registerService.CopyFolder(sourceFolder, destinationFolder);

                MessageBox.Show($"{program.ProgramName} {version.VersionName} 설치 완료!");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"설치 중 오류 발생: {ex.Message}");
                return false;
            }
        }



            //public void LoadProgramList()
            //{
            //    Programs.Clear();

            //    var programList = LoadProgramsFromDatabase();

            //    foreach (var program in programList)
            //    {
            //        var firstVersion = program.Versions.FirstOrDefault(); // 첫 번째 버전 기준

            //        if (firstVersion != null)
            //        {
            //            Programs.Add(new ProgramDisplayModel
            //            {
            //                ProgramName = program.ProgramName,
            //                Description = program.Description,
            //                SmbSourcePath = firstVersion.SmbSourcePath
            //            });
            //        }
            //    }
            //}
        }
}
