using MCC_Launcher.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
    }
}
