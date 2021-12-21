using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace beltExam.Models
{
    public class DojoActivity
    {
        [Key]
        public int DojoActivityId {get;set;}
        [Required]
        public string Title {get;set;}
        [Required]
        public string Description {get;set;}
        [Required]
        [DataType(DataType.Time)]
        public DateTime ActivityTime {get;set;}
        [Required]
        [NoPastDateTime]
        [DataType(DataType.Date)]
        public DateTime ActivityDate {get;set;}
        [Required]
        public int Duration {get;set;}
        [Required]
        public string DurationUnit {get;set;}
        public int UserId {get;set;}
        public User Coordinator {get;set;}
        public List<Attendance> Atendees {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }

    public class NoPastDateTime : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value is DateTime)
            {
                DateTime checkMe = (DateTime)value;

                //acctual logic of determing validity
                if(checkMe < DateTime.Now)
                {
                    return new ValidationResult("Please select a future date!");
                }
                else
                {
                    return ValidationResult.Success;
                }
            }
            else
            {
                return new ValidationResult("Thats not Even a Date Time!");
            }
        }
    }

}