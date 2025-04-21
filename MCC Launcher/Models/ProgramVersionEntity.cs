using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCC_Launcher.Models
{
    public class ProgramVersionEntity
    {
        public int VersionId { get; set; }              // PK
        public int ProgramCode { get; set; }            // FK to Program
        public string VersionName { get; set; }

        public string InstallPath { get; set; } // 설치 경로
        public string MainExecutablePath { get; set; } // 실행파일명
        public string SmbSourcePath { get; set; } // 설치원본 (SMB 경로)

        public ProgramsEntity Program { get; set; }
    }
}
