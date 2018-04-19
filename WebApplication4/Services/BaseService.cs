using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication4.Models;

namespace WebApplication4.Services
{
    public class BaseService
    {
        protected static readonly  ApplicationDbContext Context = new ApplicationDbContext();

    }
}