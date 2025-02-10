using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PatientAPI.Repositories;
using System.Text.Json;

namespace PatientAPI.Controllers
{
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [EnableCors]
    [ApiController]
    public class PatientInfoPublishController : ControllerBase
    {

        private IPatientPublishRepo _patientPublishRepo;
        private IPatientRepo _patientRepo;

        private IConfiguration _configuration;


        public PatientInfoPublishController(IPatientPublishRepo patientPublishRepo,
            IConfiguration configuration, IPatientRepo patientRepo)
        {
            _patientPublishRepo = patientPublishRepo;
            this._configuration = configuration;
            _patientRepo = patientRepo;
        }

        [HttpPost]
        [Route("publish")]
        public async Task<IActionResult> PublishData()
        {
            
            var data = await _patientRepo.GetAllPatients();
            var Message = JsonSerializer.Serialize(data);
            return Ok(await _patientPublishRepo
                .PublishMessage(Message, _configuration));


        }
    }
}
