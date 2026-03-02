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
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<AppMenu> AppMenus { get; set; }
        public DbSet<EmployeeTransfer> EmployeeTransfers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
            modelBuilder.Entity<Department>()
                .HasMany(d => d.Employees)
                .WithOne(e => e.Department)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict); 


            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Contracts)
                .WithOne(c => c.Employee)
                .HasForeignKey(c => c.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade); 

  
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Manager)
                .WithMany() 
                .HasForeignKey(e => e.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Department>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<EmploymentContract>().HasQueryFilter(x => !x.IsDeleted);

            base.OnModelCreating(modelBuilder);
        }

       
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
