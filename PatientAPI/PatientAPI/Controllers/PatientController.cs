using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PatientAPI.Models;
using PatientAPI.Repositories;

namespace PatientAPI.Controllers
{

    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [EnableCors]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientRepo _patientRepo;

        public PatientController(IPatientRepo patientRepo)
        {
            _patientRepo = patientRepo;
        }

        // GET: api/Policies
        [HttpGet]
        public async Task<IEnumerable<Patient>> GetPatients()
        {
            return await _patientRepo.GetAllPatients();
        }

        // GET: api/Policies/5
        [HttpGet("{patientId}")]
        public async Task<ActionResult<Patient>> GetPatient(long patientId)
        {
            var patient = await _patientRepo.GetPatient(patientId);

            if (patient == null)
            {
                return NotFound();
            }

            return patient;
        }



        // POST: api/Patients
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Patient>> PostPatient([FromBody] Patient patient)
        {
            var result = await _patientRepo.AddPatient(patient);

            return CreatedAtAction("GetPatients", new { id = result.PatientId }, result);
        }

        // DELETE: api/Patients/5
        [HttpDelete("{patientId}")]
        public async Task<IActionResult> DeletePatient(long patientId)
        {
            if (await _patientRepo.DeletePatient(patientId))
            {
                return new OkResult();
            }
            else

                return NoContent();
        }



    }
}
