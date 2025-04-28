using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCC_Launcher.Models
{
    public class ProgramRegistrationViewModel //등록용 
    {
        public string ProgramName { get; set; }
        public string Description { get; set; }
        //public string IconPath { get; set; } // 로컬 아이콘 경로
        public string SmbSourcePath { get; set; } // 버전 폴더 경로

        public List<ProgramVersionRegistrationViewModel> Versions { get; set; } = new();
    }

    public class ProgramVersionRegistrationViewModel
    {
        public string VersionName { get; set; }
        public string LocalFolderPath { get; set; } // 스토리지로 복사할 폴더
        public string MainExecutable { get; set; }
        public string PatchNote { get; set; }
    }
}
