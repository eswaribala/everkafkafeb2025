using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ConfluentPatientInfoConsumer.DTOs
{
    public enum Gender { MALE,FEMALE,TRANSGENDER}
    public class Patient
    {
        
        public long PatientId { get; set; }
        public string SSN { get; set; }
        public FullName FullName { get; set; }
       
        public Gender Gender { get; set; }

       
        public DateTime DOB { get; set; }

       
        public string Email { get; set; } = string.Empty;
       
        public long Phone { get; set; }
    }
}
