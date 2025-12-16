using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronos.Domain.Entity;
using Chronos.Domain.Common;
using Chronos.Domain.Entities;
using Chronos.Domain.Entity.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Chronos.Persistence.Context
{
    public class ChronosDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ChronosDbContext(DbContextOptions<ChronosDbContext> options) : base(options)
        {
        }

        // Khai báo các bảng
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<EmploymentContract> EmploymentContracts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. Cấu hình mối quan hệ (Fluent API)

            // Department - Employee (1-N)
            modelBuilder.Entity<Department>()
                .HasMany(d => d.Employees)
                .WithOne(e => e.Department)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict); // Xóa phòng ban không được xóa nhân viên (tránh mất dữ liệu)

            // Employee - Contract (1-N)
            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Contracts)
                .WithOne(c => c.Employee)
                .HasForeignKey(c => c.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa nhân viên thì xóa luôn hợp đồng (hoặc Restrict tùy nghiệp vụ)

            // Self-Referencing: Manager - Staff (1-N)
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Manager)
                .WithMany() // Manager có nhiều nhân viên (nhưng trong Entity ta chưa khai báo List<Employee> Subordinates nên để trống)
                .HasForeignKey(e => e.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // 2. Cấu hình Global Query Filter cho Soft Delete
            // Tự động bỏ qua những bản ghi có IsDeleted = true khi query
            modelBuilder.Entity<Employee>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Department>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<EmploymentContract>().HasQueryFilter(x => !x.IsDeleted);

            base.OnModelCreating(modelBuilder);
        }

        // Tự động cập nhật CreatedAt và LastModifiedAt
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.IsDeleted = false;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedAt = DateTime.UtcNow;
                        break;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
