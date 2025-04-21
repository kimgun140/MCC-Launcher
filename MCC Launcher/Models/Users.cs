using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCC_Launcher.Models
{
    public class UserInfo
    {



        public string UserId { get; set; }                     // PK
        public string Name { get; set; }
        public string Password { get; set; }
        public bool Activated { get; set; }
        [NotMapped] // EF Core가 DB 컬럼으로 매핑하지 않게 함
        public string RoleName => UserRoles?.FirstOrDefault()?.Role?.RoleName ?? string.Empty;

        public ICollection<UserRole> UserRoles { get; set; }   // User ↔ Role (다대다)
    }
}
