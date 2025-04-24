using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCC_Launcher.Models
{
    public class UserProgramPeriod
    {
        public string UserId { get; set; }
        public int ProgramCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public  UserInfo User { get; set; }
        public  ProgramsEntity Program { get; set; }
    }
}
