using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace beltExam.Models
{ 
    public class User 
    {
        [Key]
        public int UserId {get;set;}

        [Required]
        [MinLength(2)]
        public string Name {get;set;}

        [Required]
        [EmailAddress]
        public string Email {get;set;}

        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        [PasswordValidation]
        public string Password {get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
        public List<DojoActivity> ActivitiesOrganized {get;set;}
        public List<Attendance> ActivitiesJoined {get;set;}

        [NotMapped]
        [Required]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string Confirm {get;set;}

    }

    public class PasswordValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var input = (string)value;
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(input))
            {
                throw new Exception("Password should not be empty");
            }

            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");

            if (!hasLowerChar.IsMatch(input))
            {
                return new ValidationResult("Password should be a min length of 8 characters, contain at least 1 number, 1 letter, and a special character.");
            }
            if (!hasUpperChar.IsMatch(input))
            {
                return new ValidationResult("Password should be a min length of 8 characters, contain at least 1 number, 1 letter, and a special character.");
            }
            if (!hasNumber.IsMatch(input))
            {
                return new ValidationResult("Password should be a min length of 8 characters, contain at least 1 number, 1 letter, and a special character.");
            }

            if (!hasSymbols.IsMatch(input))
            {
                return new ValidationResult("Password should be a min length of 8 characters, contain at least 1 number, 1 letter, and a special character.");
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
}