using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;

namespace MCC_Launcher
{
    //[XmlRoot("Program")]
    public class Program
    {
        [XmlElement("Name")]
        public string ProgramName { get; set; }

        [XmlIgnore] // 폴더 경로는 직렬화하지 않음
        public string FolderPath { get; set; }

        [XmlElement("Description")]
        public string Description { get; set; }

        [XmlArray("Versions")]
        [XmlArrayItem("Version")]
        public ObservableCollection<VersionInfo> Versions { get; set; } = new ObservableCollection<VersionInfo>(); // 버전 목록

        [XmlElement("Icon")]
        public string IconPath { get; set; }

        [XmlIgnore] // BitmapImage는 직렬화할 수 없으므로 제외
        public BitmapImage IconSource
        {
            get
            {
                if (!string.IsNullOrEmpty(IconPath) && File.Exists(IconPath))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(IconPath);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    return bitmap;
                }
                return null;
            }
        }
        public int ProgramCode { get; set; } // 프로그램 코드
        //public string Description { get; set; } // 프로그램 설명

    }

    public class VersionInfo
    //programmetadata.xml
    {
        [XmlElement("Number")]
        public string Number { get; set; }

        [XmlElement("Path")]
        public string Path { get; set; }

        [XmlIgnore] // PatchNote는 직렬화에서 제외
        public string PatchNote { get; set; }

        [XmlIgnore] // PatchNote는 직렬화에서 제외

        public ObservableCollection<string> PatchNotes { get; set; }

        public bool isInstalled { get; set; }//사용안함
        public string MainExecutable { get; set; }//사용 안함
        public bool AllowAnonymousRun { get; set; } //
        public bool AllowAnonymousInstall { get; set; } //

    }
    // 버전별 데이터 
    [XmlRoot("ProgramMetaData")]
    public class ProgramMetaData
    {
        [XmlElement("Version")]
        public string Version { get; set; }

        [XmlElement("PatchNote")]
        public string PatchNote { get; set; }

        [XmlElement("MainExecutable")]
        public string MainExecutable { get; set; }

        public bool isInstalled { get; set; }
    }

    // 런처가 가지고 있는 런처에 등록된 프로그램들의 경로
    [XmlRoot("Launcher")]
    public class LauncherConfig
    {
        [XmlElement("ProgramsFolder")]
        public string ProgramsFolder { get; set; }

        [XmlElement("UncPath")]
        public string UncPath { get; set; }

        [XmlElement("UserName")]
        public string UserName { get; set; }

        [XmlElement("UserPassword")]
        public string UserPassword { get; set; }


    }
    public class OptionDefinition
    //스키마
    {
        public string LogicalName { get; set; }
        public string DefaultValue { get; set; }
        public Dictionary<string, string> VersionNameMap { get; set; } = new();
    }
    public class UserOption
    //사용자 옵션 
    {
        public string Program { get; set; }
        public string CurrentVersion { get; set; }
        public Dictionary<string, string> CurrentValues { get; set; } = new();
        public DateTime? LastModified { get; set; }
    }

    public class SoftwareVersion
    {
        public string Code { get; set; }
        public string Version { get; set; }
        public List<OptionCategory> OptionCategories { get; set; } = new List<OptionCategory>();
    }
    public class OptionCategory// SystemOption, GPIOOption, KeyboardOption
    {
        public string FilePath { get; set; }
        public string CategoryName { get; set; }
        public List<OptionProperty> OptionProperties { get; set; } = new List<OptionProperty>();
    }
    public class OptionProperty
    {
        public string Name { get; set; }
        public string TypeOrValue { get; set; }
        public string refName { get; set; }
    }

    public enum UserEditMode
    {
        추가,
        변경,
        삭제
    }
    public class UserViewModel
    {// ui바인딩용 
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; } // 이걸 UI에 바인딩
        public bool Activated { get; set; } // 이걸 UI에 바인딩

        public int RoleId { get; set; }

        //public string Mode { get; set; }
    }
    public class PermissionViewModel : ViewModelBase
    {
        public int PermissionId { get; set; }
        public string Name
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
        public bool IsSelected
        {

            get => GetValue<bool>();
            set => SetValue(value);
        }
    }
}
