using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientAPI.Models
{
    public enum Gender { MALE,FEMALE,TRANSGENDER}
    [Table("Patient")]
    public class Patient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Patient_Id")]
        public long PatientId { get; set; }
        [Column("SSN", TypeName ="varchar(12")]
        [RegularExpression("^[a-zA-Z0-9]{12}$", ErrorMessage = "SSN should be in 12 chars")]
        public string SSN {  get; set; }
        public FullName FullName { get; set; }
        [Column("Gender")]
        [EnumDataType(typeof(Gender))]
        public Gender Gender { get; set; }
      
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM dd yyyy}")]
        [Column("DOB")]
        public DateTime DOB { get; set; }

        [Column("Email")]
        [RegularExpression("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$",
            ErrorMessage = "Email Format Not matching")]
        [DefaultValue("")]
        public string Email { get; set; } = string.Empty;
        [Column("Phone")]
        [RegularExpression("^([+]\\d{2}[ ])?\\d{10}$",
            ErrorMessage = "Mobile No Format Not matching")]
        public long Phone { get; set; }

    }
}
