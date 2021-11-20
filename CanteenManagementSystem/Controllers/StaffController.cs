using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CanteenManagementSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using CanteenManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;

namespace CanteenManagementSystem.Controllers
{
    [Authorize]
    public class StaffController : Controller
    {
        private readonly AppDbContext _context;
        private readonly CmsDbContext _user;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public StaffController(AppDbContext context,
            IWebHostEnvironment hostEnvironment,
            CmsDbContext user)
        {
            checkRoleFromSession();
            _context = context;
            _user = user;
            _webHostEnvironment = hostEnvironment;
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

        public async Task<IActionResult> Index()
        {
            return View(await _context.menudetails.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var menuDetails = await _context.menudetails.FirstOrDefaultAsync(m => m.Id == id);
            if (menuDetails == null)
            {
                return NotFound();
            }

            return View(menuDetails);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(menuDetails menu, IFormCollection form)
        {
            var manuName = form["manuName"];
            var menuPrice = form["menuPrice"];
            var manuAvailbility = form["manuAvailbility"];
            var manuType = form["manuType"];
            var vegOrNonVeg = form["vegOrNonVeg"];
            var manuImage = form.Files[0].FileName;
            var manuDescription = form["manuDescription"];
            var saveImage = Path.Combine(_webHostEnvironment.WebRootPath, "images\\menu", Request.Form.Files[0].FileName);
            var fileStream = new FileStream(saveImage, FileMode.Create);
            await Request.Form.Files[0].CopyToAsync(fileStream);
            menu.manuImage = manuImage;
            if (ModelState.IsValid)
            {
                _context.Add(menu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(menu);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var menuDetails = await _context.menudetails.FindAsync(id);
            ViewBag.menuImage = menuDetails.manuImage;
            if (menuDetails == null)
            {
                return NotFound();
            }
            return View(menuDetails);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,manuName,menuPrice,manuAvailbility,manuType,vegOrNonVeg,manuImage,manuDescription")] menuDetails menuDetails)
        {
            if (id != menuDetails.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(menuDetails);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!menuDetailsExists(menuDetails.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(menuDetails);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var menuDetails = await _context.menudetails.FirstOrDefaultAsync(m => m.Id == id);
            if (menuDetails == null)
            {
                return NotFound();
            }
            return View(menuDetails);
        }

        public async Task<IActionResult> showPendingOrders()
        {
            var pendingOrders = await _context.orderDetails.Where(b => b.orderDate == DateTime.Today).Where(b => b.paymentStatus == "pending").ToListAsync();
            if (pendingOrders == null)
            {
                ViewBag.AlertMessage = "No Upcoming Orders, WORK HARD!!!";
            }
            return View(pendingOrders);
        }

        public async Task<IActionResult> showMissedOrders()
        {
            var pendingOrders = await _context.orderDetails.Where(b => b.orderDate != DateTime.Today).Where(b => b.paymentStatus == "pending").ToListAsync();
            if (pendingOrders == null)
            {
                ViewBag.AlertMessage = "No Missed orders!!!";
            }
            return View(pendingOrders);
        }

        public async Task<IActionResult> paymentStatusDone(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("showPendingOrders", "Staff");
            }
            var menuDetails = await _context.orderDetails.FirstOrDefaultAsync(m => m.Id == id);
            menuDetails.paymentStatus = "done";
            try
            {
                _context.Update(menuDetails);
                await _context.SaveChangesAsync();
                return RedirectToAction("showPendingOrders", "Staff");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!menuDetailsExists(menuDetails.Id))
                {
                    return RedirectToAction("showPendingOrders", "Staff");
                }
                else
                {
                    throw;
                }
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var menuDetails = await _context.menudetails.FindAsync(id);
            _context.menudetails.Remove(menuDetails);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool menuDetailsExists(int id)
        {
            return _context.menudetails.Any(e => e.Id == id);
        }
    }
}
