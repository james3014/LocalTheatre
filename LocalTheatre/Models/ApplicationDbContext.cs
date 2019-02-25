using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LocalTheatre.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<LocalTheatre.Models.ExpandedUser> ExpandedUsers { get; set; }

        public System.Data.Entity.DbSet<LocalTheatre.Models.Announcements> Announcements { get; set; }

        public System.Data.Entity.DbSet<LocalTheatre.Models.Comments> Comments { get; set; }
    }
}