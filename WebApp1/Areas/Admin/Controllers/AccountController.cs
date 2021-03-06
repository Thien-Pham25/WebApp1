using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApp1.Models;
using WebApp1.Utils;
using WebApp1.ViewModels;

namespace WebApp1.Areas.Admin.Controllers
{
    public class AccountController : Controller
    {       
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private readonly ApplicationDbContext _context;

        public AccountController()
        {
            _context = new ApplicationDbContext();
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: Admin/Account
        public async Task<ActionResult> Index()
        {
            var trainerRole = await _context.Roles.SingleOrDefaultAsync(r => r.Name == Role.Trainer);
            var staffRole = await _context.Roles.SingleOrDefaultAsync(r => r.Name == Role.Staff);

            var model = new GroupedUsersViewModel()
            {
                Trainers = await _context.Users
                    .Where(u => u.Roles.Any(r => r.RoleId == trainerRole.Id))
                    .ToListAsync(),
                Staffs = await _context.Users
                    .Where(u => u.Roles.Any(r => r.RoleId == staffRole.Id))
                    .ToListAsync(),
            };
            return View(model);
        }
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var model = new AccountViewModel()
            {
                Roles = new List<string>() { Role.Trainer, Role.Staff }
            };

            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Create (AccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user.Id, model.Role);

                    
                    return RedirectToAction("Index");
                }
                AddErrors(result);
            }
            model.Roles = new List<string>() { Role.Trainer, Role.Staff };

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        public async Task<ActionResult> Details (string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = await UserManager.FindByIdAsync(id);
            
            if(user == null)
            {
                return HttpNotFound();
            }


            var model = new ProfileViewModel()
            {
                User = user,
                Roles = new List<string>(await UserManager.GetRolesAsync(id))
            };
            return View(model);
        }
        [HttpGet]
        public async Task<ActionResult> Delete(string id, bool? saveChangesError = false)
        {
            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            var roles = await UserManager.GetRolesAsync(id);

            if (!roles.All(r => r == Role.Staff || r == Role.Trainer))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }


            var model = new ProfileViewModel()
            {
                User = user,
                Roles = new List<string>(await UserManager.GetRolesAsync(user.Id))
            };

            if (saveChangesError == true)
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }

            return View(model);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ConfirmedDelete(string id)
        {
            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
            {
                return RedirectToAction(nameof(Index));
            }

            IdentityResult result = await UserManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index), "Account");
            }

            return RedirectToAction(nameof(Delete), new { id, saveChangesError = true });
        }
        [HttpGet]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
            {
                return HttpNotFound();
            }


            var model = new ProfileViewModel()
            {
                User = user,
                Roles = new List<string>(await UserManager.GetRolesAsync(id))
            };
            return View(model);
        }
    }
}