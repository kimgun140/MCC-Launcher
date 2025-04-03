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

        public bool isInstalled { get; set; }

    }
    // 버전별 데이터 
    [XmlRoot("ProgramMetaData")]
    public class ProgramMetaData
    {
        [XmlElement("Version")]
        public string Version { get; set; }

        [XmlElement("PatchNote")]
        public string PatchNote { get; set; }

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
    public class Options
    {
        public string OptionName { get; set; }
        public bool OptionChecked { get; set; }

    }
}
