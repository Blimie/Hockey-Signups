using _180416_Hockey.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _180416_Hockey.Models;  
using System.Net;
using System.Net.Mail;

namespace _180416_Hockey.Controllers
{
    public class HomeController : Controller
    {                 
        HockeyDB dB = new HockeyDB(Properties.Settings.Default.ConStr);     
        public ActionResult Index()
        {
            HomePageViewModel vm = new HomePageViewModel();  
            if (TempData["Message"] != null)
            {
                vm.Message = (string)TempData["Message"];
            }
            return View(vm);
        }
        public ActionResult AddEvent()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddEvent(Event e)
        {
            dB.AddEvent(e);
            foreach(Email email in dB.GetEmails())
            {
                SendEmail(email.EmailAddress, email.FirstName, email.LastName);
            }                        
            TempData["Message"] = $"Event added successfully, new Id: {e.Id}. Emails were also sent!";
            return Redirect("/");
        }
        public ActionResult EventHistory()
        {            
            return View(dB.GetEvents());
        }
        public ActionResult EventInfo(int eventId)
        {
            return View(dB.GetPlayers(eventId));
        }
        public ActionResult SignUp()
        {
            SignUpViewModel vm = new SignUpViewModel {
                Event = dB.GetLastEvent()
            };          
            if (Request.Cookies["first_name"] != null)
            {
               vm.FirstName = Request.Cookies["first_name"].Value;
            }
            if (Request.Cookies["last_name"] != null)
            {
                vm.LastName = Request.Cookies["last_name"].Value;
            }
            if (Request.Cookies["email_address"] != null)
            {
                vm.EmailAddress = Request.Cookies["email_address"].Value;
            }       
            return View(vm);
        }
        [HttpPost]
        public ActionResult SignUp(Player p)
        {
            dB.AddPlayer(p);
            Response.Cookies.Add(new HttpCookie("first_name", p.FirstName));
            Response.Cookies.Add(new HttpCookie("last_name", p.LastName));
            Response.Cookies.Add(new HttpCookie("email_address", p.EmailAddress));
            TempData["Message"] = "You successfully signed up! See you there IYH!!! Can't wait...";   
            return Redirect("/");
        }
        public ActionResult EmailSignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult EmailSignUp(Player p)
        {
            dB.AddEmail(p);                                 
            TempData["Message"] = "You successfully signed up for the weekly email!";
            return Redirect("/");
        }

        public void SendEmail(string address, string first, string last)
        {
            var fromAddress = new MailAddress("blimiekohn@gmail.com", "Blimie Kohn");   //from email, from name
            var toAddress = new MailAddress(address, first + " " + last ); //to email, to name
            const string fromPassword = "Bk#231411"; //gmail password
            const string subject = "New game posted"; //subject
            const string body = "New hockey game is on! If you are interested to join, (which I'm sure you are 😉) please go to our website to reserve your slot.";  //body  
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };             
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }
    }
}