using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCC_Launcher.Models
{
    public class RoleProgramVersionPermission
    {
        public int RoleId { get; set; }
        public int ProgramVersionId { get; set; }
        public int PermissionId { get; set; }

        // Navigation properties (비가상, 명시적 로딩 전제)
        public Role Role { get; set; }
        public ProgramVersionEntity ProgramVersion { get; set; }
        public Permission Permission { get; set; }
    }

}
