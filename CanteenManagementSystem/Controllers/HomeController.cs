using CanteenManagementSystem.Areas.Identity.Data;
using CanteenManagementSystem.Data;
using CanteenManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CanteenManagementSystem.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<CmsAppUser> _userManager;
        private readonly CmsDbContext _user;

        public HomeController(ILogger<HomeController> logger,
            UserManager<CmsAppUser> userManager,
            AppDbContext context,
            CmsDbContext user)
        {
            checkRoleFromSession();
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _user = user;
        }

        private async Task<IActionResult> checkRoleFromSession()
        {
            var customerEmailId = "";
            if (User!=null)
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

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult ContactUs()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> ContactUs(IFormCollection form)
        {
            var name = form["sname"];
            var email = form["semail"];
            var messages = form["smessage"];
            var phone = form["sphone"];
            var x = await SendEmail(name, email, messages, phone);
            if (x == "sent")
            {
                TempData["AlertMessage"] = "Your Message Has Been Sent, Thank you for your Feedback :)";
            }
            else
            {
                TempData["AlertMessage"] = "Something went wrong, Try again later :(";
            }
            return RedirectToAction("ContactUs");
        }

        [Authorize]
        private async Task<string> SendEmail(string name, string email, string messages, string phone)
        {
            try
            {
                var message = new MailMessage();
                message.To.Add(new MailAddress("ambaliaharshit25@gmail.com")); // Receiver's email id     
                message.From = new MailAddress("joyroy252525@gmail.com"); // Sender's email id     
                message.Subject = "From CMS";
                message.Body = "Name : " + name + "\nFrom : " + email + "\nPhone No. : " + phone + "\nMessage : " + messages;
                message.IsBodyHtml = true;
                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = "joyroy252525@gmail.com", // Sender's email id     
                        Password = "2hRa&#5gP-#h" // Password     
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(message);
                    return "sent";
                }
            }
            catch (Exception)
            {
                return "not sent";
            }
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> showMenu()
        {
            return View(await _context.menudetails.Where(b => b.manuAvailbility == "yes").ToListAsync());
        }

        [Authorize]
        public async Task<IActionResult> menuSearch(IFormCollection form)
        {
            string search = form["menuSearch"];
            if ( String.IsNullOrEmpty(search) )
            {
                TempData["AlertMessage"] = "No menu Available by this name :(";
                return RedirectToAction("showMenu");
            }
            var Data = await _context.menudetails.Where(b => b.manuName.Contains(search)).Where(b => b.manuAvailbility == "yes").ToListAsync();
            return View(Data);
        }

        [Authorize]
        public async Task<IActionResult> boookedMenu()
        {
            var menuDetails = await _context.orderDetails.Where(b => b.orderDate==DateTime.Today).Where(m => m.customerEmailId == User.Identity.Name).OrderBy(b => b.orderDate).ToListAsync();
            return View(menuDetails);
        }

        [Authorize]
        public async Task<IActionResult> orderNow(int? id)
        {
            var orderdetails = new orderDetails();
            orderdetails.customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customerEmailId = orderdetails.customerEmailId = User.Identity.Name;
            if (id == null)
            {
                TempData["AlertMessage"] = "Something went Wrong try Again later";
                return RedirectToAction("showMenu", "Home");
            }
            orderdetails.menuId = (int)id;
            var menuDetails = await _context.menudetails.FirstOrDefaultAsync(m => m.Id == id);
            var userDetails = await _user.Users.FirstOrDefaultAsync(u => u.Email == customerEmailId);
            if (menuDetails == null)
            {
                TempData["AlertMessage"] = "Something went Wrong try Again later";
                return RedirectToAction("showMenu", "Home");
            }
            orderdetails.menuName = menuDetails.manuName;
            orderdetails.menuPrice = menuDetails.menuPrice;
            orderdetails.orderDate = DateTime.Now;
            orderdetails.customerName = userDetails.userFirstName;
            orderdetails.paymentStatus = "pending";
            if (ModelState.IsValid)
            {
                _context.Add(orderdetails);
                await _context.SaveChangesAsync();
                TempData["AlertMessage"] = "Your Order is Confirmed";
                return RedirectToAction("showMenu", "Home");
            }
            TempData["AlertMessage"] = "Something went Wrong try Again later";
            return RedirectToAction("showMenu", "Home");
            // Generate PDF 
        }

        [Authorize]
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
            ViewBag.menuId = menuDetails.Id;
            return View(menuDetails);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /////////////////////////////////
        public async Task<IActionResult> printPDF(int? id)
        {
            var orderDetails = await _context.orderDetails.FirstOrDefaultAsync(m => m.Id == id);
            var menuId = orderDetails.menuId;
            var menuName = orderDetails.menuName;
            var menuPrice = orderDetails.menuPrice;
            var orderDate = orderDetails.orderDate;
            var custName = orderDetails.customerName;
            var custEmailId = orderDetails.customerEmailId;
            var paymentStts = orderDetails.paymentStatus;
            PdfDocument doc = new PdfDocument();

            PdfPage page = doc.Pages.Add();

            PdfGraphics graphics = page.Graphics;

            //here enter the photo location
            FileStream imageStream = new FileStream("wwwroot/images/download.png", FileMode.Open, FileAccess.Read);
            RectangleF bounds = new RectangleF(120, 0, 250, 250);
            PdfImage image = PdfImage.FromStream(imageStream);
            //Draws the image to the PDF page
            page.Graphics.DrawImage(image, bounds);

            PdfGrid pdfGrid = new PdfGrid();
            //Add values to list
            List<object> data = new List<object>();
            //here enter the name and relative value
            Object row1 = new { ID = "Menu Id", Value = menuId };
            Object row2 = new { ID = "Menu Name", Value = menuName };
            Object row3 = new { ID = "Menu Price", Value = menuPrice };
            Object row4 = new { ID = "Order Date", Value = orderDate };
            Object row5 = new { ID = "Customer Name", Value = custName };
            Object row6 = new { ID = "Customer Email Id", Value = custEmailId };
            data.Add(row1);
            data.Add(row2);
            data.Add(row3);
            data.Add(row4);
            data.Add(row5);
            data.Add(row6);

            IEnumerable<object> dataTable = data;

            pdfGrid.DataSource = dataTable;
            //Draw grid to the page of PDF document.
            pdfGrid.Draw(page, new Syncfusion.Drawing.PointF(10, 300));

            //Save the PDF document to stream
            MemoryStream stream = new MemoryStream();
            doc.Save(stream);
            //If the position is not set to '0' then the PDF will be empty.
            stream.Position = 0;
            //Close the document.
            doc.Close(true);
            //Defining the ContentType for pdf file.
            string contentType = "application/pdf";
            //Define the file name.

            //change file name withusername and id
            string fileName = "Outputt.pdf";
            //Creates a FileContentResult object by using the file contents, content type, and file name.
            return File(stream, contentType, fileName);
        }
    }
}
