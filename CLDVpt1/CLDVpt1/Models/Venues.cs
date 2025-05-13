﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using CLDVpt1.Models;


namespace CLDVpt1.Models
{

    public class Venues
    {
        [Key]
        public int VenueID { get; set; }
        public string? VenueName { get; set; }
        public string? Location { get; set; }
        public int Capacity { get; set; }
        public string? Description { get; set; }
        public string? ImageURL { get; set; }

        [NotMapped] // 👈 Add this to ignore in EF
        public IFormFile ImageFile { get; set; }

        public List<Events> Events { get; set; } = new();
    }
}
