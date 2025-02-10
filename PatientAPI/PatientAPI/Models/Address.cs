using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PatientAPI.Models
{
    [Table("Address")]
    public class Address
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Address_Id")]
        public long AddressId { get; set; }
        [Column("Door_No")]
        public string DoorNo { get; set; }
        [Column("Street_Name")]
        public string StreetName { get; set; }
        [Column("City")]
        public string City { get; set; }
        [Column("State")]
        public string State { get; set; }
        [Column("Country")]
        public string Country { get; set; }
        [Column("Postal_Code")]
        public long PostalCode { get; set; }


        [ForeignKey("Patient")]
        [Column("Patient_Id_FK")]
        public long PatientId { get; set; }

        public Patient Patient { get; set; }    

    }
}
