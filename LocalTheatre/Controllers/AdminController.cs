using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LocalTheatre.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using PagedList;

namespace LocalTheatre.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;


        // GET: Admin
        [Authorize(Roles = "Administrator")]
        #region public ActionResult Index(string searchStringUserNameOrEmail)
        public ActionResult Index(string searchStringUserNameOrEmail, string currentFilter, int? page)
        {
            try
            {
                int intPage = 1;
                int intPageSize = 5;
                int intTotalPageCount = 0;

                if (searchStringUserNameOrEmail != null)
                {
                    intPage = 1;
                }
                else
                {
                    if (currentFilter != null)
                    {
                        searchStringUserNameOrEmail = currentFilter;
                        intPage = page ?? 1;
                    }
                    else
                    {
                        searchStringUserNameOrEmail = "";
                        intPage = page ?? 1;
                    }
                }

                ViewBag.CurrentFilter = searchStringUserNameOrEmail;

                List<ExpandedUser> col_User = new List<ExpandedUser>();
                int intSkip = (intPage - 1) * intPageSize;

                intTotalPageCount = UserManager.Users
                    .Where(x => x.UserName.Contains(searchStringUserNameOrEmail))
                    .Count();

                var result = UserManager.Users
                    .Where(x => x.UserName.Contains(searchStringUserNameOrEmail))
                    .OrderBy(x => x.UserName)
                    .Skip(intSkip)
                    .Take(intPageSize)
                    .ToList();

                foreach (var item in result)
                {
                    ExpandedUser objUsers = new ExpandedUser
                    {
                        UserName = item.UserName,
                        Email = item.Email,
                    };

                    col_User.Add(objUsers);
                }

                // Set the number of pages
                var _UserAsIPagedList = new StaticPagedList<ExpandedUser>(col_User, intPage, intPageSize, intTotalPageCount);

                return View(_UserAsIPagedList);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                List<ExpandedUser> col_User = new List<ExpandedUser>();
                return View(col_User.ToPagedList(1, 25));
            } 
        }
        #endregion



        // User Management *****************************

        // GET: Admin/Create
        [Authorize(Roles = "Administrator")]
        #region public ActionResult Create()
        public ActionResult Create()
        {
            ExpandedUser objRoles = new ExpandedUser();

            ViewBag.Roles = GetAllRolesAsSelectList();

            return View(objRoles);
        }
        #endregion

        // POST: Admin/Create
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        #region public ActionResult Create(ExpandedUserRoles roles)
        public ActionResult Create(ExpandedUser expandedUser)
        {
            try
            {
                if (expandedUser == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var email = expandedUser.Email.Trim();
                var password = expandedUser.Password.Trim();

                // Username is lower case of email
                var userName = email.ToLower();

                if (email == "")
                {
                    throw new Exception("No Email");
                }
                
                if (password == "")
                {
                    throw new Exception("No Password");
                }

                // Create User
                ApplicationUser objNewAdminUser = new ApplicationUser { UserName = userName, Email = email };
                IdentityResult AdminUserCreateResult = UserManager.Create(objNewAdminUser, password);

                if (AdminUserCreateResult.Succeeded == true)
                {
                    string strNewRole = Convert.ToString(Request.Form["Roles"]);

                    if (strNewRole != "0")
                    {
                        // Put user in role
                        UserManager.AddToRole(objNewAdminUser.Id, strNewRole);
                    }

                    return Redirect("~/Admin");
                }
                else
                {
                    ViewBag.Roles = GetAllRolesAsSelectList();
                    ModelState.AddModelError(string.Empty, "Error: Failed to create the user. Check password requirements.");

                    return View(expandedUser);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Roles = GetAllRolesAsSelectList();
                ModelState.AddModelError(string.Empty, "Error: " + ex);

                return View("Create");
            }
        }
        #endregion

        // GET: Admin/Edit
        [Authorize(Roles = "Administrator")]
        #region public ActionResult Edit(string username)
        public ActionResult EditUser(string username)
        {
            if (username == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ExpandedUser expandedUserRoles = GetUser(username);

            if (expandedUserRoles == null)
            {
                return HttpNotFound();
            }

            return View(expandedUserRoles);
        }
        #endregion

        // POST: Admin/Edit/
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        #region public ActionResult EditUser(ExpandedUserRoles allUserRoles)
        public ActionResult EditUser(ExpandedUser allUserRoles)
        {
            try
            {
                if (allUserRoles == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                ExpandedUser expandedUserRoles = UpdateUser(allUserRoles);

                if (expandedUserRoles == null)
                {
                    return HttpNotFound();
                }

                return Redirect("~/Admin");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                return View("EditUser", GetUser(allUserRoles.UserName));
            }
        }
        #endregion

        // GET: Admin/Delete/
        [Authorize(Roles = "Administrator")]
        #region public ActionResult DeleteUser(string username)
        public ActionResult DeleteUser(string username)
        {
            try
            {
                if (username == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (username.ToLower() == this.User.Identity.Name.ToLower())
                {
                    ModelState.AddModelError(string.Empty, "Error: Cannot delete the current user");

                    return View("EditUser");
                }

                ExpandedUser expandedUserRoles = GetUser(username);

                if (expandedUserRoles == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    DeleteUser(expandedUserRoles);
                }

                return Redirect("~/Admin");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);

                return View("EditUser", GetUser(username));
            }
        }
        #endregion

        // SUSPEND: Admin/SuspendUser
        [Authorize(Roles = "Administrator")]
        #region public ActionResult SuspendUser(string username)
        public ActionResult SuspendUser(string username)
        {
            try
            {
                if (username == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (username.ToLower() == this.User.Identity.Name.ToLower())
                {
                    ModelState.AddModelError(string.Empty, "Error: Cannot suspend the current user");

                    return View("Index");
                }

                ExpandedUser expandedUser = GetUser(username);

                if(expandedUser == null)
                {
                    return HttpNotFound();
                }
                else
                {
                    SuspendUser(expandedUser);
                }

                return Redirect("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);

                return View("~/Admin", GetUser(username));
            }
        }
        #endregion




        // Role Management *****************************

        // GET: /Admin/ViewAllRoles
        [Authorize(Roles = "Administrator")]
        #region public ActionResult ViewAllRoles()
        public ActionResult ViewAllRoles()
        {
            var roleManager =
                new RoleManager<IdentityRole>
                (
                    new RoleStore<IdentityRole>(new ApplicationDbContext())
                    );

            List<Role> colRole = (from objRole in roleManager.Roles
                                     select new Role
                                     {
                                         Id = objRole.Id,
                                         RoleName = objRole.Name
                                     }).ToList();

            return View(colRole);
        }
        #endregion

        // GET: /Admin/AddRole
        [Authorize(Roles = "Administrator")]
        #region public ActionResult AddRole()
        public ActionResult AddRole()
        {
            Role objRole = new Role();
            return View(objRole);
        }
        #endregion

        // POST: /Admin/AddRole
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        #region public ActionResult AddRole(Role role)
        public ActionResult AddRole(Role role)
        {
            try
            {
                if (role == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var RoleName = role.RoleName.Trim();

                if (RoleName == "")
                {
                    throw new Exception("No Rolename");
                }

                // Create Role
                var roleManager =
                    new RoleManager<IdentityRole>(
                        new RoleStore<IdentityRole>(new ApplicationDbContext())
                        );

                if (!roleManager.RoleExists(RoleName))
                {
                    roleManager.Create(new IdentityRole(RoleName));
                }

                return Redirect("~/Admin/ViewAllRoles");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                return View("AddRole");
            }
        }
        #endregion

        // GET: /Admin/EditRoles
        [Authorize(Roles = "Administrator")]
        #region public ActionResult EditRoles(string username)
        public ActionResult EditRole(string username)
        {
            if (username == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Check that we have an actual user
            ExpandedUser expandedUser = GetUser(username);

            if (expandedUser == null)
            {
                return HttpNotFound();
            }

            UserAndRoles userAndRoles = GetUserAndRoles(username);

            return View(userAndRoles);
        }
        #endregion

        // POST: / Admin/EditRoles
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        #region public ActionResult EditRoles(UserAndRoles userAndRoles)
        public ActionResult EditRole(UserAndRoles userAndRoles)
        {
            try
            {
                if (userAndRoles == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                string username = userAndRoles.UserName;
                string newRole = Convert.ToString(Request.Form["AddRole"]);

                if (newRole != "No Roles Found")
                {
                    // Go get the user
                    ApplicationUser applicationUser = UserManager.FindByName(username);

                    // Put user in role
                    UserManager.AddToRole(applicationUser.Id, newRole);
                }

                ViewBag.AddRole = new SelectList(RolesUserIsNotIn(username));

                UserAndRoles allRoles = GetUserAndRoles(username);

                return View(allRoles);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                return View("EditRole");
            }
        }
        #endregion


        // DELETE: /Admin/DeleteUserRole?RoleName=TestRole
        [Authorize(Roles = "Administrator")]
        #region public ActionResult DeleteUserRole(string RoleName)
        public ActionResult DeleteUserRole(string RoleName)
        {
            try
            {
                if (RoleName == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (RoleName.ToLower() == "administrator")
                {
                    throw new Exception(string.Format("Cannot delete {0} role", RoleName));
                }

                var roleManager = new RoleManager<IdentityRole>(
                    new RoleStore<IdentityRole>(new ApplicationDbContext()));

                var UsersInRole = roleManager.FindByName(RoleName).Users.Count();

                if(UsersInRole > 0)
                {
                    throw new Exception(
                        string.Format(
                            "Cannot delete {0} role because it still has users", RoleName)
                            );
                }

                var objRoleToDelete = (from objRole in roleManager.Roles
                                       where objRole.Name == RoleName
                                       select objRole).FirstOrDefault();

                if (objRoleToDelete != null)
                {
                    roleManager.Delete(objRoleToDelete);
                }
                else
                {
                    throw new Exception(
                        string.Format("Cannot delete {0} as role does not exist", RoleName)
                        );
                }

                List<Role> colRole = (from objRole in RoleManager.Roles
                                         select new Role
                                         {
                                             Id = objRole.Id,
                                             RoleName = objRole.Name
                                         }).ToList();

                return View("ViewAllRoles", colRole);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error:" + ex);

                var roleManager =
                    new RoleManager<IdentityRole>(
                        new RoleStore<IdentityRole>(new ApplicationDbContext()));

                List<Role> colRole = (from objRole in roleManager.Roles
                                         select new Role
                                         {
                                             Id = objRole.Id,
                                             RoleName = objRole.Name
                                         }).ToList();

                return View("ViewAllRoles", colRole);
            }
        }
        #endregion


        // DELETE: /Admin/DeleteUserRole?UserName="TestUser&&RoleName="Administrator"
        [Authorize(Roles = "Administrator")]
        #region public ActionResult DeleteRole(string username, string roleName
        public ActionResult DeleteRole(string username, string RoleName)
        {
            try
            {
                if ((username == null) || (RoleName == null))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                username = username.ToLower();

                // Check that we have an actual user
                ExpandedUser objExpandedUser = GetUser(username);

                if (objExpandedUser == null)
                {
                    return HttpNotFound();
                }

                if (username.ToLower() ==
                    this.User.Identity.Name.ToLower() && RoleName == "Administrator")
                {
                    ModelState.AddModelError(string.Empty,
                        "Error: Cannot delete Administrator Role for the current user");
                }

                // Go get the User
                ApplicationUser user = UserManager.FindByName(username);
                // Remove User from role
                UserManager.RemoveFromRoles(user.Id, RoleName);
                UserManager.Update(user);

                ViewBag.AddRole = new SelectList(RolesUserIsNotIn(username));

                return RedirectToAction("EditRoles", new { UserName = username });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);

                ViewBag.AddRole = new SelectList(RolesUserIsNotIn(username));

                UserAndRoles objUserAndRoles =
                    GetUserAndRoles(username);

                return View("EditRoles", objUserAndRoles);
            }
        }
        #endregion




        // Utility Functions *****************************

        #region public ApplicationRoleManager RoleManager
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ??
                    HttpContext.GetOwinContext()
                    .GetUserManager<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }
        #endregion

        #region public ApplicationUserManager UserManager
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ??
                    HttpContext.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        #endregion

        #region private ExpandedUser GetUser(string username)
        private ExpandedUser GetUser(string username)
        {
            ExpandedUser expandedUser = new ExpandedUser();

            var result = UserManager.FindByName(username);

            // If we could not find the user, throw an exception
            if (result == null) throw new Exception("Could not find the user");

            expandedUser.UserName = result.UserName;
            expandedUser.Email = result.Email;

            return expandedUser;
        }
        #endregion

        #region private List GetAllRolesAsSelectList()
        private List<SelectListItem> GetAllRolesAsSelectList()
        {
            List<SelectListItem> SelectRoleListItems = new List<SelectListItem>();

            var roleManager =
                new RoleManager<IdentityRole>(
                    new RoleStore<IdentityRole>(new ApplicationDbContext()));

            var colRoleSelectList = roleManager.Roles.OrderBy(x => x.Name).ToList();

            SelectRoleListItems.Add(
                new SelectListItem
                {
                    Text = "Select",
                    Value = "0"
                });

            foreach (var item in colRoleSelectList)
            {
                SelectRoleListItems.Add(
                    new SelectListItem
                    {
                        Text = item.Name.ToString(),
                        Value = item.Name.ToString()
                    });
            }

            return SelectRoleListItems;
        }
        #endregion

        #region private ExpandedUser UpdateUser(ExpandedUser expandedUser)
        private ExpandedUser UpdateUser(ExpandedUser expandedUser)
        {
            ApplicationUser result = UserManager.FindByName(expandedUser.UserName);

            // If the user is not found throw exception
            if (result == null)
            {
                throw new Exception("Could not find a user");
            }

            result.Email = expandedUser.Email;

            // Check if account is locked
            if (UserManager.IsLockedOut(result.Id))
            {
                // Unlock user
                UserManager.ResetAccessFailedCountAsync(result.Id);
            }

            UserManager.Update(result);

            // Was a password sent
            if (!string.IsNullOrEmpty(expandedUser.Password))
            {
                // Remove current password
                var removePassword = UserManager.RemovePassword(result.Id);

                if (removePassword.Succeeded)
                {
                    var addPassword = UserManager.AddPassword(result.Id, expandedUser.Password);

                    if (addPassword.Errors.Count() > 0)
                    {
                        throw new Exception(addPassword.Errors.FirstOrDefault());
                    }
                } 
            }

            return expandedUser;
        }
        #endregion

        #region private void DeleteUser(ExpandedUser expandedUser)
        private void DeleteUser(ExpandedUser expandedUser)
        {
            ApplicationUser user = UserManager.FindByName(expandedUser.UserName);

            // If user cannot be found, throw an exception
            if (user == null)
            {
                throw new Exception("Could not find the user");
            }

            UserManager.RemoveFromRoles(user.Id, UserManager.GetRoles(user.Id).ToArray());
            UserManager.Update(user);
            UserManager.Delete(user);
        }
        #endregion

        #region private UserAndRoles GetUserAndRoles(string username)
        private UserAndRoles GetUserAndRoles(string username)
        {
            // Go get the user
            ApplicationUser user = UserManager.FindByName(username);

            List<UserRole> colUserRole = 
                (from objRole in UserManager.GetRoles(user.Id)
                 select new UserRole
                 {
                    RoleName = objRole,
                    UserName = username
                 }).ToList();

            if (colUserRole.Count() == 0)
            {
                colUserRole.Add(new UserRole { RoleName = "No Roles Found" });
            }

            ViewBag.AddRole = new SelectList(RolesUserIsNotIn(username));

            // Create UserRolesAndPermissions
            UserAndRoles userAndRoles = new UserAndRoles();

            userAndRoles.UserName = username;
            userAndRoles.ColUserRole = colUserRole;
            return userAndRoles;
        }
        #endregion

        #region private List<string> RolesUserIsNotIn(string username)
        private List<string> RolesUserIsNotIn(string username)
        {
            // Get roles the user is not in
            var colAllRoles = RoleManager.Roles.Select(x => x.Name).ToList();

            // Get the roles for an individual
            ApplicationUser user = UserManager.FindByName(username);

            // If we could not find the user, throw an exception
            if (user == null)
            {
                throw new Exception("Could not find the user");
            }

            var colRolesForUser = UserManager.GetRoles(user.Id).ToList();

            var colRolesUserIsNotIn = (from objRole in colAllRoles
                                       where !colRolesForUser.Contains(objRole)
                                       select objRole).ToList();

            if (colRolesUserIsNotIn.Count() == 0)
            {
                colRolesUserIsNotIn.Add("No Roles Found");
            }

            return colRolesUserIsNotIn;
        }
        #endregion

        #region private ExpandedUser SuspendUser(ExpandedUser expandedUser)
        private ExpandedUser SuspendUser(ExpandedUser expandedUser)
        {
            ApplicationUser user = UserManager.FindByName(expandedUser.UserName);

            // If we can't find the user, throw an exception
            if (user == null)
            {
                throw new Exception("Could not find the user");
            }

            // Try to create new user object ** Doesn't work currently **
            try
            {
                UserManager.AddToRole(user.Id, "Suspended");
                UserManager.RemoveFromRole(user.Id, "User");

                expandedUser.IsSuspended = true;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
            }

            return (expandedUser);
        }
        #endregion

        #region private ExpandedUser UnsuspendUser(ExpandedUser expandedUser)
        private ExpandedUser UnsuspendUser(ExpandedUser expandedUser)
        {
            ApplicationUser user = UserManager.FindByName(expandedUser.UserName);

            // If we can't find the user, throw an exception
            if (user == null)
            {
                throw new Exception("Could not find the user");
            }

            // Try to create new user object ** Doesn't work currently **
            try
            {
                UserManager.AddToRole(user.Id, "User");
                UserManager.RemoveFromRole(user.Id, "Suspended");

                expandedUser.IsSuspended = false;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
            }

            return (expandedUser);
        }
        #endregion
    }


}
