using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCC_Launcher.Models
{
    public class ProgramsEntity
    {
        public int ProgramCode { get; set; }            // PK
        public string Name { get; set; }
        public bool AllowAnonymousRun { get; set; }
        public bool AllowAnonymousInstall { get; set; }
        public ICollection<ProgramVersionEntity> Versions { get; set; }
        public ICollection<RoleProgramPermission> RoleProgramPermissions { get; set; }
    }
}
