using MCC_Launcher.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCC_Launcher.Services
{
    public class RegisterService
    {
        private void CopyDirectory(string sourceDir, string destinationDir)
        {
            foreach (var dirPath in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourceDir, destinationDir));
            }

            foreach (var filePath in Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(filePath, filePath.Replace(sourceDir, destinationDir), true);
            }
        }
        public bool RegisterProgram(ProgramRegistrationViewModel viewModel, string storageRootPath)
        {
            using var context = new LauncherDbContext();

            // 프로그램 엔티티 생성
            var programEntity = new ProgramEntity
            {
                ProgramName = viewModel.ProgramName,
                Description = viewModel.Description,
                IconPath = viewModel.IconPath,
                Versions = new List<ProgramVersionEntity>()
            };

            foreach (var versionVm in viewModel.Versions)
            {
                // 스토리지 경로 지정
                string destinationPath = Path.Combine(storageRootPath, viewModel.ProgramName, versionVm.VersionName);

                if (!Directory.Exists(destinationPath))
                    Directory.CreateDirectory(destinationPath);

                // 로컬 폴더 → 스토리지 복사
                CopyDirectory(versionVm.LocalFolderPath, destinationPath);

                // 버전 엔티티 생성
                var versionEntity = new ProgramVersionEntity
                {
                    VersionName = versionVm.VersionName,
                    SmbSourcePath = destinationPath,
                    InstallPath = "", // 설치 경로는 설치시 결정
                    MainExecutable = versionVm.MainExecutable,
                    PatchNote = versionVm.PatchNote
                };

                programEntity.Versions.Add(versionEntity);
            }

            // DB 저장
            context.Programs.Add(programEntity);
            context.SaveChanges();

            return true;
        }
    }
}
