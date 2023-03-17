using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymBookingApp.Data;
using bookingadmintest.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Net;
using System.Net.Mail;

namespace GymBookingApp.Controllers
{
    
    //befognhet att se endast för medlemmar
    [Authorize(Roles = "Member")]
    public class UserWorkoutController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserWorkoutController(ApplicationDbContext context)
        {
            _context = context;
        }

        //Befogenhet för alla att se alla träningspass
        [AllowAnonymous]
        [Route("/traningspass")]
        // GET: User
        public async Task<IActionResult> Index(string workoutName)
        {
            //hämtar in och lagrar workouts i variable
            var workout = from w in _context.Workouts select w;

            //kontroll om det finns något i tabllen för träningspass
            if (!workout.Any())
            {
                ViewBag.NoWorkouts = "Inga träningspass finns inlagda.";
            }

            //om strängen inte är tom
            if (!String.IsNullOrEmpty(workoutName))
            {
                //sortera resultatet efter medskickad parameter
                workout = workout.Where(w => w.WorkoutName.Contains(workoutName));
                return View(workout.OrderBy(m => m.WorkoutDate));
            }

            return _context.Workouts != null ?
            View(await _context.Workouts.OrderBy(m => m.WorkoutDate).ToListAsync()) :
            Problem("Entity set 'ApplicationDbContext.Workouts'  is null.");

        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Workouts == null)
            {
                return NotFound();
            }

            var workout = await _context.Workouts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workout == null)
            {
                return NotFound();
            }

            return View(workout);
        }

        [Route("/boka")]
        // GET: Book/Create
        public async Task<IActionResult> Create(int? id)
        {
            if (id == null || _context.Workouts == null)
            {
                return NotFound();
            }

            var workout = await _context.Workouts.FindAsync(id);

            if (workout == null)
            {
                return NotFound();
            }

            //hämtar in id för inloggad användare
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ViewData["WorkoutId"] = workout.Id;
            ViewData["UserId"] = userId;
            ViewData["Workout"] = workout;

            return View();
        }

        [Route("/boka")]
        // POST: Book/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingDate, UserId, WorkoutId")] Booking booking)
        {
            // Kontrollera om workout finns och hämta in specifik workout med id
            var workout = await _context.Workouts.FindAsync(booking.WorkoutId);

            //hämtar in id för inloggad användare
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ModelState.IsValid)
            {
                //kontroll om workout inte är tom
                if ((workout != null && workout.BookedQuantity != 0))
                {
                    _context.Add(booking);
                    //ta bort en plats vid bokning och spara
                    workout.BookedQuantity = workout.BookedQuantity - 1;
                    //skicka mailbekräftelse
                    //SendEmail(workout.WorkoutName, workout.WorkoutDate, workout.Cost);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            return _context.Workouts != null ?
            View(await _context.Workouts.OrderBy(m => m.WorkoutDate).ToListAsync()) :
            Problem("Entity set 'ApplicationDbContext.Workouts'  is null.");

        }
    }
}
