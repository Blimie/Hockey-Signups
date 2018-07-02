using _180416_Hockey.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _180416_Hockey.Models
{
    public class HomePageViewModel
    {
        public string Message { get; set; }
    }
    public class SignUpViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public Event Event { get; set; }
    }
}