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

        public string CommentAuthor { get; set; }

        [Required(ErrorMessage = "Please enter a comment")]
        [Display(Name = "Content")]
        public string CommentBody { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Date")]
        public DateTime CommentDate { get; set; }

        public int AnnouncementId { get; set; }

    }
}