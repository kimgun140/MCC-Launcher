using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCC_Launcher.Models
{
    public class RoleProgramPermission
    {
        // 역할, 프로그램, 권한을 연결하는 테이블 
        // 역할과 프로그램 사이를 연결하는 중간 테이블 
        // 동일 역할이어도 프로그램별 권한을 세분화할 수 있음 

        public int RoleId { get; set; }
        public Role Role { get; set; }

        public int ProgramCode { get; set; }
        public ProgramsEntity Program { get; set; }

        public int PermissionId { get; set; }
        public Permission Permission { get; set; }


    }
}
