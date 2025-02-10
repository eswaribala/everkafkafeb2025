using Microsoft.EntityFrameworkCore;
using PatientAPI.Contexts;
using PatientAPI.Models;

namespace PatientAPI.Repositories
{
    public class PatientRepo : IPatientRepo
    {
        private PatientContext _dbContext;

        public PatientRepo(PatientContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Patient> AddPatient(Patient Patient)
        {
            
                var result = await this._dbContext.Patients.AddAsync(Patient);
                await this._dbContext.SaveChangesAsync();
                return result.Entity;

            
        }
        private async Task<Patient> IsPatientExists(long patientId)
        {

            return await this._dbContext.Patients.FirstOrDefaultAsync(p=>p.PatientId == patientId); 

        }
        public async Task<bool> DeletePatient(long patientId)
        {
            bool Status = false;
            var result = await IsPatientExists(patientId);
            if (result != null)
            {
                this._dbContext.Patients.Remove(result);
                await this._dbContext.SaveChangesAsync();
                Status = true;
            }

            return Status;
        }

        public async Task<IEnumerable<Patient>> GetAllPatients()
        {
            return await this._dbContext.Patients.ToListAsync();
               
        }

        public async Task<Patient> GetPatient(long patientId)
        {
            return await IsPatientExists(patientId);
        }
    }
}
