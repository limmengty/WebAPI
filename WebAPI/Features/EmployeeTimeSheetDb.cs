using Microsoft.EntityFrameworkCore;
using WebAPI.Entities;
namespace WebAPI.Features
{
    public class EmployeeTimeSheetDb : DbContext
    {
        public EmployeeTimeSheetDb(DbContextOptions<EmployeeTimeSheetDb> options): base(options) { }
        public DbSet<Employee> Employees => Set<Employee>();


    }
}
