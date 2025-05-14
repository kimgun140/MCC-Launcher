using MCC_Launcher.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace MCC_Launcher.Services
{
    public class RealLauncherService
    {

        RegisterService registerService = new RegisterService();


        public bool IsProgramInstalled2(string InstallPath, string ProgramName, string versionPath)
        //설치확인
        {
            string installPath = Path.Combine(InstallPath, ProgramName, versionPath);

            return Directory.Exists(installPath);
        }



        public async Task ProgramSetupAsync(IProgress<int> progress, ProgramEntity programEntity, ProgramVersionEntity programVersionEntity,  Action<bool> setProgressBarVisibility)
        {
            //
            setProgressBarVisibility(true);
            var sourceDir = Path.Combine(programEntity.SmbSourcePath, programEntity.ProgramName, programVersionEntity.VersionName);

            var destinationDir = Path.Combine(programVersionEntity.InstallPath, programEntity.ProgramName, programVersionEntity.VersionName);

            // 모든 하위 폴더 포함하여 파일 개수 계산
            var files = Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories);
            int total = files.Length;
            int current = 0;


            Directory.CreateDirectory(destinationDir);

            foreach (string file in files)
            {
                string relativePath = file.Substring(sourceDir.Length + 1); // 상대 경로
                string destFile = Path.Combine(destinationDir, relativePath);
                string destFolder = Path.GetDirectoryName(destFile)!;

                //if (File.Exists(destFolder))
                    Directory.CreateDirectory(destFolder);// 
                try
                {
                    using (FileStream sourceStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                    using (FileStream destinationStream = new FileStream(destFile, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        byte[] buffer = new byte[32768];
                        int bytesRead;
                        while ((bytesRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await destinationStream.WriteAsync(buffer, 0, bytesRead);
                        }
                    }

                    current++;
                    progress.Report((int)(current / (double)total * 100));
                }
                catch (IOException ioEx)
                {
                    MessageBox.Show($"복사 실패: {file}\n{ioEx.Message}");
                }
            }

            setProgressBarVisibility(false);
        }
        public void RunProgram1(ProgramEntity programEntity, ProgramVersionEntity programVersionEntity)
        // 프로그램 이름을 전달해야겠네 
        {


            var destinationDir = Path.Combine(programVersionEntity.InstallPath, programEntity.ProgramName, programVersionEntity.VersionName);
            string exePath = Path.Combine(destinationDir, programVersionEntity.MainExecutable);
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
                    WorkingDirectory = destinationDir,
                    //Arguments = $"--jwt=\"{token}\"",

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
        public void deleteDirectory(string InstalledDir)
        //삭제
        {

            //Task task = new Task(() => deleteDirectory(InstalledDir));
            try
            {
                if (string.IsNullOrEmpty(InstalledDir) || !Directory.Exists(InstalledDir))
                {
                    MessageBox.Show("삭제할 폴더가 존재하지 않습니다.");
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
        public void OptionExport(ProgramEntity programEntity, ProgramVersionEntity programVersionEntity )
        {
            // 설정 파일 이름
             string file = "ProgramSettings.xml";
            //DateTime now = DateTime.Now;
            string formattedTime = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            // 설치된 프로그램 경로 가져오기
            string installPath = Path.Combine(programVersionEntity.InstallPath, programEntity.ProgramName, programVersionEntity.VersionName);
            //


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
            string versionFolderName = Path.GetFileName(programVersionEntity.VersionName); // 예: V1.0
            string backupFileName = $"{formattedTime}_ProgramSettings_{versionFolderName}.xml";
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
        public string ImportOptionFolder(ProgramEntity programEntity, ProgramVersionEntity programVersionEntity)//옵션을 선택하게 되어있네 
        {
            // 백업폴더에 들어있는 옵션을 찾는다. 
            // 설치된 프로그램 경로
            string installPath = Path.Combine(programVersionEntity.InstallPath, programEntity.ProgramName);
            // 백업 폴더 경로
            string backupFolder = Path.Combine(installPath, "백업폴더");
            // 폴더 선택 다이얼로그
            using (var folderDialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                folderDialog.SelectedPath = backupFolder;
                folderDialog.Description = "옵션이 포함된 백업 폴더를 선택하세요.";

                if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    // 백업폴더를 선택 
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
        public UserOption? LatestRunVersionRecord(ProgramEntity programEntity, ProgramVersionEntity programVersionEntity)
        // 
        {

            const string file = "ProgramSettings.xml";
            // 버전 별로 1개 

            // 설치된 프로그램 경로 가져오기
            string installPath = Path.Combine(programVersionEntity.InstallPath, programEntity.ProgramName, programVersionEntity.VersionName);
            string version = programVersionEntity.VersionName;
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

        public List<OptionDefinition> LoadCompatibility(ProgramEntity programEntity, ProgramVersionEntity programVersionEntity)
        //스키마 리스트에 있는 버전별 속성 가져오기 스키마 불러오기
        {
            // 옵션 호환성 관리파일은 selectedProgram.folder에 위치 ,
            string schemafile = "OptionSchema.xml";
            string SelectedProgramPath = Path.Combine(programEntity.SmbSourcePath, programEntity.ProgramName);

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
        public Dictionary<string, string> ConvertUserOption(UserOption user, List<OptionDefinition> compatibilityList, string targetVersion)
        {// 호환성 맞추기 

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

        public async Task RepairProgramAsync(ProgramEntity program, ProgramVersionEntity version)
        {
            string installPath = Path.Combine(version.InstallPath, program.ProgramName, version.VersionName);
            string sourcePath = Path.Combine(program.SmbSourcePath, program.ProgramName, version.VersionName);

            var files = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                string relative = file.Substring(sourcePath.Length + 1);
                string destFile = Path.Combine(installPath, relative);

                // 파일이 없거나 다르면 복사
                if (!File.Exists(destFile) || !FilesAreEqual(file, destFile))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(destFile)!);
                    File.Copy(file, destFile, overwrite: true);
                }
            }
        }
        public void SaveUpdatedUserOption(ProgramEntity programEntity, ProgramVersionEntity programVersionEntity,  Dictionary<string, string> updatedValues)

        {//호환성 변경된 내용 xml쓰기
            var root = new XElement("Option");
           var outputPath = Path.Combine(programVersionEntity.InstallPath, programEntity.ProgramName,programVersionEntity.VersionName, "ProgramSettings.xml");

            // 메타 정보 추가
            root.Add(new XElement("program", programEntity.ProgramName));
            root.Add(new XElement("version", programVersionEntity.VersionName));

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

        public bool FilesAreEqual(string filePath1, string filePath2)
        {
            FileInfo fileInfo1 = new FileInfo(filePath1);
            FileInfo fileInfo2 = new FileInfo(filePath2);

            // 크기 다르면 바로 false
            if (fileInfo1.Length != fileInfo2.Length)
                return false;

            const int bufferSize = 1024 * 8;
            using FileStream fs1 = fileInfo1.OpenRead();
            using FileStream fs2 = fileInfo2.OpenRead();

            byte[] buffer1 = new byte[bufferSize];
            byte[] buffer2 = new byte[bufferSize];

            int bytesRead1, bytesRead2;
            do
            {
                bytesRead1 = fs1.Read(buffer1, 0, bufferSize);
                bytesRead2 = fs2.Read(buffer2, 0, bufferSize);

                if (bytesRead1 != bytesRead2)
                    return false;

                for (int i = 0; i < bytesRead1; i++)
                {
                    if (buffer1[i] != buffer2[i])
                        return false;
                }

            } while (bytesRead1 > 0);

            return true;

            return true;
        }
    }
}
