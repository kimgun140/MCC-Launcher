using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCC_Launcher.Models
{
    public class UserPermissionInfo
    {
        public HashSet<(int ProgramId, int PermissionId)> Permissions { get; set; } = new();

    }
}
