using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Exam.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Exam.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;

        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [Route("processuser")]
        public IActionResult ProcessUser(User user)
        {
        if(ModelState.IsValid)
            {
        // If a User exists with provided email
                if(dbContext.Users.Any(u => u.Email == user.Email))
        {
            // Manually add a ModelState error to the Email field, with provided
            // error message
            ModelState.AddModelError("Email", "Email already in use, must use unique email!");
            
            return View("Index");
        }
        else{
            PasswordHasher<User> Hasher = new PasswordHasher<User>();
            user.Password = Hasher.HashPassword(user, user.Password);
            dbContext.Users.Add(user);
            dbContext.SaveChanges();
            HttpContext.Session.SetInt32("UserId", user.UserId);
            return Redirect($"dashboard/{user.UserId}");
            }
        }
        return View("Index");

        }
        [HttpGet]
        [Route("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet ("logout")]
        public IActionResult Logout ()
        {
            HttpContext.Session.Clear ();
            return RedirectToAction ("Index");
        }

        [HttpPost]
        [Route("processlogin")]

        public IActionResult ProcessLogin(LoginUser login)
        {
            if(ModelState.IsValid)
            {
            // If inital ModelState is valid, query for a user with provided email
            var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == login.LoginEmail);
            // If no user exists with provided email
            if(userInDb == null)
            {
                // Add an error to ModelState and return to View!
                ModelState.AddModelError("LoginEmail", "That email is invalid! Try again or register!");
                return View("Login");
            }
            else {

            // Initialize hasher object
            var hasher = new PasswordHasher<LoginUser>();
            
            // verify provided password against hash stored in db
            var result = hasher.VerifyHashedPassword(login, userInDb.Password, login.LoginPassword);
            
            // result can be compared to 0 for failure
            if(result == 0)
            {
                ModelState.AddModelError("LoginPassword", "Incorrect password!");
                return View("Login");
            }
            HttpContext.Session.SetInt32("UserId", userInDb.UserId);
            return Redirect($"dashboard/{userInDb.UserId}");

            }
    
        }
        return View("Login");

        }

        [HttpGet]
        [Route("dashboard/{userId}")]

        public IActionResult Dashboard(int userId)
        {
             if (HttpContext.Session.GetInt32("UserId")==null)
            {
                return Redirect("/login");
            }
            var loggedinuser=dbContext.Users.FirstOrDefault(u=>u.UserId==userId);
            ViewBag.AllEvents=dbContext.Events.Include(e=>e.Creator)
                                                .Include(e=>e.Participants)
                                                .OrderBy(e=>e.Date)
                                                .ToList();

            return View(loggedinuser);

        }

        [HttpGet]
        [Route("new")]

        public IActionResult NewEvent()
        {
            if (HttpContext.Session.GetInt32("UserId")==null)
            {
                return Redirect("/login");
            }
            return View();
        }

        [HttpPost]
        [Route("processevent")]

        public IActionResult ProcessEvent(Event newevent)
        {
        
            
            if (ModelState.IsValid)
            {
               int? userid=HttpContext.Session.GetInt32("UserId");
               List<Event> myEvents=dbContext.Users
                                            .Include(u=>u.PlannedEvents)
                                            .FirstOrDefault(u=>u.UserId== (int)userid)
                                            .PlannedEvents.ToList();

                if (isBooked(newevent, myEvents ))
                {
                        ModelState.AddModelError ("Date", "You are already at an event at this time!");
                        return View ("NewEvent");
                    }

               
                    newevent.UserId=(int)userid;
                    dbContext.Events.Add(newevent);
                    dbContext.SaveChanges();

                    return Redirect($"dashboard/{userid}"); 
                }
            else{
                return View("NewEvent");  
            }
            
        }
         public static bool isBooked (Event newevent, List<Event> events)
        {
            DateTime eventDate = newevent.Date;
            DateTime eventTime = newevent.Time;
            foreach (var e in events)
            {
                DateTime otherDate = e.Date;
                DateTime otherTime= e.Time;
                
                if (eventDate == otherDate && eventTime == otherTime)
                {
                    return true;
                }
            }
            return false;
        }

        [HttpGet]
        [Route("delete/{eventId}")]

        public IActionResult DeleteEvent(int eventId)
        {
            int? userId=HttpContext.Session.GetInt32("UserId");
            var eventtodelete=dbContext.Events.FirstOrDefault(e=>e.EventId==eventId);
            dbContext.Events.Remove(eventtodelete);
            dbContext.SaveChanges();

            return Redirect($"/dashboard/{userId}");


        }

        [HttpGet]
        [Route("join/{eventId}/{userId}")]

        public IActionResult JoinEvent(int eventId, int userId)
        {
            Participant newparticipant=new Participant();
            newparticipant.EventId=eventId;
            newparticipant.UserId=userId;
            dbContext.Participants.Add(newparticipant);
            dbContext.SaveChanges();
            return Redirect($"/dashboard/{userId}");

        }

        [HttpGet]
        [Route("leave/{eventId}/{userId}")]

        public IActionResult LeaveEvent(int eventId, int userId)
        {
            Participant leavingparticipant=dbContext.Participants.FirstOrDefault(a=>a.EventId==eventId && a.UserId==userId); 
            dbContext.Participants.Remove(leavingparticipant);
            dbContext.SaveChanges();
            return Redirect($"/dashboard/{userId}");

        }

        [HttpGet]
        [Route("displayevent/{eventId}")]

        public IActionResult DisplayEvent(int eventId)
        {
             if (HttpContext.Session.GetInt32("UserId")==null)
            {
                return Redirect("/login");
            }
            ViewBag.EventToDisplay=dbContext.Events.Include(e=>e.Creator)
                                                    .Include(e=>e.Participants)
                                                    .ThenInclude(a=>a.Planner)
                                                    .FirstOrDefault(e=>e.EventId==eventId);
            int? userId=HttpContext.Session.GetInt32("UserId");
            var loggedinuser=dbContext.Users.FirstOrDefault(u=>u.UserId==userId);
            return View(loggedinuser);

        }




        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
