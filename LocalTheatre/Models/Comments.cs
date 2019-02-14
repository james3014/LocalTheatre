using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LocalTheatre.Models
{
    public class Comments
    {
        [Key]
        public int CommentId { get; set; }

        [Display(Name = "Author")]
        public string CommentAuthor { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Required(ErrorMessage = "Enter a comment")]
        [Display(Name = " Comment")]
        [MaxLength(50)]
        public string CommentBody { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Date Added")]
        public DateTime CommentDate { get; set; }

        public Comments()
        {
            CommentDate = DateTime.Now;
        }

        [Display(Name = "Post ID")]
        public int AnnouncementId { get; set; }
        public virtual Announcements Announcements { get; set; }

    }
}