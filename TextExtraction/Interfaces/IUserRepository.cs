using TextExtraction.Models;

namespace TextExtraction.Interfaces
{
    public interface IUserRepository
    {
        Task<AppUser> GetUserByIdAsync(string id);
    }
}
