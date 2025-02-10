using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientAPI.Models
{
    [Owned]
    public class FullName
    {
        [Required]
        [RegularExpression("^[a-zA-Z]{3,25}$",ErrorMessage ="First Name should be between 3 to 25 chars")]
        [Column("First_Name", TypeName ="varchar(50)")]
        public string FirstName { get; set; }
       
        [Column("Middle_Name", TypeName = "varchar(50)")]
        public string MiddleName { get; set; }
        [Required]
        [RegularExpression("^[a-zA-Z]{3,25}$", ErrorMessage = "Last Name should be between 3 to 25 chars")]
        [Column("Last_Name", TypeName ="varchar(50)")]
        public string LastName { get; set; }
    }
}
