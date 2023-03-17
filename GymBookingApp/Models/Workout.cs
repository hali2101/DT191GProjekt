using System;
using System.ComponentModel.DataAnnotations;

namespace bookingadmintest.Models
{
    public class Workout
    {
        //properties
        public int Id { get; set; }

        //sätter fältet som nödvändigt vid ifyllnad och med felmeddelande
        [Required(ErrorMessage = "Ange ett namn för träningspasset.")]
        [Display(Name = "Träningsform")]
        public string? WorkoutName { get; set; }

        [Required(ErrorMessage = "Ange datum och tid för träningspasset.")]
        [Display(Name = "Datum/Tid")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime? WorkoutDate { get; set; }

        [Required(ErrorMessage = "Ange instruktör.")]
        [Display(Name = "Instruktör")]
        public string? Instructor { get; set; }

        [Required(ErrorMessage = "Ange längd på passet.")]
        [Display(Name = "Längd")]
        public int? Duration { get; set; }

        [Required(ErrorMessage = "Ange pris.")]
        [Display(Name = "Pris")]
        public int? Cost { get; set; }

        [Required(ErrorMessage = "Ange antal.")]
        [Display(Name = "Max antal platser")]
        public int? Quantity { get; set; }

        [Display(Name = "Platser kvar")]
        public int? BookedQuantity { get; set; }

    }
}
