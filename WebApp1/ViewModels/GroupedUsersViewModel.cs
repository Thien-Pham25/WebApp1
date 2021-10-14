using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApp1.Models;

namespace WebApp1.ViewModels
{
    public class GroupedUsersViewModel
    {
        public List<ApplicationUser> Staffs { get; set; }
        public List<ApplicationUser> Trainers { get; set; }
       
    }
}