using MCC_Launcher.Models;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCC_Launcher
{
    public class LauncherDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        //public DbSet<RolePermission> RolePermissions { get; set; }
        //public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<ProgramEntity> Programs { get; set; }
        public DbSet<ProgramVersionEntity> ProgramVersion { get; set; }
        public DbSet<RoleProgramPermission> RoleProgramPermissions { get; set; }
        public DbSet<UserProgramPeriod> UserProgramPeriods { get; set; } //
        //public DbSet<LauncherInfo> Launchers { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=launcherdb;Username=postgres;Password=1234");
        }

        //    protected override void OnModelCreating(ModelBuilder modelBuilder)
        //    {
        //        base.OnModelCreating(modelBuilder);

        //        //  UserInfo → 기존 'Users' 테이블로 매핑
        //        modelBuilder.Entity<UserInfo>().ToTable("Users");
        //        modelBuilder.Entity<UserInfo>().HasKey(u => u.UserId);

        //        //  Program 관계
        //        //modelBuilder.Entity<ProgramsEntity>().HasKey(p => p.ProgramCode);
        //        //modelBuilder.Entity<ProgramVersionEntity>().HasKey(v => v.VersionId);
        //        //modelBuilder.Entity<ProgramVersionEntity>()
        //        //    .HasOne(v => v.Program)
        //        //    .WithMany(p => p.Versions) // Ensure the navigation property name matches the model.  
        //        //    .HasForeignKey(v => v.ProgramCode);

        //        modelBuilder.Entity<ProgramEntity>()
        //.HasKey(p => p.ProgramCode);

        //        modelBuilder.Entity<ProgramEntity>()
        //            .Property(p => p.Name).IsRequired();

        //        modelBuilder.Entity<ProgramEntity>()
        //            .HasMany(p => p.Versions)
        //            .WithOne(v => v.Program)
        //            .HasForeignKey(v => v.ProgramCode)
        //            .OnDelete(DeleteBehavior.Cascade);


        //        // UserRole 관계
        //        modelBuilder.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });
        //        modelBuilder.Entity<UserRole>()
        //            .HasOne(ur => ur.User)
        //            .WithMany(u => u.UserRoles)
        //            .HasForeignKey(ur => ur.UserId);
        //        modelBuilder.Entity<UserRole>()
        //            .HasOne(ur => ur.Role)
        //            .WithMany(r => r.UserRoles)
        //            .HasForeignKey(ur => ur.RoleId);

        //        //  RolePermission 관계
        //        modelBuilder.Entity<RolePermission>().HasKey(rp => new { rp.RoleId, rp.PermissionID });
        //        modelBuilder.Entity<RolePermission>()
        //            .HasOne(rp => rp.Role)
        //            .WithMany(r => r.RolePermissions)
        //            .HasForeignKey(rp => rp.RoleId);
        //        modelBuilder.Entity<RolePermission>()
        //            .HasOne(rp => rp.Permission)
        //            .WithMany(p => p.RolePermissions)
        //            .HasForeignKey(rp => rp.PermissionID);



        //        modelBuilder.Entity<RoleProgramPermission>()
        //.HasKey(rpp => new { rpp.RoleId, rpp.ProgramCode, rpp.PermissionId });

        //        modelBuilder.Entity<RoleProgramPermission>()
        //            .HasOne(rpp => rpp.Role)
        //            .WithMany(r => r.RoleProgramPermissions)
        //            .HasForeignKey(rpp => rpp.RoleId);

        //        modelBuilder.Entity<RoleProgramPermission>()
        //            .HasOne(rpp => rpp.Program)
        //            .WithMany(p => p.RoleProgramPermissions)
        //            .HasForeignKey(rpp => rpp.ProgramCode);

        //        modelBuilder.Entity<RoleProgramPermission>()
        //            .HasOne(rpp => rpp.Permission)
        //            .WithMany(p => p.RoleProgramPermissions)
        //            .HasForeignKey(rpp => rpp.PermissionId);


        ////        modelBuilder.Entity<RoleProgramPermission>()
        ////.HasKey(rpp => new { rpp.RoleId, rpp.ProgramCode, rpp.PermissionId, rpp.VersionId });

        ////        modelBuilder.Entity<RoleProgramPermission>()
        ////            .HasOne(rpp => rpp.Version)
        ////            .WithMany()
        ////            .HasForeignKey(rpp => rpp.VersionId)
        ////            .OnDelete(DeleteBehavior.Restrict);


        //        //modelBuilder.Entity<RoleProgramPermission>(entity =>
        //        //{
        //        //    // 복합키 정의 (버전까지 포함)
        //        //    entity.HasKey(rpp => new { rpp.RoleId, rpp.ProgramCode, rpp.PermissionId, rpp.VersionId });

        //        //    // Role 관계
        //        //    entity.HasOne(rpp => rpp.Role)
        //        //        .WithMany(r => r.RoleProgramPermissions)
        //        //        .HasForeignKey(rpp => rpp.RoleId)
        //        //        .OnDelete(DeleteBehavior.Restrict);

        //        //    // Program 관계
        //        //    entity.HasOne(rpp => rpp.Program)
        //        //        .WithMany(p => p.RoleProgramPermissions)
        //        //        .HasForeignKey(rpp => rpp.ProgramCode)
        //        //        .OnDelete(DeleteBehavior.Restrict);

        //        //    // Permission 관계
        //        //    entity.HasOne(rpp => rpp.Permission)
        //        //        .WithMany(p => p.RoleProgramPermissions)
        //        //        .HasForeignKey(rpp => rpp.PermissionId)
        //        //        .OnDelete(DeleteBehavior.Restrict);

        //        //    // ProgramVersion (버전) 관계
        //        //    entity.HasOne(rpp => rpp.Version)
        //        //        .WithMany()
        //        //        .HasForeignKey(rpp => rpp.VersionId)
        //        //        .OnDelete(DeleteBehavior.Restrict);
        //        //});



        //        modelBuilder.Entity<UserProgramPeriod>()
        //.HasKey(upp => new { upp.UserId, upp.ProgramCode });  // 복합 키 지정

        //        modelBuilder.Entity<UserProgramPeriod>()
        //            .HasOne(upp => upp.User)
        //            .WithMany() // or WithMany(u => u.UserProgramPeriods) if collection exists
        //            .HasForeignKey(upp => upp.UserId)
        //            .OnDelete(DeleteBehavior.Restrict); // 필요 시 삭제 동작 설정

        //        modelBuilder.Entity<UserProgramPeriod>()
        //            .HasOne(upp => upp.Program)
        //            .WithMany() // or WithMany(p => p.UserProgramPeriods)
        //            .HasForeignKey(upp => upp.ProgramCode)
        //            .OnDelete(DeleteBehavior.Restrict);
        //    }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Users
            modelBuilder.Entity<User>()
                .ToTable("Users")
                .HasKey(u => u.UserId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProgramVersionEntity>()
    .HasKey(v => v.VersionId);

            modelBuilder.Entity<ProgramVersionEntity>()
                .Property(v => v.VersionName).IsRequired();

            modelBuilder.Entity<ProgramVersionEntity>()
                .HasOne(v => v.Program)
                .WithMany(p => p.Versions)
                .HasForeignKey(v => v.ProgramId)
                .OnDelete(DeleteBehavior.Cascade);

            // Roles
            modelBuilder.Entity<Role>()
                .HasKey(r => r.RoleId);

            // Programs
            modelBuilder.Entity<ProgramEntity>()
                .HasKey(p => p.ProgramId);

            modelBuilder.Entity<ProgramEntity>()
                .Property(p => p.ProgramName)
                .IsRequired();

            // Permissions
            modelBuilder.Entity<Permission>()
                .HasKey(p => p.PermissionId);

            // RoleProgramPermissions
            modelBuilder.Entity<RoleProgramPermission>()
                .HasKey(rpp => new { rpp.RoleId, rpp.ProgramId, rpp.PermissionId });

            modelBuilder.Entity<RoleProgramPermission>()
                .HasOne(rpp => rpp.Role)
                .WithMany(r => r.RoleProgramPermissions)
                .HasForeignKey(rpp => rpp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RoleProgramPermission>()
                .HasOne(rpp => rpp.Program)
                .WithMany(p => p.RoleProgramPermissions)
                .HasForeignKey(rpp => rpp.ProgramId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RoleProgramPermission>()
                .HasOne(rpp => rpp.Permission)
                .WithMany(p => p.RoleProgramPermissions)
                .HasForeignKey(rpp => rpp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserProgramPeriod
            modelBuilder.Entity<UserProgramPeriod>()
                .HasKey(upp => new { upp.UserId, upp.ProgramCode });

            modelBuilder.Entity<UserProgramPeriod>()
                .HasOne(upp => upp.User)
                .WithMany()
                .HasForeignKey(upp => upp.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserProgramPeriod>()
                .HasOne(upp => upp.Program)
                .WithMany()
                .HasForeignKey(upp => upp.ProgramCode)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
