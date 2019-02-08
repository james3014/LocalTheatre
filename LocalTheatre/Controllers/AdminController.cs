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
                else;
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
                intTotalPageCount = _userManager.Users
                    .Where(x => x.UserName.Contains(searchStringUserNameOrEmail))
                    .Count();

                var result = _userManager.Users
                    .Where(x => x.UserName.Contains(searchStringUserNameOrEmail))
                    .OrderBy(x => x.UserName)
                    .Skip(intSkip)
                    .Take(intPageSize)
                    .ToList();

                foreach (var item in result)
                {
                    ExpandedUser objUsers = new ExpandedUser();
                    objUsers.UserName = item.UserName;
                    objUsers.Email = item.Email;
                    objUsers.LockoutEndDate = item.LockoutEndDateUtc;
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
            return View();
        }
        #endregion

        // POST: Admin/Create
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        #region public ActionResult Create(ExpandedUserRoles roles)
        public ActionResult Create(ExpandedUser roles)
        {
            try
            {
                if (roles == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var email = roles.Email.Trim();
                var userName = roles.UserName.Trim();
                var password = roles.Password.Trim();

                if (email == "")
                {
                    throw new Exception("No Email");
                }
                
                if (password == "")
                {
                    throw new Exception("No Password");
                }

                // userName is lower case of email
                userName = email.ToLower();

                // Create User
                var objNewAdminUser = new ApplicationUser { UserName = userName, Email = email, };
                var AdminUserCreateResult = _userManager.Create(objNewAdminUser, password);

                if (AdminUserCreateResult.Succeeded == true)
                {
                    string strNewRole = Convert.ToString(Request.Form["Roles"]);

                    if (strNewRole != "0")
                    {
                        // Put user in role
                        _userManager.AddToRole(objNewAdminUser.Id, strNewRole);
                    }

                    return Redirect("~/Admin");
                }
                else
                {
                    ViewBag.Roles = GetAllRolesAsSelectList();
                    ModelState.AddModelError(string.Empty, "Error: Failed to create the user. Check password requirements.");

                    return View(roles);
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

            List<RoleDTO> colRole = (from objRole in roleManager.Roles
                                     select new RoleDTO
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
            RoleDTO objRole = new RoleDTO();
            return View(objRole);
        }
        #endregion

        // POST: /Admin/AddRole
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        #region public ActionResult AddRole(RoleDTO role)
        public ActionResult AddRole(RoleDTO role)
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
        public ActionResult EditRoles(string username)
        {
            if (username == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            username = username.ToLower();

            // Check that we have an actual user
            ExpandedUser expandedUser = new ExpandedUser();
            if (expandedUser == null)
            {
                return HttpNotFound();
            }

            UserAndRolesDTO userAndRoles = GetUserAndRoles(username);

            return View(userAndRoles);
        }
        #endregion

        // POST: / Admin/EditRoles
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        #region public ActionResult EditRoles(UserAndRolesDTO userAndRoles)
        public ActionResult EditRoles(UserAndRolesDTO userAndRoles)
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
                    ApplicationUser user = _userManager.FindByName(username);

                    // Put user in role
                    _userManager.AddToRole(user.Id, newRole);
                }

                ViewBag.AddRole = new SelectList(RolesUserIsNotIn(username));

                UserAndRolesDTO allRoles = GetUserAndRoles(username);

                return View(allRoles);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                return View("EditRoles");
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

                List<RoleDTO> colRole = (from objRole in RoleManager.Roles
                                         select new RoleDTO
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

                List<RoleDTO> colRole = (from objRole in roleManager.Roles
                                         select new RoleDTO
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
                ExpandedUser objExpandedUserDTO = GetUser(username);

                if (objExpandedUserDTO == null)
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
                ApplicationUser user = _userManager.FindByName(username);
                // Remove User from role
                _userManager.RemoveFromRoles(user.Id, RoleName);
                _userManager.Update(user);

                ViewBag.AddRole = new SelectList(RolesUserIsNotIn(username));

                return RedirectToAction("EditRoles", new { UserName = username });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);

                ViewBag.AddRole = new SelectList(RolesUserIsNotIn(username));

                UserAndRolesDTO objUserAndRolesDTO =
                    GetUserAndRoles(username);

                return View("EditRoles", objUserAndRolesDTO);
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

        #region private ExpandedUser GetUser(string username)
        private ExpandedUser GetUser(string username)
        {
            ExpandedUser expandedUser = new ExpandedUser();

            var result = _userManager.FindByName(username);

            // If we could not find the user, throw an exception
            if (result == null) throw new Exception("Could not find the user");

            expandedUser.UserName = result.UserName;
            expandedUser.Email = result.Email;
            expandedUser.LockoutEndDate = result.LockoutEndDateUtc;
            expandedUser.AccessFailedCount = result.AccessFailedCount;
            expandedUser.PhoneNumber = result.PhoneNumber;

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
            ApplicationUser result = _userManager.FindByName(expandedUser.UserName);

            // If the user is not found throw exception
            if (result == null)
            {
                throw new Exception("Could not find a user");
            }

            result.Email = expandedUser.Email;

            // Check if account is locked
            if (_userManager.IsLockedOut(result.Id))
            {
                // Unlock user
                _userManager.ResetAccessFailedCountAsync(result.Id);
            }

            _userManager.Update(result);

            // Was a password sent
            if (!string.IsNullOrEmpty(expandedUser.Password))
            {
                // Remove current password
                var removePassword = _userManager.RemovePassword(result.Id);

                if (removePassword.Succeeded)
                {
                    var addPassword = _userManager.AddPassword(result.Id, expandedUser.Password);

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
            ApplicationUser user = _userManager.FindByName(expandedUser.UserName);

            // If user cannot be found, throw an exception
            if (user == null)
            {
                throw new Exception("Could not find the user");
            }

            _userManager.RemoveFromRoles(user.Id, _userManager.GetRoles(user.Id).ToArray());
            _userManager.Update(user);
            _userManager.Delete(user);
        }
        #endregion

        #region private UserAndRolesDTO GetUserAndRoles(string username)
        private UserAndRolesDTO GetUserAndRoles(string username)
        {
            // Go get the user
            ApplicationUser user = _userManager.FindByName(username);

            List<UserRoleDTO> colUserRole = 
                (from objRole in _userManager.GetRoles(user.Id)
                 select new UserRoleDTO
                 {
                    RoleName = objRole,
                    UserName = username
                 }).ToList();

            if (colUserRole.Count() == 0)
            {
                colUserRole.Add(new UserRoleDTO { RoleName = "No Roles Found" });
            }

            ViewBag.AddRole = new SelectList(RolesUserIsNotIn(username));

            // Create UserRolesAndPermissions
            UserAndRolesDTO userAndRoles = new UserAndRolesDTO();

            userAndRoles.UserName = username;
            userAndRoles.ColUserRoleDTO = colUserRole;
            return userAndRoles;
        }
        #endregion

        #region private List<string> RolesUserIsNotIn(string username)
        private List<string> RolesUserIsNotIn(string username)
        {
            // Get roles the user is not in
            var colAllRoles = _roleManager.Roles.Select(x => x.Name).ToList();

            // Get the roles for an individual
            ApplicationUser user = _userManager.FindByName(username);

            // If we could not find the user, throw an exception
            if (user == null)
            {
                throw new Exception("Could not find the user");
            }

            var colRolesForUser = _userManager.GetRoles(user.Id).ToList();

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
    }
}
