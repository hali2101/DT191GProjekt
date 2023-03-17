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
    [Authorize(Roles = "Member")]
    public class UserBookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserBookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("/minabokningar")]
        // GET: Book
        public async Task<IActionResult> Index()
        {
            //hämtar in userid
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //sorterar efter datum
            var applicationDbContext = _context.Bookings.OrderBy(m => m.Workout.WorkoutDate).Include(b => b.Workout)
                .Where(b => b.UserId == userId)
                .ToListAsync();

            return View(await applicationDbContext);
        }

        [Route("/minabokningar/detaljer")]
        // GET: Book/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Bookings == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Workout)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        [Route("/minabokningar/avboka")]
        // GET: Book/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Bookings == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Workout)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        [Route("/minabokningar/avboka")]
        // POST: Book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Bookings == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Bookings'  is null.");
            }
            var booking = await _context.Bookings.FindAsync(id);
            // Kontrollera om workout finns och hämta in specifik workout
            var workout = await _context.Workouts.FindAsync(booking.WorkoutId);

            //kontroll så att antalet inte blir högre än maxantalet
            if (booking != null)
            {
                //lägger till en plats vid bokning och spara
                workout.BookedQuantity = workout.BookedQuantity + 1;
                //skickar avbokningsmail
                //SendEmail(workout.WorkoutName, workout.WorkoutDate);
                //tar bort bokningen
                _context.Bookings.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return (_context.Bookings?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
