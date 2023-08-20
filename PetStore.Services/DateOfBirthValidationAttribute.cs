using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetStore.Services
{
    public class DateOfBirthValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            DateTime dateTime = Convert.ToDateTime(value);
            return dateTime <= DateTime.Now;
        }

        // Started working on validation for duplicate name + dob records

        //protected override ValidationResult IsValid(DateTime value,
        //    ValidationContext validationContext)
        //{
        //    var context = (PetStoreContext)validationContext.GetService(typeof(PetStoreContext));
        //    if (!context.Pets.Any(a => a.DateOfBirth == value))
        //    {
        //        return ValidationResult.Success;
        //    }
        //    return new ValidationResult("DoB exists");
        //}
    }
}
