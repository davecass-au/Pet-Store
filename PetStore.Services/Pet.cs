using System.ComponentModel.DataAnnotations;

namespace PetStore.Services
{
    public class Pet
    {
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = String.Empty;
                
        public PetType Type { get; set; }
        
        [Required]
        [DataType(DataType.Date)]
        [DateOfBirthValidation]
        [Display(Name = "Date Of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Range(0, 100)]
        [Display(Name = "Weight (kg)")]
        public Double Weight { get; set; }
                
    }
}
