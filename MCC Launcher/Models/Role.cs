using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCC_Launcher.Models
{
    public class Role
    {


        public int RoleId { get; set; }                        // PK
        public string RoleName { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }           // User ↔ Role
        public ICollection<RolePermission> RolePermissions { get; set; } // Role ↔ Permission

        public ICollection<RoleProgramPermission> RoleProgramPermissions { get; set; }
    }

    public class Permission
    {
        public int PermissionID { get; set; }                  // PK
        public string PermissionName { get; set; }

        public ICollection<RolePermission> RolePermissions { get; set; } // Role ↔ Permission
        public ICollection<RoleProgramPermission> RoleProgramPermissions { get; set; }
    }

    public class RolePermission
    {
        public int RoleId { get; set; }            // FK
        public Role Role { get; set; }

        public int PermissionID { get; set; }      // FK
        public Permission Permission { get; set; }
    }

    public class UserRole
    {
        public string UserId { get; set; }         // FK
        public UserInfo User { get; set; }

        public int RoleId { get; set; }            // FK
        public Role Role { get; set; }


    }
}