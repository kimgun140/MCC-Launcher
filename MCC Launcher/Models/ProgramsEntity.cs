using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCC_Launcher.Models
{
    public class ProgramsEntity
    {
        public int ProgramCode { get; set; }
        public string Name { get; set; }

        public string? Description { get; set; }     // 프로그램 설명
        public string? IconPath { get; set; }        // 아이콘 파일 경로

        public bool AllowAnonymousRun { get; set; }
        public bool AllowAnonymousInstall { get; set; }

        public ICollection<ProgramVersionEntity> Versions { get; set; }
        public ICollection<RoleProgramPermission> RoleProgramPermissions { get; set; }
    }
}
