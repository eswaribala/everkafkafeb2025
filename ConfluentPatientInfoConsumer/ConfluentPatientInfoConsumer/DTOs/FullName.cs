using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ConfluentPatientInfoConsumer.DTOs
{
    public class FullName
    {
        
        public string FirstName { get; set; }

       
        public string MiddleName { get; set; }
        
        public string LastName { get; set; }
    }
}
