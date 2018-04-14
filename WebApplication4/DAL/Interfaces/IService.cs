using WebApplication4.Models;

namespace WebApplication4.DAL.Interfaces
{
    public interface IService
    {
         ApplicationDbContext _context { get; set; }

    }
}