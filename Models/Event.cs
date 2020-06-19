using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Exam.Models
{
    public class Event{
        [Key]
        public int EventId{get;set;}
        [Required]
        public string Title{get;set;}

        [Required]
        [DataType(DataType.Date)]
        [FutureDate]
        public DateTime Date{get;set;}

        [Required]
        [DataType(DataType.Time)]

        public DateTime Time{get;set;}

        public string Duration {get;set;}

        [Required]
        public string Description{get;set;}

        public int UserId {get;set;}

        public User Creator {get;set;}

        public List<Participant> Participants{get;set;}

    }
}