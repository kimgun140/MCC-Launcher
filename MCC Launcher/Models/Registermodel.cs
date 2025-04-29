using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCC_Launcher.Models
{
    //public class ProgramRegistrationViewModel //등록용 
    //{
    //    public string ProgramName { get; set; }
    //    public string Description { get; set; }
    //    //public string IconPath { get; set; } // 로컬 아이콘 경로
    //    public string SmbSourcePath { get; set; } // 버전 폴더 경로

    //    public List<ProgramVersionRegistrationViewModel> Versions { get; set; } = new();
    //}
    public class ProgramRegistrationViewModel
    {
        public string ProgramName { get; set; }
        public string Description { get; set; }
        public string IconFileName { get; set; } // ex) "icon.png"
        public string SmbSourcePath { get; set; } // ex) \\Gms-mcc-nas01\audio-file\test1\programs\AudioServer
        public string InstallPath { get; set; } // ex) C:\Program Files\AudioServer
        public string MainExecutable { get; set; } // ex) AudioServer.exe
        public string PatchNote { get; set; } // ex) "초기 배포 버전입니다."
        public string VersionName { get; set; } = "v1.0.0"; // 기본 버전
    }
    public class ProgramVersionRegistrationViewModel
    {
        public string VersionName { get; set; }
        public string LocalFolderPath { get; set; } // 스토리지로 복사할 폴더
        public string MainExecutable { get; set; }
        public string PatchNote { get; set; }
    }
}
