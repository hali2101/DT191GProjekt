using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace bookingadmintest.Models
{
    public class Booking
    {
        //properties

        public int Id { get; set; }

        [Display(Name = "Bokningsdatum")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime? BookingDate { get; set; } = DateTime.Now;

        [ForeignKey("AspNetUsersId")]
        public string? UserId { get; set; }
        public IdentityUser? User { get; set; }

        [Display(Name = "Träningspass")]
        [ForeignKey("Workout")]
        public int WorkoutId { get; set; }
        [Display(Name = "Träningspass")]
        public Workout? Workout { get; set; }

    }
}