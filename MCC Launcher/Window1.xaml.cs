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

        public void LoadProgramList()
        {
            Programs.Clear();

            var programList = LoadProgramsFromDatabase();

            foreach (var program in programList)
            {
                Programs.Add(new ProgramDisplayModel
                {
                    ProgramName = program.ProgramName,
                    Description = program.Description,
                    IconPath = program.IconPath
                });
            }
        }
    }
}
