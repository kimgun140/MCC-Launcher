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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.Drawing;

//using System.Windows.Forms;

namespace MCC_Launcher.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly LauncherService fileCopyManager = new LauncherService();
        public ObservableCollection<Program> Programs { get; set; } = new ObservableCollection<Program>();




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
                PatchNotes = null;
                RaisePropertiesChanged(nameof(PatchNotes));
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
        //public bool IsInstalled
        //{
        //    get => GetValue<bool>();
        //    set => SetValue(value);
        //}
        IMessageBoxService MessageBoxService { get { return GetService<IMessageBoxService>(); } }




        public DelegateCommand<object> SelectedProgramCommand { get; set; }
        public DelegateCommand LaunchProgramCommand { get; set; }
        public DelegateCommand DeleteProgramCommand { get; set; }
        public DelegateCommand RepairProgramCommand { get; set; }
        public DelegateCommand backupCommand { get; set; }
        public DelegateCommand OptionsImportCommand { get; set; }
        public DelegateCommand OptionCompatibleCommand { get; set; }
        public DelegateCommand LoadPatchNotesCommand { get; set; }

        public DelegateCommand LoadVersionsCommand { get; set; }



        public MainViewModel()
        {
            fileCopyManager.Connection();
            LoadPrograms();
            SelectedProgramCommand = new DelegateCommand<object>(SetSeletedProgram);
            LaunchProgramCommand = new DelegateCommand(LaunchSelectedVersion);
            DeleteProgramCommand = new DelegateCommand(DeleteProgram);
            RepairProgramCommand = new DelegateCommand(RepairProgram);
            backupCommand = new DelegateCommand(OptionExport);
            OptionsImportCommand = new DelegateCommand(OptionImport);
            LoadVersionsCommand = new DelegateCommand(LoadVersions);

            //LoadPatchNotesCommand = new DelegateCommand(AllPatchNotes);

            //메시지 박스 

        }
        private void SetSeletedProgram(object param)
        {
            if (param is Program seleted)
            {
                SelectedProgram = seleted;

            }
            PatchNote = null;
            RaisePropertiesChanged(nameof(PatchNote));
            AllPatchNotes();

        }

        private async void LaunchSelectedVersion()
        //
        {
            try
            {
                if (SelectedVersion == null)
                    return;

                // 설치 여부 확인 
                bool isInstalled = fileCopyManager.IsProgramInstalled(SelectedProgram.FolderPath, SelectedVersion.Path);
                string installPath = fileCopyManager.GetInstalledversionPath(SelectedProgram.FolderPath, SelectedVersion.Path);


                if (isInstalled)
                {
                    var result = MessageBoxService.Show(
                         messageBoxText: $"{SelectedProgram.ProgramName}프로그램을 실행합니다.",
                         caption: "실행",
                         button: MessageBoxButton.OK,
                         MessageBoxImage.Question);


                    var versionname = Path.GetFileName(SelectedVersion.Path);
                    //설치된 프로그램 경로 가져오기 
                    var FolderPath = fileCopyManager.GetInstalledProgramFolderPath(SelectedProgram.FolderPath);

                    fileCopyManager.SaveLastUsedVersion(FolderPath, versionname);

                    //실행 
                    fileCopyManager.RunProgram(SelectedProgram.FolderPath, SelectedVersion.Path);


                    return;
                }
                else
                {
                    var result = MessageBoxService.Show(
                         messageBoxText: $"{SelectedProgram.ProgramName}프로그램을 설치하시겠습니까?.",
                         caption: "설치",
                         button: MessageBoxButton.YesNo,
                         MessageBoxImage.Question);
                    //MessageBox.Show("설치합니다");
                    if (result == MessageBoxResult.Yes)
                    {
                        //설치하기 
                        var progress = new Progress<int>(value => ProgressBarValue = value);
                        await fileCopyManager.InstallProgram(progress, SelectedVersion.Path, SelectedProgram.FolderPath, SetProgressBarVisibility);

                        InstallOrRun(); // UI 버튼 업데이트
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }



        private void SetProgressBarVisibility(bool isVisible)
        //값 바꿔주기 
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
        //버전선택할때 동작 
        {
            if (SelectedVersion == null)
            {
                // SelectedVersion이 null인 경우 처리 처음 선택되지않은 순간
                ButtonContent = "설치";
                return;
            }

            bool isInstalled = fileCopyManager.IsProgramInstalled(SelectedProgram.FolderPath, SelectedVersion.Path);
            SelectedVersion = fileCopyManager.LoadMetaData(SelectedVersion.Path, SelectedVersion);

            PatchNote = SelectedVersion.PatchNote;

            Flag = isInstalled;
            ButtonContent = Flag ? "실행" : "설치";

        }

        private void AllPatchNotes()
        {

            if (SelectedProgram == null)
                return;

            var versioninfo = fileCopyManager.AllPatchNotes(SelectedProgram.FolderPath);
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
                string fullInstallPath = fileCopyManager.GetInstalledversionPath(SelectedProgram.FolderPath, SelectedVersion.Path);
                //프로그램 이름, 버전이름으로 설치된 폴더 경로 만들기 
                fileCopyManager.deleteDirectory(fullInstallPath);
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
                fileCopyManager.OptionExport(SelectedProgram.FolderPath, SelectedVersion.Path);

            }


        }



        //private void howan()
        //{
        //    string installPath = fileCopyManager.GetInstalledversionPath(SelectedProgram.FolderPath, SelectedVersion.Path);
        //    // 사용자 옵션 파일 위치 폴더 
        //    List<OptionDefinition> OptionDefinition = new List<OptionDefinition>();
        //    OptionDefinition = fileCopyManager.LoadCompatibility(SelectedProgram.FolderPath, SelectedVersion.Path);
        //    //호환성 리스트 파일 
        //    UserOption userOptions = new UserOption();
        //    userOptions = fileCopyManager.LoadUserOption(installPath);
        //    var result = new Dictionary<string, string>();
        //    string fileversionname = Path.GetFileName(SelectedVersion.Path);
        //    result = fileCopyManager.ConvertUserOption(userOptions, OptionDefinition, fileversionname);
        //    // 버전 
        //    fileCopyManager.SaveUpdatedUserOption(installPath, SelectedProgram.ProgramName, fileversionname, result);
        //    //세이브 되는 경로만 다른 버전으로 바꿔주면 해당버전으로 마이그레이션된 옵션파일 생성됨 

        //    // 어떤 버전으로 할건지 선택만 하게 하면된다. 
        //}

        private void OptionImport()
        {//호환성체크 
            //설치 프로그램 폴더
            string installedPath = fileCopyManager.GetInstalledProgramFolderPath(SelectedProgram.FolderPath);
            // 버전 선택안하면 에러
            string fileversionname = Path.GetFileName(SelectedVersion.Path);

            var record = fileCopyManager.LatestRunVersionRecord(SelectedProgram.FolderPath, SelectedVersion.Path);
            string installPath = fileCopyManager.GetInstalledversionPath(SelectedProgram.FolderPath, SelectedVersion.Path);
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
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var defs = fileCopyManager.LoadCompatibility(SelectedProgram.FolderPath, SelectedVersion.Path);
                    var newOption = fileCopyManager.ConvertUserOption(backupOption, defs, fileversionname);
                    fileCopyManager.SaveUpdatedUserOption(installPath, SelectedProgram.ProgramName, fileversionname, newOption);

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
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    var defs = fileCopyManager.LoadCompatibility(SelectedProgram.FolderPath, SelectedVersion.Path);
                    var newOption = fileCopyManager.ConvertUserOption(backupOption, defs, fileversionname);
                    fileCopyManager.SaveUpdatedUserOption(installPath, SelectedProgram.ProgramName, fileversionname, newOption);
                }
            }
        }

        private void LoadVersions()
        {

            var reuslt = fileCopyManager.LoadCompAbilityList(SelectedProgram.FolderPath);
            var path = fileCopyManager.GetInstalledversionPath(SelectedProgram.FolderPath, SelectedVersion.Path);
            var CurrentOption = fileCopyManager.LoadUserOption2(path);

            var versionpath = fileCopyManager.GetInstalledversionPath(SelectedProgram.FolderPath, SelectedVersion.Path);
            var TargetVersion = Path.GetFileName(versionpath);
            // 현재는 선택된 버전이지만 들어가있는 파일이 다른버전이라서 가능하기는 함 바뀌는것만 확인 나중에 파일 다시 쓰기 

            fileCopyManager.ConvertOptions(CurrentOption, reuslt, TargetVersion);
        }
    }
}

