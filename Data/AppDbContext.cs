using Microsoft.EntityFrameworkCore;
using VacationManagement.Models;

namespace VacationManagement.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<SpGetReportVacationPlans>().HasNoKey().ToSqlQuery("SpGetReportVacationPlans");
        }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        //public DbSet<EntityBase> EntityBases { get; set; }

        public DbSet<RequestVacation> RequestVacations { get; set; }

        public DbSet<VacationPlan> VacationPlans { get; set; }
        public DbSet<VacationType> VacationTypes { get; set; }
        public DbSet<SpGetReportVacationPlans> SpGetReportVacationPlans { get; set; }



    }
}
