using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using beltExam.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;


namespace beltExam.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private MyContext _context;

        public HomeController(ILogger<HomeController> logger, MyContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        //==============LOGIN & REGISTER====================

        
        [HttpPost("register")]
        public IActionResult Register(User newUser)
        {
            if(ModelState.IsValid)
            {
                if(_context.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email is already in use!");
                    return View("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                _context.Add(newUser);
                _context.SaveChanges();
                HttpContext.Session.SetInt32("loggedInUser", newUser.UserId);
                return RedirectToAction("Dashboard");
            }else{
                return View("Index");
            }
        }

        [HttpPost("login")]
        public IActionResult Login(LogUser logUser)
        {
            if(ModelState.IsValid)
            {
                User userindb = _context.Users.FirstOrDefault(u => u.Email == logUser.LogEmail);
                if(userindb == null)
                {
                    ModelState.AddModelError("LogEmail", "Invalid login attempt");
                    return View("Index");
                }
                PasswordHasher<LogUser> hasher = new PasswordHasher<LogUser>();
                var result = hasher.VerifyHashedPassword(logUser, userindb.Password, logUser.LogPassword);
                if(result == 0)
                {
                    ModelState.AddModelError("LogEmail", "Invalid login attempt");
                    return View("Index");
                }
                HttpContext.Session.SetInt32("loggedInUser", userindb.UserId);
                return RedirectToAction("Dashboard");
            }else{
                return View("Index");
            }
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
        //==============LOGIN & REGISTER====================^^

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            if(HttpContext.Session.GetInt32("loggedInUser") == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.AllActivities = _context.DojoActivities.Include(f => f.Coordinator).Include(d => d.Atendees).ThenInclude(g => g.Atendee).OrderBy(s => s.ActivityTime).ToList();
            ViewBag.LoggedInUser = _context.Users.FirstOrDefault(d => d.UserId == (int)HttpContext.Session.GetInt32("loggedInUser"));
            return View();
        }

        [HttpGet("newActivity")]
        public IActionResult NewActivity()
        {
            if(HttpContext.Session.GetInt32("loggedInUser") == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost("addActivity")]
        public IActionResult AddActivity(DojoActivity newActivity)
        {
            if(HttpContext.Session.GetInt32("loggedInUser") == null)
            {
                return RedirectToAction("Index");
            }
            if(ModelState.IsValid)
            {
                newActivity.UserId = (int)HttpContext.Session.GetInt32("loggedInUser");
                int activityid = newActivity.DojoActivityId;
                _context.Add(newActivity);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }else {
                return View("NewActivity");
            }
        }

        [HttpGet("activity/{activityId}")]
        public IActionResult OneActivity(int activityId)
        {
            if(HttpContext.Session.GetInt32("loggedInUser") == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.LoggedInUser = _context.Users.FirstOrDefault(d => d.UserId == (int)HttpContext.Session.GetInt32("loggedInUser"));
            DojoActivity oneActivity = _context.DojoActivities.Include(c => c.Coordinator).Include(f => f.Atendees).ThenInclude(f => f.Atendee).FirstOrDefault(p => p.DojoActivityId == activityId);
            return View(oneActivity);
        }

        [HttpGet("join/{activityId}/{userId}")]
        public IActionResult Join(int activityId, int userId)
        {
            if(HttpContext.Session.GetInt32("loggedInUser") == null)
            {
                return RedirectToAction("Index");
            }
            if(HttpContext.Session.GetInt32("loggedInUser") != userId)
            {
                return RedirectToAction("Logout");
            }
            Attendance attend = new Attendance();
            attend.DojoActivityId = activityId;
            attend.UserId = userId;
            _context.Add(attend);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("leave/{activityId}/{userId}")]
        public IActionResult UnlikePost(int activityId, int userId)
        {
            if(HttpContext.Session.GetInt32("loggedInUser") == null)
            {
                return RedirectToAction("Index");
            }
            if(HttpContext.Session.GetInt32("loggedInUser") != userId)
            {
                return RedirectToAction("Logout");
            }
            Attendance AttendanceToRemove = _context.Attendance.FirstOrDefault(d => d.DojoActivityId == activityId && d.UserId == userId);
            _context.Attendance.Remove(AttendanceToRemove);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("delete/{ActivityId}")]
        public IActionResult Delete(int ActivityId)
        {
            DojoActivity ActivityToDelete = _context.DojoActivities.FirstOrDefault(p => p.DojoActivityId == ActivityId);
            if(HttpContext.Session.GetInt32("loggedInUser") != ActivityToDelete.UserId)
            {
                return RedirectToAction("Logout");
            }
            _context.DojoActivities.Remove(ActivityToDelete);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
