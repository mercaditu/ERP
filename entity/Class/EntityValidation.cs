using System;
using System.ComponentModel.DataAnnotations;

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

        public static ValidationResult Checkbranch(app_branch app_branch_origin)
        {
            if (app_branch_origin != null)
                return ValidationResult.Success;
            else
                return new ValidationResult("Invalid branch");
        }
    }
}