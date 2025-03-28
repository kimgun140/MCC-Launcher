using DevExpress.Mvvm;
using MCC_Launcher.Utilities;
using MCC_Launcher.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Documents;
using System.Collections.Generic;
using System.IO;

namespace MCC_Launcher.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly LauncherService fileCopyManager = new LauncherService();
        public ObservableCollection<Program> Programs { get; set; } = new ObservableCollection<Program>();

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

            }

            //set => SetValue(value, changedCallback: InstallOrRun);



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

        public DelegateCommand<object> SelectedProgramCommand { get; set; }

        public DelegateCommand LaunchProgramCommand { get; set; }
        public DelegateCommand DeleteProgramCommand { get; set; }
        public DelegateCommand RepairProgramCommand { get; set; }
        public DelegateCommand backupCommand { get; set; }

        public DelegateCommand OptionsImportCommand { get; set; }

        public DelegateCommand OptionCompatibleCommand { get; set; }
        //public ICommand<object> SelectedProgramCommand { get; private set; }


        public MainViewModel()
        {
            fileCopyManager.Connection();
            LoadPrograms();
            //SelectedProgramCommand = new DelegateCommand<object>(SetSeletedProgram);
            SelectedProgramCommand = new DelegateCommand<object>(SetSeletedProgram);
            LaunchProgramCommand = new DelegateCommand(LaunchSelectedVersion);
            DeleteProgramCommand = new DelegateCommand(DeleteProgram);
            RepairProgramCommand = new DelegateCommand(RepairProgram);
            backupCommand = new DelegateCommand(settingsbackup);
            OptionsImportCommand = new DelegateCommand(ImportOptions);
            OptionCompatibleCommand = new DelegateCommand(howan);

        }
        private void SetSeletedProgram(object param)
        {
            if (param is Program seleted)
            {
                SelectedProgram = seleted;
            }
        }

        private async void LaunchSelectedVersion()
        // 이거는 비동기 커맨드로 ? 
        {
            try
            {
                if (SelectedVersion == null)
                    return;

                // 설치 여부 확인 
                bool isInstalled = fileCopyManager.IsProgramInstalled(SelectedProgram.FolderPath, SelectedVersion.Path);

                if (isInstalled)
                {
                    MessageBox.Show("이미 설치된 프로그램입니다. 실행");
                    //fileCopyManager.RunProgram(SelectedProgram.FolderPath, SelectedVersion.Path);

                    return;
                }
                else
                {
                    MessageBox.Show("설치합니다");
                    //설치하기 
                    var progress = new Progress<int>(value => ProgressBarValue = value);
                    await fileCopyManager.InstallProgram(progress, SelectedVersion.Path, SelectedProgram.FolderPath, SetProgressBarVisibility);

                    InstallOrRun(); // UI 버튼 상태 업데이트
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }



        private void SetProgressBarVisibility(bool isVisible)
        {
            ProgressBarVisibility = isVisible;
        }

        private void LoadPrograms()
        {

            Programs = fileCopyManager.LoadPrograms();

            if (Programs.Count == 0)
            {
                //Task.Delay(10000);
                Programs = fileCopyManager.LoadPrograms();
            }
            InstallOrRun();


        }
        private void InstallOrRun()
        {
            if (SelectedVersion == null)
            {
                // SelectedVersion이 null인 경우 처리 처음 선택되지않은 순간
                ButtonContent = "설치";
                return;
            }
            bool isInstalled = fileCopyManager.IsProgramInstalled(SelectedProgram.FolderPath, SelectedVersion.Path);
            SelectedVersion = fileCopyManager.LoadMetaData(SelectedVersion.Path, SelectedVersion);
            //PatchNote = SelectedVersion.PatchNotes[0].ToString();
            PatchNote = SelectedVersion.PatchNote;

            //패치노트 한개뿐  , 파라미터로 들어간 SelectedVersion에서 PatchNotes[0]만 바뀜 
            // patchnotes
            Flag = isInstalled;
            ButtonContent = Flag ? "실행" : "설치";

        }
        public void DeleteProgram()
        // 삭제
        {
            if (SelectedProgram == null)
                return;
            MessageBox.Show("프로그램 삭제");

            string fullInstallPath = fileCopyManager.GetInstallPath(SelectedProgram.FolderPath, SelectedVersion.Path);
            //프로그램 이름, 버전이름으로 설치된 폴더 경로 만들기 
            MessageBox.Show("삭제완료");
            fileCopyManager.deleteDirectory(fullInstallPath);
            InstallOrRun();

        }
        private void RepairProgram()
        {
            //선택된 프로그램 버전을 삭제하고 다시 설치
            DeleteProgram();
            //
            LaunchSelectedVersion();
        }
        private void settingsbackup()
        {
            //
            MessageBox.Show("설정 백업");
            fileCopyManager.OptionExport(SelectedProgram.FolderPath, SelectedVersion.Path);
        }

        private void ImportOptions()
        {
            fileCopyManager.OptionsImport(SelectedProgram.FolderPath, SelectedVersion.Path);
            // 선택한 버전의 백업옵션을 현재 폴더로 옮기기  같은 버전의 백업만 찾아서 옮길 수 있음 
        }

        private void howan()
        {
            string installPath = fileCopyManager.GetInstallPath(SelectedProgram.FolderPath, SelectedVersion.Path);
            // 사용자 옵션 파일 위치 폴더 
            List<OptionDefinition> OptionDefinition = new List<OptionDefinition>();
            OptionDefinition = fileCopyManager.LoadCompatibility(SelectedProgram.FolderPath, SelectedVersion.Path);
            //호환성 리스트 파일 
            UserOption userOptions = new UserOption();
            userOptions = fileCopyManager.LoadUserOption(installPath);
            var result = new Dictionary<string, string>();
            string fileversionname = Path.GetFileName(SelectedVersion.Path);
            result = fileCopyManager.ConvertUserOption(userOptions, OptionDefinition, fileversionname);
            // 버전 
            fileCopyManager.SaveUpdatedUserOption(installPath, SelectedProgram.ProgramName, fileversionname, result);
            //세이브 되는 경로만 다른 버전으로 바꿔주면 해당버전으로 마이그레이션된 옵션파일 생성됨 
          
            // 어떤 버전으로 할건지 선택만 하게 하면된다. 
        }


    }
}

