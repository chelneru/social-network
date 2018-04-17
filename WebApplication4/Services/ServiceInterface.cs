using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication4.Models;

namespace WebApplication4.Services
{
    interface ServiceInterface
    {
          ApplicationDbContext _context { get; set; }

    }
}
