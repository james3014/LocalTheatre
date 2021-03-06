﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LocalTheatre.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "Enter a first name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Enter a surname")]
        public string Surname { get; set; }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public static implicit operator ApplicationUser(ExpandedUser v)
        {
            throw new NotImplementedException();
        }
    }
}