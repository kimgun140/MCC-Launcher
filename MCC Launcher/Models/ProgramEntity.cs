using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MCC_Launcher.Models
{
    public class ProgramEntity
    {
        public int ProgramId { get; set; }
        public string ProgramName { get; set; }

        public string Description { get; set; }     // 프로그램 설명
        public string IconPath { get; set; }        // 아이콘 파일 경로
        public string SmbSourcePath { get; set; }


        public ICollection<ProgramVersionEntity> Versions { get; set; }
        public ICollection<RoleProgramPermission> RoleProgramPermissions { get; set; }

        //public bool AllowAnonymousRun { get; set; }
        //public bool AllowAnonymousInstall { get; set; }



        //public int ProgramCode { get; set; }  // 고유 ID
        //public string Name { get; set; }
        //public string Description { get; set; }
        //public string IconPath { get; set; }

        //public virtual ICollection<ProgramVersionEntity> Versions { get; set; }

        //public int ProgramId { get; set; }
        //public string ProgramName { get; set; }

        //public string Description { get; set; }
        //public string SmbSourcePath { get; set; }

        //public ICollection<RoleProgramPermission> RoleProgramPermissions { get; set; }
        //public ICollection<ProgramVersionEntity> Versions { get; set; }

    }
    public class ProgramDisplayModel
    {
        public string ProgramName { get; set; }
        public string Description { get; set; }
        public string IconPath { get; set; }
        public string SmbSourcePath { get; set; } // 버전 폴더 경로

        //public ImageSource IconSource
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(IconPath))
        //            return null;

        //        try
        //        {
        //            return new BitmapImage(new Uri(IconPath, UriKind.Absolute));
        //        }
        //        catch
        //        {
        //            return null;
        //        }
        //    }
        //}
        public ImageSource IconSource
            // 파일 이름을 그대로 불러와서 붙여주기만 하면 될듯 / db에 저장을 파일이름.확장자까지 포함할 때 면 가능 할듯 
        {
            get
            {
                if (string.IsNullOrEmpty(SmbSourcePath))
                    return null;

                try
                {
                    var iconFullPath = System.IO.Path.Combine(SmbSourcePath, "icon.png");
                    return new BitmapImage(new Uri(iconFullPath, UriKind.Absolute));
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
