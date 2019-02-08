using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LocalTheatre.Models
{
    public class Announcements
    {
        [Key]
        public int AnnouncementId { get; set; }

        [Required(ErrorMessage = "Add an announcement title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Add announcement content")]
        public string Announcement { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        [Required]
        [Display(Name = "Category")]
        public string Category { get; set; }

        [Display(Name = "Author")]
        public string Author { get; set; }
    }
}