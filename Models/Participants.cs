using System;
using System.ComponentModel.DataAnnotations;

namespace Exam.Models{

    public class Participant
    {
        [Key]
        public int ParticipantId{get;set;}

        public int UserId{get;set;}

        public int EventId{get;set;}

        public User Planner{get;set;}

        public Event EventAttending{get;set;}

        public DateTime CreatedAt{get;set;}=DateTime.Now;

        public DateTime UpdatedAt{get;set;}=DateTime.Now;

    }
}