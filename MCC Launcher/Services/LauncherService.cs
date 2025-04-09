using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using System.Xml.Serialization;
using static System.Windows.Forms.Design.AxImporter;

namespace MCC_Launcher.Services
{

    public class LauncherService

    {
        [DllImport("mpr.dll")]
        // Windows API 함수 WNetAddConnection2를 호출하기 위한 선언입니다. 
        // Multiple Provider Router DLL 에서 제공하는 함수, smb 네트워크 드라이브 연결을 설정하는 역할을 한다. netresource 구조체를 전달하여 네트워크 경로를 지정하고, username과 password로 인증을 수행한다. 
        // 스토리지 접근권한 주기
        private static extern int WNetAddConnection2(ref NETRESOURCE netResource, string password, string username, int flags);

        [DllImport("mpr.dll")]
        // smb 네트워크 드라이브 연결을 해재하는 기능을 한다.
        private static extern int WNetCancelConnection2(string name, int flags, bool force);
        [StructLayout(LayoutKind.Sequential)]// 메모리에 순차적으로 배치하다
        private struct NETRESOURCE
        // 네트워크 리소스 나타내는 구조체 
        {
            public int dwScope;
            public int dwType;//리소스 유형 1 이면 디스크 드라이브 
            public int dwDisplayType;
            public int dwUsage;
            public string lpLocalName;// 로컬드라이브 문자, 매핑할 경우 사용 
            public string lpRemoteName;// 원격 경로
            public string lpComment;
            public string lpProvider;
        }

        //string uncPath = @"\\gms-mcc-nas01\AUDIO-FILE\test1\test121.txt";
        ////string content = "이것은 공유 스토리지에 저장되는 테스트 파일입니다.";
        //public string username = "develop";
        //public string password = "Akds0ft3!48";

        public const string XmlFilePath = @"C:\Program Files\launcherfolder\ProgramsPath.xml";
        //런처 폴더 경로

        // 프로그램 설치 폴더 경로 
        public const string installedPath = @"C:\Program Files\LauncherPrograms";
        string schemafile = "OptionSchema.xml";
        //public string XmlFilePaths = Path.Combine(XmlFilePath, "ProgramsPath.xml");

        LauncherConfig launcherConfig;




        public ObservableCollection<Program> LoadPrograms()
        {

            ObservableCollection<Program> programs = new ObservableCollection<Program>();

            if (!File.Exists(XmlFilePath))
                return new ObservableCollection<Program>();

            programs.Clear();

            //  런처 XML을 역직렬화하여 프로그램 폴더 경로 가져오기
            XmlSerializer launcherSerializer = new XmlSerializer(typeof(LauncherConfig));
            //if (launcherConfig == null)

            using (StreamReader reader = new StreamReader(XmlFilePath))
            {
                launcherConfig = (LauncherConfig)launcherSerializer.Deserialize(reader);
            }

            if (string.IsNullOrEmpty(launcherConfig.ProgramsFolder) || !Directory.Exists(launcherConfig.ProgramsFolder))
                return new ObservableCollection<Program>();


            // 
            var programFolders = Directory.GetDirectories(launcherConfig.ProgramsFolder);
            // unc 경로에 위치한 폴더 목록  가져오기

            foreach (var programFolder in programFolders)

            {
                string programXmlPath = Path.Combine(programFolder, "program.xml");


                if (!File.Exists(programXmlPath))
                    continue;


                XmlSerializer programSerializer = new XmlSerializer(typeof(Program));
                Program programData;
                //여기서 접근 

                using (StreamReader reader = new StreamReader(programXmlPath))
                {
                    programData = (Program)programSerializer.Deserialize(reader);

                }


                //programData.Versions.isInstalled = IsProgramInstalled(programpath, folder);
                programData.FolderPath = programFolder;
                programData.IconPath = Path.Combine(programFolder, programData.IconPath);
                //programData.ProgramName = ;
                // 버전 경로 넣고 폴더 있으면 설치된거이



                programs.Add(programData);

            }
            // 프로그램 데이터 
            return programs;

        }
        public VersionInfo AllPatchNotes(string selectedProgram)
        //패치노트들만 읽기 
        {
            var programFolders = Directory.GetDirectories(launcherConfig.ProgramsFolder);
            var programpath = Path.GetFullPath(selectedProgram);
            // 프로그램들 폴더 \\Gms-mcc-nas01\audio-file\test1\programs
            VersionInfo version = new VersionInfo();

            version.PatchNotes = new ObservableCollection<string>();

            //var programpath = Path.GetFileName(selectedProgram);
            var path = Path.Combine(programFolders[0], programpath);
            //[0]이 아니네 
            var folders = Directory.GetDirectories(path);
            if (folders == null)
            {
                return version;
            }
            foreach (var folder in folders)
            {
                string programXmlPath = Path.Combine(folder, "ProgramMetaData.xml");
                XmlSerializer programSerializer = new XmlSerializer(typeof(ProgramMetaData));
                using (StreamReader reader = new StreamReader(programXmlPath))
                {

                    ProgramMetaData metaData = (ProgramMetaData)programSerializer.Deserialize(reader);

                    //var versionPath=  Path.Combine(folder, metaData.Version);//경로
                    //var versionPath = metaData.Version;// 버전숫자 

                    //여기서 설치되었느지 검사해서 넣기 

                    version.PatchNote = metaData.PatchNote; // 
                                                   version.MainExecutable = metaData.MainExecutable;

                    //version.isInstalled = metaData.isInstalled;


                    version.PatchNotes.Add(metaData.PatchNote);
                    //version.isInstalled = true;
                    //패치노트가 patch notes


                }
            }
            return version;
        }
        //private void asdf()
        //{

