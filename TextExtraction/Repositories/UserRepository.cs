using Microsoft.EntityFrameworkCore;
using TextExtraction.Data;
using TextExtraction.Interfaces;
using TextExtraction.Models;

namespace TextExtraction.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AppUser> GetUserByIdAsync(string id)
        {
            return await _context.Users.Include(x => x.FileHistory).FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
