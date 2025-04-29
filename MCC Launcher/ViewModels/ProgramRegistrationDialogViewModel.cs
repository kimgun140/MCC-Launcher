using DevExpress.Mvvm;
using MCC_Launcher.Models;
using MCC_Launcher.Services;
using Microsoft.EntityFrameworkCore.Internal;

namespace MCC_Launcher.ViewModels
{
    public class ProgramRegistrationDialogViewModel : ViewModelBase
    {
        public string ProgramName
        {
            get => GetValue<string>();
            set => SetValue(value);

        }
        public string Description
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string IconFileName
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        public string SmbSourcePath
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        public string InstallPath
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        public string MainExecutable
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        public string PatchNote
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        public string VersionName
        {
            get => GetValue<string>();
            set => SetValue(value);
        }



        ProgramRegistrationViewModel programRegistration
        {
            get => GetValue<ProgramRegistrationViewModel>();
            set => SetValue(value);
        }
        //programRegistration = new ProgramRegistrationViewModel();

        public RegisterService RegisteredServices = new RegisterService();

        public DelegateCommand RegisterCommand { get; set; }
        public DelegateCommand UploadCommand { get; set; }
        //public DelegateCommand
        public ProgramRegistrationDialogViewModel()
        {
            RegisterCommand = new DelegateCommand(RegisterProgram);
            UploadCommand = new DelegateCommand(UploadProgram);
            //programRegistration.ProgramName
        }
        public string LocalFolderPath
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public void RegisterProgram()
        {
            var programRegistration = new ProgramRegistrationViewModel
            {
                ProgramName = ProgramName,
                Description = Description,
                IconFileName = IconFileName,
                SmbSourcePath = SmbSourcePath,
                InstallPath = InstallPath,
                MainExecutable = MainExecutable,
                PatchNote = PatchNote,
                VersionName = VersionName
            };

            RegisteredServices.RegisterProgram(programRegistration, LocalFolderPath);


            // 프로그램 등록 로직 구현
            // 예: 데이터베이스에 프로그램 정보 저장
            // MessageBoxService.ShowMessage($"프로그램 '{ProgramName}' 등록 완료!", "등록 완료", MessageButton.OK, MessageIcon.Information);
        }
        public void UploadProgram()
        {

            using (var folderDialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                folderDialog.Description = "업로드할 폴더를 선택하세요.";
                folderDialog.ShowNewFolderButton = false;
                if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    LocalFolderPath = folderDialog.SelectedPath;
                }
            }
            //    openFolderDialogService.Title = "폴더 선택";
            //openFolderDialogService.ShowDialog();
            // 업로드 로직 구현
            // 예: 파일을 서버에 업로드
            // MessageBoxService.ShowMessage($"프로그램 '{ProgramName}' 업로드 완료!", "업로드 완료", MessageButton.OK, MessageIcon.Information);
        }

    }
}
