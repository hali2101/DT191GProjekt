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
using System.Net;
using System.Net.Mail;

namespace GymBookingApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminWorkoutController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminWorkoutController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("/admin/träningspass")]
        // GET: Admin
        public async Task<IActionResult> Index()
        {
            return _context.Workouts != null ?
                        View(await _context.Workouts.OrderBy(m => m.WorkoutDate).ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Workouts'  is null.");
        }

        [Route("/träningspass/detaljer")]
        // GET: Admin/Details/5
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

        [Route("/träningspass/skapa")]
        // GET: Admin/Create
        public IActionResult Create()
        {
            return View();
        }

        [Route("/träningspass/skapa")]
        // POST: Admin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,WorkoutName,WorkoutDate,Instructor,Duration,Cost,Quantity")] Workout workout)
        {
            if (ModelState.IsValid)
            {
                _context.Add(workout);
                //sätter quantity och bookedquantity lika
                workout.BookedQuantity = workout.Quantity;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(workout);
        }

        [Route("/träningspass/ändra")]
        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(int? id)
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
            return View(workout);
        }

        [Route("/träningspass/ändra")]
        // POST: Admin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,WorkoutName,WorkoutDate,Instructor,Duration,Cost,Quantity")] Workout workout)
        {
            if (id != workout.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workout);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkoutExists(workout.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(workout);
        }

        [Route("/träningspass/radera")]
        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        [Route("/träningspass/radera")]
        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workout = await _context.Workouts.FindAsync(id);

            if (workout != null)
            {
                _context.Workouts.Remove(workout);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkoutExists(int id)
        {
            return (_context.Workouts?.Any(e => e.Id == id)).GetValueOrDefault();
        }

    }
}
