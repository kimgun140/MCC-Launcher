using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCC_Launcher.Models
{
    public class ProgramVersionEntity
    {
        public int VersionId { get; set; }

        public int ProgramCode { get; set; }
        public ProgramsEntity Program { get; set; }

        public string VersionName { get; set; }            // 버전 이름 (v1.0.1 등) 예정
        public string SmbSourcePath { get; set; }          // NAS 경로 (원본) 예정
        public string InstallPath { get; set; }            // 설치 위치 (로컬) 예정
        public string MainExecutable { get; set; }         // 실행 파일 이름 예정
        public string PatchNote { get; set; }              // 버전별 패치노트 예정
    }
}
