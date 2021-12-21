using System.ComponentModel.DataAnnotations;
namespace beltExam.Models
{
    public class Attendance
    {
        [Key]
        public int AttendanceId {get;set;}
        public int UserId {get;set;}
        public int DojoActivityId {get;set;}
        public User Atendee {get;set;}
        public DojoActivity DojoActivity {get;set;}
    }
}