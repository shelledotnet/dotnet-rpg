using dotnet_rpg.domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace dotnet_rpg.domain.Data
{
    //****Note register the EmployeeManagerDbContext in the IOC container
    public class EmployeeManagerDbContext : DbContext
    {
        public EmployeeManagerDbContext(DbContextOptions<EmployeeManagerDbContext> options)
            : base(options) { }

        public DbSet<Employee> Employees => Set<Employee>();

        public DbSet<Department> Departments => Set<Department>();


#if DEBUG
        #region Seed Data
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Department>().HasData(
                new Department { Id = 1, Name = "Finance" },
                 new Department { Id = 2, Name = "IT" },
                 new Department { Id = 3, Name = "Control" },
                  new Department { Id = 4, Name = "HR" },
                   new Department { Id = 5, Name = "Marketing" }
                );
            modelBuilder.Entity<Employee>().HasData(
                new Employee { Id = 1, FirstName = "Adeola", LastName = "Malian", IsDeveloper = false, DepartmentId = 2 },
                new Employee { Id = 2, FirstName = "Olaolu", LastName = "Mapayi", IsDeveloper = true, DepartmentId = 3 },
                new Employee { Id = 3, FirstName = "Deji", LastName = "Henry", IsDeveloper = false, DepartmentId = 4 },
                new Employee { Id = 4, FirstName = "Sheyi", LastName = "Alao", IsDeveloper = true, DepartmentId = 1 },
                new Employee { Id = 5, FirstName = "Opeola", LastName = "Bayo", IsDeveloper = false, DepartmentId = 5 }
                );

        }
        #endregion
#else
#endif


    }
}
