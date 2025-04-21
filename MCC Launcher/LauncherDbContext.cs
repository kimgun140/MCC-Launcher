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
        public DbSet<UserInfo> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<ProgramsEntity> Programs { get; set; }
        public DbSet<ProgramVersionEntity> ProgramVersion { get; set; }
        public DbSet<RoleProgramPermission> RoleProgramPermissions { get; set; }
        //public DbSet<LauncherInfo> Launchers { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=launcherdb;Username=postgres;Password=1234");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //  UserInfo → 기존 'Users' 테이블로 매핑
            modelBuilder.Entity<UserInfo>().ToTable("Users");
            modelBuilder.Entity<UserInfo>().HasKey(u => u.UserId);

            //  Program 관계
            modelBuilder.Entity<ProgramsEntity>().HasKey(p => p.ProgramCode);
            modelBuilder.Entity<ProgramVersionEntity>().HasKey(v => v.VersionId);
            modelBuilder.Entity<ProgramVersionEntity>()
                .HasOne(v => v.Program)
                .WithMany(p => p.Versions) // Ensure the navigation property name matches the model.  
                .HasForeignKey(v => v.ProgramCode);

            // UserRole 관계
            modelBuilder.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            //  RolePermission 관계
            modelBuilder.Entity<RolePermission>().HasKey(rp => new { rp.RoleId, rp.PermissionID });
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId);
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionID);



            modelBuilder.Entity<RoleProgramPermission>()
    .HasKey(rpp => new { rpp.RoleId, rpp.ProgramCode, rpp.PermissionId });

            modelBuilder.Entity<RoleProgramPermission>()
                .HasOne(rpp => rpp.Role)
                .WithMany(r => r.RoleProgramPermissions)
                .HasForeignKey(rpp => rpp.RoleId);

            modelBuilder.Entity<RoleProgramPermission>()
                .HasOne(rpp => rpp.Program)
                .WithMany(p => p.RoleProgramPermissions)
                .HasForeignKey(rpp => rpp.ProgramCode);

            modelBuilder.Entity<RoleProgramPermission>()
                .HasOne(rpp => rpp.Permission)
                .WithMany(p => p.RoleProgramPermissions)
                .HasForeignKey(rpp => rpp.PermissionId);
        }
    }
}
