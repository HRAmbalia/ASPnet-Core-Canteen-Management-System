 using CanteenManagementSystem.Areas.Identity.Data;
using CanteenManagementSystem.Areas.Identity.Pages.Account;
using CanteenManagementSystem.Data;
using CanteenManagementSystem.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static CanteenManagementSystem.Areas.Identity.Pages.Account.ExternalLoginModel;

namespace CanteenManagementSystem.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private UserManager<CmsAppUser> _userManager;
        private SignInManager<CmsAppUser> _signInManager;
        private IWebHostEnvironment _webHostEnvironment;
        private IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _context;
        private readonly CmsDbContext _user;
        private readonly ILogger<LogoutModel> _logger;
        private string UserInputtedRole;
        private string actualRole;

        public AdminController(UserManager<CmsAppUser> userManager,
            SignInManager<CmsAppUser> signInManager,
            IWebHostEnvironment hostEnvironment,
            IHttpContextAccessor httpContextAccessor,
            AppDbContext context,
            CmsDbContext user,
            ILogger<LogoutModel> logger)
        {
            checkRoleFromSession();
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = hostEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _user = user;
            _logger = logger;
        }

        private async Task<IActionResult> checkRoleFromSession()
        {
            var customerEmailId = "";
            if (User != null)
            {
                customerEmailId = User.Identity.Name;
                var userDetails = await _user.Users.FirstOrDefaultAsync(u => u.Email == customerEmailId);
                var role = userDetails.Role;
                if (role == "Staff")
                {
                    return RedirectToAction("Staff", "Index");
                }
                else if (role == "customer")
                {
                    return RedirectToAction("Home", "Index");
                }
                else
                {
                    return RedirectToAction("Admin", "Index");
                }
            }
            else
            {
                return RedirectToAction("Home", "Index");
            }
        }

        public async Task<IActionResult> showMenu()
        {
            return View(await _context.menudetails.Where(b => b.manuAvailbility == "yes").ToListAsync());
        }

        public async Task<ActionResult> IndexAsync(string userInputtedRole)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userID = _userManager.GetUserId(HttpContext.User);
                CmsAppUser user = _userManager.FindByIdAsync(userID).Result;
                if (user != null)
                {
                    actualRole = user.Role;
                    HttpContext.Session.SetString("_actualRole", actualRole); 
                    if (userInputtedRole != actualRole)
                    {
                        await _signInManager.SignOutAsync();
                        _logger.LogInformation("User logged out.");
                        TempData["login"] = "loggedin but but userInputtedRole and actualRole does not match, So loggedOut";
                        return RedirectToAction("loginStaff", "Admin");
                    }
                    if (userInputtedRole == "Staff")
                    {
                        return RedirectToAction("Index", "Staff");
                    }
                }
            }
            return View();
        }

        public ActionResult registerStaff()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> registerStaff(IFormCollection form, IFormFile ifile, string returnUrl = null)
        {
            var staffFirstName = form["staffFirstName"];
            var staffLastName = form["staffLastName"];
            var staffEmail = form["staffEmail"];
            var staffPhoneNo = form["staffPhoneNo"];
            var staffGender = form["staffGender"];
            var staffRole = form["staffRole"];
            var staffJoiningDate = form["staffJoiningDate"];
            string staffPassword = form["staffPassword"];
            string staffConfPassword = form["staffConfPassword"];

            if ( staffPassword == staffConfPassword) 
            {
                try
                {
                    if (form.Files.Count > 0 && form.Files[0] != null)
                    {
                        var imageExtentantion = Path.GetExtension(form.Files[0].FileName);
                        if (imageExtentantion == ".jpg" || imageExtentantion == ".gif" || imageExtentantion == ".jpeg")
                        {
                            var saveImage = Path.Combine(_webHostEnvironment.WebRootPath, "images\\user", form.Files[0].FileName);
                            var fileStream = new FileStream(saveImage, FileMode.Create);
                            await form.Files[0].CopyToAsync(fileStream);
                            var user = new CmsAppUser { UserName = staffEmail, Email = staffEmail, userFirstName = staffFirstName, userLastSurname = staffLastName, FullName = staffFirstName + staffLastName, userGender = staffGender, Role = staffRole, userStream = "none", staffJoinDate = staffJoiningDate, staffImage = saveImage, PhoneNumber = staffPhoneNo };
                            var result = await _userManager.CreateAsync(user, staffPassword);
                            if (result.Succeeded)
                            {
                                TempData["Messages"] = staffRole + " by name " + staffFirstName + " " + staffLastName + " is Successfully Created";
                            }
                            foreach (var error in result.Errors)
                            {
                                TempData["Messages"] = error.Description;
                            }
                        }
                        else
                        {
                            TempData["Messages"] = "Only jpg, gif and jpeg files are accepted";
                        }
                    }
                    else
                    {
                        TempData["Messages"] = "Something went wrong, Try again later!";
                    }
                }
                catch (Exception)
                {
                    TempData["Messages"] = "Something went wrong, Try again later!";
                }
            }
            else
            {
                TempData["Messages"] = "Password Does Not Match";
            }
            return RedirectToAction(returnUrl);
        }

        [AllowAnonymous]
        public ActionResult loginStaff()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> loginStaff(IFormCollection form, IFormFile ifile, string returnUrl = null)
        {
            var staffEmail = form["staffEmail"].ToString();
            var staffPassword = form["staffPassword"].ToString();
            var RememberMe = false;
            if (form["staffRememberMe"].ToString() == "on")
            {
                RememberMe = true;
            }
            else
            {
                RememberMe = false;
            }
            if(form["findRole"].ToString() == "on")
            {
                UserInputtedRole = "Admin";
            }
            else
            {
                UserInputtedRole = "Staff";
            }
            var result = await _signInManager.PasswordSignInAsync(staffEmail, staffPassword, RememberMe, lockoutOnFailure: false);
            if ( result.Succeeded )
            {
                TempData["login"] = staffEmail + " " + staffPassword + " " + RememberMe + " " + User.Identity.Name;
            }
            return RedirectToAction("Index", new { userInputtedRole = UserInputtedRole });
        }

        public ActionResult viewCustomer()
        {
            return View();
        }
    }
}
