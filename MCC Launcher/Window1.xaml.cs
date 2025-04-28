using MCC_Launcher.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public Window1()
        {
            InitializeComponent();
            this.DataContext = this;
            LoadProgramList();
        }
        // 프로그램 리스트 불러오기
        public List<ProgramEntity> LoadProgramsFromDatabase()
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
        public void LoadProgramList()//필터링
        {
            Programs.Clear();

            var programList = LoadProgramsFromDatabase();

            foreach (var program in programList)
            {
                var firstVersion = program.Versions.FirstOrDefault();
                if (firstVersion == null)
                    continue;

                // 🔥 allowed 필터링 (PermissionId 1=Install, 2=Run 라고 가정)
                bool hasPermission = LoggedInUserPermissions.Permissions
                    .Any(p => p.ProgramId == program.ProgramId && (p.PermissionId == 1 || p.PermissionId == 2));

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