        //}

        public void Connection()
        {

            //unc경로, 아이디 , 비밀번호 읽기 
            XmlSerializer launcherSerializer = new XmlSerializer(typeof(LauncherConfig));

            using (StreamReader reader = new StreamReader(XmlFilePath))
            {
                launcherConfig = (LauncherConfig)launcherSerializer.Deserialize(reader);
            }
            //if (string.IsNullOrEmpty(launcherConfig.ProgramsFolder) || !Directory.Exists(launcherConfig.ProgramsFolder))
            //    return;
            string path = launcherConfig.UncPath;
            string username = launcherConfig.UserName;
            string password = launcherConfig.UserPassword;


            NETRESOURCE netResource = new NETRESOURCE
            {
                dwType = 1,// 파일드라이버처럼 처리
                lpRemoteName = path
            };
            // 구조체 초기화해 경로 설정 
            int result = WNetAddConnection2(ref netResource, password, username, 0);


            //연결 끊기 
            //WNetCancelConnection2(path, 0, true);

            if (result == 0)// 0이면 연결 
            {

                //MessageBox.Show("연결");

            }
            else
            {
                MessageBox.Show(result.ToString());


            }
        }

        public void RunProgram(string programFolder, string versionPath, string ExeFile)
            // 프로그램 이름을 전달해야겠네 
        {
          
            // xml에서 읽어오거나 정해놓거나 
            string installPath = GetInstalledversionPath(programFolder, versionPath);
            programFolder = Path.GetFileName(programFolder);
            installPath = Path.Combine(installPath, programFolder);
            string exePath = Path.Combine(installPath, ExeFile);
            if (!File.Exists(exePath))
            {
                MessageBox.Show("실행할 파일이 없습니다.");
                //return;
            }
            try
            {

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = exePath,
                    WorkingDirectory = installPath,
                    Arguments = " ",
                    //ArgumentList = { " ", "" },
                    Verb = "runas",
                    UseShellExecute = true,
                };
                Process.Start(startInfo);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }




        }
        public void OptionExport(string programFolder, string versionPath)
        {
            // 설정 파일 이름
            const string file = "ProgramSettings.xml";

            // 설치된 프로그램 경로 가져오기
            string installPath = GetInstalledversionPath(programFolder, versionPath);


            // 원본 설정 파일 경로
            string sourceFilePath = Path.Combine(installPath, file);

            //  백업 폴더는 해당 프로그램 폴더에 위치해야 함
            string programRootPath = Path.GetDirectoryName(installPath); // ProgramA 폴더 경로
            string backupFolder = Path.Combine(programRootPath, "백업폴더");
            if (!File.Exists(backupFolder))
            {
                Directory.CreateDirectory(backupFolder); // 폴더가 없으면 생성 애초에 설치 폴더에 있어도 되겠다. 
            }
            // 버전명을 포함한 백업 파일명 생성
            string versionFolderName = Path.GetFileName(versionPath); // 예: V1.0
            string backupFileName = $"ProgramSettings_{versionFolderName}.xml";
            string backupFilePath = Path.Combine(backupFolder, backupFileName);
            if (!File.Exists(sourceFilePath))
            {
                //MessageBox.Show("백업할 설정 파일이 없습니다.");
                return;
            }
            // 파일 복사
            using (FileStream sourceStream = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (FileStream destinationStream = new FileStream(backupFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                byte[] bytes = new byte[32768];
                int bytesRead;
                while ((bytesRead = sourceStream.Read(bytes, 0, bytes.Length)) > 0)
                {
                    destinationStream.Write(bytes, 0, bytesRead);
                }
            }

        }

        public void OptionFolderBackup(string programFolder, string versionPath)
        {
            string installVersionPath = GetInstalledversionPath(programFolder, versionPath);
            string configSourceFolder = Path.Combine(installVersionPath, "Config");

            if (!Directory.Exists(configSourceFolder))
            {
                MessageBox.Show("Config 폴더가 존재하지 않습니다.");
                return;
            }

            // 백업 위치: ProgramA/백업폴더/v1.0.1/Config/
            string programRootPath = Path.GetDirectoryName(installVersionPath)!;
            string versionFolderName = Path.GetFileName(versionPath); // ex) v1.0.1
            string backupTargetPath = Path.Combine(programRootPath, "백업폴더", versionFolderName, "Config");

            Directory.CreateDirectory(backupTargetPath);

            // 기존 Config 폴더 전체를 복사
            foreach (string file in Directory.GetFiles(configSourceFolder, "*.*", SearchOption.AllDirectories))
            {
                string relativePath = Path.GetRelativePath(configSourceFolder, file);
                string targetPath = Path.Combine(backupTargetPath, relativePath);
                Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);
                File.Copy(file, targetPath, overwrite: true);
            }

            MessageBox.Show("옵션 폴더 백업이 완료되었습니다.");
        }


        public UserOption LoadBackupOption(string SelectedBackupOption)
        // import할때 백업 옵션 읽고, 버전 비교
        {
            //if()
            var userdoc = XDocument.Load(SelectedBackupOption);
            var userRoot = userdoc.Root;
            var user = new UserOption
            {
                Program = userRoot.Element("program")?.Value,

                CurrentVersion = userRoot.Element("version")?.Value

            };


            // 나머지 엘리먼트 처리
            foreach (var elem in userRoot.Elements())
            {
                if (elem.Name == "program" || elem.Name == "version")
                    continue;

                if (elem.Name == "date")
                {
                    if (DateTime.TryParse(elem.Value, out var parsedDate))
                    {
                        user.LastModified = parsedDate;
                    }
                    continue;
                }

                user.CurrentValues[elem.Name.LocalName] = elem.Value;

            }
            return user;
        }



        public void SaveUpdatedUserOption(string outputPath, string programName, string targetVersion, Dictionary<string, string> updatedValues)

        {//호환성 변경된 내용 xml쓰기
            var root = new XElement("Option");
            outputPath = Path.Combine(outputPath, "ProgramSettings.xml");

            // 메타 정보 추가
            root.Add(new XElement("program", programName));
            root.Add(new XElement("version", targetVersion));

            //  현재 시간 기록
            root.Add(new XElement("date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));

            // 옵션 값들 추가
            foreach (var kvp in updatedValues)
            {
                root.Add(new XElement(kvp.Key, kvp.Value));
            }

            var doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);
            doc.Save(outputPath);

            //MessageBox.Show($"변환된 설정이 저장되었습니다: {outputPath}");
        }
        public string GetInstalledversionPath(string programFolder, string versionPath)
        // 경로만들기 
        {
            string versionFolderName = Path.GetFileName(versionPath);
            string programFolderName = Path.GetFileName(programFolder);

            return Path.Combine(installedPath, programFolderName, versionFolderName);
        }
        public string GetInstalledProgramFolderPath(string programFolder)
        {
            string programFolderName = Path.GetFileName(programFolder);

            //로컬 프로그램 폴더 
            return Path.Combine(installedPath, programFolderName);
        }

        public bool IsProgramInstalled(string programFolder, string versionPath)
        //설치확인
        {
            string installPath = GetInstalledversionPath(programFolder, versionPath);

            return Directory.Exists(installPath);
        }



        public async void deleteDirectory(string InstalledDir)
        //삭제
        {

            //Task task = new Task(() => deleteDirectory(InstalledDir));
            try
            {
                if (string.IsNullOrEmpty(InstalledDir) || !Directory.Exists(InstalledDir))
                {
                    //MessageBox.Show("삭제할 폴더가 존재하지 않습니다.");
                    return;
                }
                foreach (string file in Directory.GetFiles(InstalledDir))
                {
                    File.Delete(file);
                }
                foreach (string dir in Directory.GetDirectories(InstalledDir))
                {
                    deleteDirectory(dir);
                }
                Directory.Delete(InstalledDir);
            }
            catch (Exception e)
            {
                MessageBox.Show("삭제 실패");
                //Log(e.Message);
            }
        }
        public async Task ProgramSetup(IProgress<int> progress, string sourceDir, string destinationDir, Action<bool> SetProgressBarVisibility)
        // 설치
        {
            SetProgressBarVisibility(true);

            //  모든 하위 폴더 포함하여 파일 개수 계산
            var files = Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories);
            int total = files.Length; // 전체 파일 개수
            int current = 0; // 현재 복사된 파일 개수

            //  대상 폴더 생성 (없으면 생성)
            Directory.CreateDirectory(destinationDir);

            //  모든 파일 복사 + 진행률 업데이트
            foreach (string file in files)
            {
                string relativePath = file.Substring(sourceDir.Length + 1); // 상대 경로 가져오기
                string destFile = Path.Combine(destinationDir, relativePath);
                string destFolder = Path.GetDirectoryName(destFile)!;

                Directory.CreateDirectory(destFolder); // 파일이 속한 폴더가 없으면 생성

                using (FileStream sourceStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (FileStream destinationStream = new FileStream(destFile, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    //await Task.Delay(1000);
                    // 다운로드할 때 프로그래스바 눈으로 확인하기 
                    try
                    {
                        byte[] bytes = new byte[32768];
                        int bytesRead;

                        while ((bytesRead = await sourceStream.ReadAsync(bytes, 0, bytes.Length)) > 0)
                        {
                            await destinationStream.WriteAsync(bytes, 0, bytesRead);
                        }

                        //  진행률 업데이트
                        progress.Report((int)(++current / (double)total * 100));

                    }
                    catch (IOException)
                    {
                        MessageBox.Show($"복사 실패: {file}");
                    }


                }
            }
            SetProgressBarVisibility(false);
            //MessageBox.Show("설치완료");
            //progress.Report(100);
        }


        public async Task InstallProgram(IProgress<int> progress, string sourcePath, string programFolder, Action<bool> updateVisibility)
        //경로에 설치
        {
            string fullInstallPath = GetInstalledversionPath(programFolder, sourcePath);
            await ProgramSetup(progress, sourcePath, fullInstallPath, updateVisibility);
        }

        public VersionInfo LoadMetaData(string Path, VersionInfo version)//패치노트 
        {
            string metadataPath = System.IO.Path.Combine(Path, "ProgramMetaData.xml");
            if (!File.Exists(metadataPath))
            {
                //PatchNote = "패치 노트 없음";
                return version;
            }

            XmlSerializer serializer = new XmlSerializer(typeof(ProgramMetaData));
            using (StreamReader reader = new StreamReader(metadataPath))
            {
                ProgramMetaData metaData = (ProgramMetaData)serializer.Deserialize(reader);
                version.PatchNotes = new ObservableCollection<string>();
                //version.PatchNote
                //var PatchNotes = new ObservableCollection<string>();
                version.PatchNote = metaData.PatchNote; // 
                version.MainExecutable = metaData.MainExecutable;
                version.PatchNotes.Add(metaData.PatchNote);
                //패치노트가 patch notes
                return version;
            }
        }

        public void SaveLastUsedVersion(string programFolderPath, string versionName)
        {
            string recordFilePath = Path.Combine(programFolderPath, "last_used_version.txt");
            // 최근 실행한 버전을 기록 

            try
            {
                File.WriteAllText(recordFilePath, versionName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("최근 실행 버전 기록 실패: " + ex.Message);
            }
        }





        public UserOption? LatestRunVersionRecord(string programFolder, string versionPath)
        {

            const string file = "ProgramSettings.xml";
            // 버전 별로 1개 

            // 설치된 프로그램 경로 가져오기
            string installPath = GetInstalledversionPath(programFolder, versionPath);
            string version = Path.GetFileName(versionPath);
            // 원본 설정 파일 경로
            string sourceFilePath = Path.Combine(installPath, file);

            string programRootPath = Path.GetDirectoryName(installPath); // ProgramA 폴더 경로
            string backupFolder = Path.Combine(programRootPath, "백업폴더");
            // 선택한 프로그램 버전의 programsettings_v1.0.1.xml를 가져오기 

            //옵션파일은 현재 실행 버전이랑, 선택된 버전랑 비교
            //최근 실행버전 기록

            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "xml Files| *.xml";
            fileDialog.InitialDirectory = backupFolder;
            string[] filename;
            string filepath;
            bool? success = fileDialog.ShowDialog();
            // 여기서 파일 선택한걸로 버전이 다르면 마이그레이션 할건지 물어보기 
            // 선택한 버전이랑 옵션파일이랑같은 버전인경우에는 현재 옵션파일을 이걸로 덮어씌우기 할건지 물어보고 실행하기
            // 백업파일도 여러개 만들 수 있게 해야겠네 
            if (success == true)
            {
                //파일 절대경로, 선택한 백업파일
                filename = fileDialog.FileNames;
                filepath = filename[0];
                //파일이름.확장자
                //string[] fileshortname = fileDialog.SafeFileNames;
                UserOption user = LoadBackupOption(filepath);
                return user;
            }
            else
            {
                // 파일선택안했을떄 
                return null;

            }


        }

        public List<SoftwareVersion> LoadCompAbilityList(string SelectedProgramPath) //dz 
        // 옵션스키마읽기 
        {
            string Softwareversions = "SoftwareVersions.xml";
            string SoftwareversionsPath = Path.Combine(SelectedProgramPath, Softwareversions);

            var SoftwareVersionList = new List<SoftwareVersion>();
            var doc = XDocument.Load(SoftwareversionsPath);
            foreach (var VersionElement in doc.Root.Elements("SoftwareVersion"))
            {
                var version = new SoftwareVersion()
                {
                    Code = VersionElement.Attribute("Code").Value,
                    Version = VersionElement.Attribute("Version").Value,
                    OptionCategories = new List<OptionCategory>()
                };


                foreach (var CategoryElement in VersionElement.Elements())
                {
                    var category = new OptionCategory()
                    {
                        CategoryName = CategoryElement.Name.LocalName,// 태그이름 
                        FilePath = CategoryElement.Attribute("FilePath").Value,
                        OptionProperties = new List<OptionProperty>()
                    };


                    foreach (var PropertyElement in CategoryElement.Elements())
                    {
                        category.OptionProperties.Add(
                            new OptionProperty
                            {
                                Name = PropertyElement.Name.LocalName,
                                TypeOrValue = PropertyElement.Value,
                                refName = PropertyElement.Attribute("refName")?.Value

                            });

                    }
                    version.OptionCategories.Add(category);

                }
                SoftwareVersionList.Add(version);
            }
            return SoftwareVersionList;

        }

        public Dictionary<string, List<OptionProperty>> LoadUserOptionsGrouped(string xmlPath)
        {// 사용자 옵션 읽기 

            var result = new Dictionary<string, List<OptionProperty>>();
            if (!File.Exists(xmlPath))
            {
                //옵션파일 
                return result;
            }
            var doc = XDocument.Load(xmlPath);
            var softwareVersionElem = doc.Element("SoftwareVersion");

            if (softwareVersionElem == null)
                throw new Exception("루트 요소 <SoftwareVersion>이 없습니다.");

            foreach (var categoryElem in softwareVersionElem.Elements())
            {
                string categoryName = categoryElem.Name.LocalName;

                var properties = categoryElem.Elements()
                    .Select(p => new OptionProperty
                    {
                        Name = p.Name.LocalName,
                        TypeOrValue = p.Value,
                        refName = p.Attribute("refName")?.Value
                    }).ToList();

                result[categoryName] = properties;
            }

            return result;
        }
        public Dictionary<string, List<OptionProperty>> LoadGroupedUserOptionsBySchema(
    string programFolderPath,
    string versionName)
            //사용자옵션파일 호환 
        {
            var compatibilityList = LoadCompAbilityList(programFolderPath);
            var versionSchema = compatibilityList.FirstOrDefault(v => v.Version == versionName);
            if (versionSchema == null)
                throw new Exception($"버전 {versionName}에 대한 스키마 정보를 찾을 수 없습니다.");

            // 설치 경로 가져오기 (예: C:\Programs\ProgramA\v1.0.1)
            string installedPath = GetInstalledversionPath(programFolderPath, versionName);

            var result = new Dictionary<string, List<OptionProperty>>();

            foreach (var category in versionSchema.OptionCategories)
            {
                // 스키마에서 지정한 옵션 파일 경로 조합
                string fullPath = Path.Combine(installedPath, category.FilePath);

                // 그룹별 옵션 로드
                var group = LoadUserOptionsGrouped(fullPath);

                // Dictionary에 추가 (카테고리 이름 기준)
                if (group.TryGetValue(category.CategoryName, out var optionList))
                {
                    result[category.CategoryName] = optionList;
                }
            }

            return result;
        }

        //이거이거
        public Dictionary<OptionCategory, Dictionary<string, string>> ConvertOptionsGroupedByCategory(
                 Dictionary<string, List<OptionProperty>> currentOptions,
                 List<SoftwareVersion> softwareVersions,
                 string targetVersion)
            //호환 
        {
            var result = new Dictionary<OptionCategory, Dictionary<string, string>>();

            var targetVersionInfo = softwareVersions.FirstOrDefault(v => v.Version == targetVersion);
            if (targetVersionInfo == null)
                throw new Exception($"타겟 버전 {targetVersion}을 찾을 수 없습니다.");

            foreach (var category in targetVersionInfo.OptionCategories)
            {
                var categoryName = category.CategoryName;
                var targetProps = category.OptionProperties;
                var outputValues = new Dictionary<string, string>();

                currentOptions.TryGetValue(categoryName, out var userOptions);
                userOptions ??= new List<OptionProperty>();

                foreach (var targetProp in targetProps)
                {
                    string sourceKey = targetProp.refName ?? targetProp.Name;
                    string userValue = userOptions.FirstOrDefault(p => p.Name == sourceKey)?.TypeOrValue;

                    outputValues[targetProp.Name] = userValue ?? targetProp.TypeOrValue;
                }

                result[category] = outputValues;
            }

            return result;
        }
        //호환 맞추고  저장 
        public void SaveMigratedOptionsGrouped(
            Dictionary<OptionCategory, Dictionary<string, string>> groupedOptions,
            string outputFolder,
            string programCode,
            string targetVersion)
        {
            foreach (var kvp in groupedOptions)
            {
                var category = kvp.Key;
                var settings = kvp.Value;

                var softwareVersionElement = new XElement("SoftwareVersion");
                softwareVersionElement.SetAttributeValue("Code", programCode);
                softwareVersionElement.SetAttributeValue("Version", targetVersion);

                var categoryElement = new XElement(category.CategoryName);

                foreach (var setting in settings)
                {
                    categoryElement.Add(new XElement(setting.Key, setting.Value));
                }

                softwareVersionElement.Add(categoryElement);

                var outputPath = Path.Combine(outputFolder, category.FilePath);
                var outputDir = Path.GetDirectoryName(outputPath);
                if (!Directory.Exists(outputDir))
                    Directory.CreateDirectory(outputDir);

                new XDocument(new XDeclaration("1.0", "utf-8", "yes"), softwareVersionElement)
                    .Save(outputPath);
            }
        }













        public Dictionary<string, string> ConvertUserOption(UserOption user, List<OptionDefinition> compatibilityList, string targetVersion)
        {

            var result = new Dictionary<string, string>();

            foreach (var option in compatibilityList)
            {
                string logicKey = option.LogicalName;

                // 현재 사용자가 가진 옵션 이름 찾기 (버전 상관 없이)
                string currentName = option.VersionNameMap
                    .Values
                    .FirstOrDefault(name => user.CurrentValues.ContainsKey(name));

                // 대상 버전에서 사용할 이름
                if (!option.VersionNameMap.TryGetValue(targetVersion, out string targetName))
                    continue; // 이 논리 옵션은 해당 버전에서는 존재하지않는 옵션

                // 제거 조건: None
                if (string.Equals(targetName, "None", StringComparison.OrdinalIgnoreCase))
                {
                    continue; // 대상 버전에서는 제거됨
                }

                if (currentName != null)
                {
                    // 사용자가 현재 값을 가지고 있는 경우 → 이름만 바꿔줌
                    result[targetName] = user.CurrentValues[currentName];
                }
                else
                {
                    // 사용자가 이 옵션을 가지고 있지 않음 → Default 값 추가
                    result[targetName] = option.DefaultValue;
                }
            }

            return result;
        }
        public List<OptionDefinition> LoadCompatibility(string SelectedProgramPath, string SelectedVersion)
        //스키마 리스트에 있는 버전별 속성 가져오기 스키마 불러오기
        {
            //
            // 옵션 호환성 관리파일은 selectedProgram.folder에 위치 ,
            //string schemafile = "OptionSchema.xml";
            string compatibilityPath = Path.Combine(SelectedProgramPath, schemafile);
            // 옵션  스키마 위치 
            var versionMap = new Dictionary<string, string>();



            var doc = XDocument.Load(compatibilityPath);
            var shcemalist = new List<OptionDefinition>();
            //스키마 옵션 리스트

            foreach (var OptionElement in doc.Root.Elements("Option"))
            {
                var option = new OptionDefinition()
                {
                    LogicalName = OptionElement.Attribute("name").Value,
                    DefaultValue = OptionElement.Element("Default").Value,
                };
                foreach (var VersionElement in OptionElement.Elements())
                {
                    if (VersionElement.Name == "default")
                        continue;
                    string ver = VersionElement.Name.LocalName; // 그대로 사용
                    option.VersionNameMap[ver] = VersionElement.Value;
                }
                shcemalist.Add(option);

            }
            return shcemalist;
        }
        public string ImportOptionFolder(string programFolder, string versionPath)
        {
            // 설치된 프로그램 경로
            string installPath = GetInstalledversionPath(programFolder, versionPath);
            string version = Path.GetFileName(versionPath);

            // 백업 폴더 경로
            string programRootPath = Path.GetDirectoryName(installPath);
            string backupFolder = Path.Combine(programRootPath, "백업폴더");

            // 폴더 선택 다이얼로그
            using (var folderDialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                folderDialog.SelectedPath = backupFolder;
                folderDialog.Description = "옵션이 포함된 백업 폴더를 선택하세요.";

                if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string selectedFolder = folderDialog.SelectedPath;

                    // 사용자가 선택한 폴더 안의 옵션 파일 경로를 구성
                    //string optionFilePath = Path.Combine(selectedFolder, "");
                    //if (!File.Exists(optionFilePath))
                    //{
                    //    MessageBox.Show("선택한 폴더에 ProgramSettings.xml 파일이 없습니다.");
                    //    //folderDialog.SelectedPath;
                    //    return null;
                    //}

                    // 옵션 불러오기
                    //UserOption user = LoadBackupOption(optionFilePath);
                    return folderDialog.SelectedPath;
                }
                else
                {
                    return null; // 취소
                }
            }
        }
    }
}
