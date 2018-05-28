using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication4.Models;

namespace WebApplication4.Services
{
    public class UserService : BaseService
    {
        protected UserManager<ApplicationUser> UserManager { get; set; }

        public UserService()
        {
            this.UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(Context));
        }
        public ApplicationUser GetUserById(string userId)
        {
            var user = UserManager.FindById(userId);
            return user;

        }
    }

        
}