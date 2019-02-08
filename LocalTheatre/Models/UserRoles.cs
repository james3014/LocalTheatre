using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LocalTheatre.Models
{
    public class ExpandedUserRoles
    {
        [Key]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        [Display(Name = "Lockout End Date")]
        public DateTime? LockoutEndDate { get; set; }

        public int AccessFailedCount { get; set; }

        public string PhoneNumber { get; set; }

        public IEnumerable<UserRoles> Roles { get; set; }
    }


    public class UserRoles
    {
        [Key]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
    }

    public class UserRoleDTO
    {
        [Key]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
    }

    public class RoleDTO
    {
        [Key]
        public string Id { get; set; }
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
    }

    public class UserAndRolesDTO
    {
        [Key]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        public List<UserRoleDTO> ColUserRoleDTO { get; set; }
    }
}

