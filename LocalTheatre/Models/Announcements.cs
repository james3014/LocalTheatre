using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

        [Display(Name = "Author")]
        public string Author { get; set; }

        public virtual ICollection<Comments> Comments { get; set; }

        [Required]
        [Display(Name = "Category")]
        public Category Category { get; set; }


        public static IEnumerable<SelectListItem> GetCategory()
        {
            yield return new SelectListItem { Text = "Announcement", Value = "Announcement" };
            yield return new SelectListItem { Text = "Event", Value = "Event" };
            yield return new SelectListItem { Text = "Review", Value = "Review" };
        }
    }

    public enum Category
    {
        Announcement,
        Event,
        Review
    }


}