using PatientAPI.Models;

namespace PatientAPI.Repositories
{
    public interface IPatientRepo
    {
        Task<Patient> AddPatient(Patient Patient);

        Task<bool> DeletePatient(long patientId);
        Task<Patient> GetPatient(long patientId);
        Task<IEnumerable<Patient>> GetAllPatients();
    }
}
