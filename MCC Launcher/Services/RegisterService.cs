using MCC_Launcher.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
        //public bool RegisterProgram(ProgramRegistrationViewModel viewModel, string storageRootPath)
        //{
        //    using var context = new LauncherDbContext();

        //    // 프로그램 엔티티 생성
        //    var programEntity = new ProgramEntity
        //    {
        //        ProgramName = viewModel.ProgramName,
        //        Description = viewModel.Description,
        //        SmbSourcePath = viewModel.SmbSourcePath, // UNC 스토리지 경로 
        //        //IconPath = viewModel.IconPath,
        //        Versions = new List<ProgramVersionEntity>()
        //    };

        //    foreach (var versionVm in viewModel.Versions)
        //    {
        //        // 스토리지 경로 지정
        //        string destinationPath = Path.Combine(storageRootPath, viewModel.ProgramName, versionVm.VersionName);

        //        if (!Directory.Exists(destinationPath))
        //            Directory.CreateDirectory(destinationPath);

        //        // 로컬 폴더 → 스토리지 복사
        //        CopyDirectory(versionVm.LocalFolderPath, destinationPath);

        //        // 버전 엔티티 생성
        //        var versionEntity = new ProgramVersionEntity
        //        {
        //            VersionName = versionVm.VersionName,
        //            SmbSourcePath = destinationPath,
        //            InstallPath = "", // 설치 경로는 설치시 결정
        //            MainExecutable = versionVm.MainExecutable,
        //            PatchNote = versionVm.PatchNote
        //        };

        //        programEntity.Versions.Add(versionEntity);
        //    }
        //    if (programEntity.Versions.Any())
        //    {
        //        programEntity.SmbSourcePath = programEntity.Versions.First().SmbSourcePath;
        //    }
        //    // DB 저장
        //    context.Programs.Add(programEntity);
        //    context.SaveChanges();

        //    return true;
        //}

        //public bool RegisterProgram(ProgramRegistrationViewModel viewModel)
        //{// 빈값들 예외처리 
        //    try
        //    {
        //        using var context = new LauncherDbContext();


        //        var programEntity = new ProgramEntity
        //        {
        //            ProgramName = viewModel.ProgramName,
        //            Description = viewModel.Description,
        //            SmbSourcePath = viewModel.SmbSourcePath,
        //            IconPath = viewModel.IconFileName,
        //            Versions = new List<ProgramVersionEntity>
        //    {
        //        new ProgramVersionEntity
        //        {
        //            VersionName = viewModel.VersionName,
        //            InstallPath = viewModel.InstallPath,
        //            SmbSourcePath = viewModel.SmbSourcePath,
        //            MainExecutable = viewModel.MainExecutable,
        //            PatchNote = viewModel.PatchNote
        //        }
        //    }
        //        };

        //        context.Programs.Add(programEntity);
        //        context.SaveChanges();

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageBox.Show($"프로그램 등록 실패: {ex.Message}");
        //        return false;
        //    }
        //}
        public bool RegisterProgram(ProgramRegistrationViewModel viewModel, string localFolderPath)
        {
            try
            {
                using var context = new LauncherDbContext();

                var existingProgram = context.Programs
                    .Include(p => p.Versions)
                    .FirstOrDefault(p => p.ProgramName == viewModel.ProgramName);

                if (existingProgram == null)
                {
                    // 새 프로그램 등록
                    var programEntity = new ProgramEntity
                    {
                        ProgramName = viewModel.ProgramName,
                        Description = viewModel.Description,
                        IconPath = viewModel.IconFileName,
                        SmbSourcePath = viewModel.SmbSourcePath,
                        Versions = new List<ProgramVersionEntity>()
                    };

                    var versionEntity = new ProgramVersionEntity
                    {
                        VersionName = viewModel.VersionName,
                        SmbSourcePath = Path.Combine(viewModel.SmbSourcePath, viewModel.VersionName),
                        InstallPath = viewModel.InstallPath,
                        MainExecutable = viewModel.MainExecutable,
                        PatchNote = viewModel.PatchNote
                    };

                    programEntity.Versions.Add(versionEntity);

                    context.Programs.Add(programEntity);
                }
                else
                {
                    // 기존 프로그램에 버전만 추가
                    var versionEntity = new ProgramVersionEntity
                    {
                        VersionName = viewModel.VersionName,
                        SmbSourcePath = Path.Combine(existingProgram.SmbSourcePath, viewModel.VersionName),
                        InstallPath = viewModel.InstallPath,
                        MainExecutable = viewModel.MainExecutable,
                        PatchNote = viewModel.PatchNote
                    };

                    existingProgram.Versions.Add(versionEntity);
                }

                // 스토리지로 폴더 복사
                var storagePath = Path.Combine(viewModel.SmbSourcePath,viewModel.ProgramName ,viewModel.VersionName);
                CopyFolder(localFolderPath, storagePath); // 복사 

                context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"프로그램 등록 실패: {ex.Message}");
                return false;
            }
        }
        private void CopyFolder(string sourceFolder, string destinationFolder)
        {
            if (!Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);// 프로그램 폴더 
            }

            foreach (var file in Directory.GetFiles(sourceFolder))
            {
                var destFile = Path.Combine(destinationFolder, Path.GetFileName(file));
                File.Copy(file, destFile, true);
            }

            foreach (var directory in Directory.GetDirectories(sourceFolder))
            {
                var destDir = Path.Combine(destinationFolder, Path.GetFileName(directory));
                CopyFolder(directory, destDir);
            }
        }

    }
}
