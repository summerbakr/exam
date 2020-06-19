using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exam.Models
{
    public class User {
            [Key]
            public int UserId { get; set; }
            // MySQL VARCHAR and TEXT types can be represeted by a string
            [Required]
            [MinLength(2)]
            public string FirstName { get; set; }
            [Required]
            [MinLength(2)]
            public string LastName { get; set; }
            [Required]
            [EmailAddress]
            public string Email { get; set; }
            [Required]
            [MinLength(8)]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        
            public DateTime CreatedAt {get;set;}=DateTime.Now;
            public DateTime UpdatedAt {get;set;}=DateTime.Now;
            [NotMapped]
            [Compare("Password")]
            [DataType(DataType.Password)]
            public string Confirm {get;set;}

            public List<Event> PlannedEvents{get;set;}

            public List<Participant> Attendees{get;set;}


        }
    }