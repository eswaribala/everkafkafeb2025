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

        public Task<bool> DeletePatient(long patientId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Patient>> GetAllPatients()
        {
            throw new NotImplementedException();
        }

        public Task<Patient> GetPatient(long patientId)
        {
            throw new NotImplementedException();
        }
    }
}
