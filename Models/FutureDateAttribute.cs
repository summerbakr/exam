using System;
using System.ComponentModel.DataAnnotations;

public class FutureDateAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if ((DateTime)value < DateTime.Now)
        {
            return new ValidationResult("Your activity date must be in the future!");
        
        }
    return ValidationResult.Success;
}
}