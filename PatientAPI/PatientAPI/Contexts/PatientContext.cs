using Microsoft.EntityFrameworkCore;
using PatientAPI.Models;

namespace PatientAPI.Contexts
{
    public class PatientContext:DbContext
    {
        public PatientContext(DbContextOptions<PatientContext> options) : base(options) { 
        
           Database.EnsureCreated();
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Address> Addresses { get; set; }


    }
}
