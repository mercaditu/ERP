using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace entity.Class
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class EntityValidation : ValidationAttribute
    {
        public static ValidationResult CheckId(int id)
        {
            if (id > 0)
                return ValidationResult.Success;
            else
                return new ValidationResult("Invalid Id");
        }
    }
}
