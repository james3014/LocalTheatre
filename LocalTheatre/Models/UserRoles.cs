using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LocalTheatre.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ExpandedUser
    {
        [Key]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public bool IsSuspended { get; set; }

        public IEnumerable<UserRoles> Roles { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class UserRoles
    {
        [Key]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class UserRole
    {
        [Key]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Role
    {
        [Key]
        public string Id { get; set; }
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class UserAndRoles
    {
        [Key]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        public List<UserRole> ColUserRole { get; set; }
    }
}

