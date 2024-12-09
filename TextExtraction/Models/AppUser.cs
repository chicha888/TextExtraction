using Microsoft.AspNetCore.Identity;

namespace TextExtraction.Models
{
    public class AppUser : IdentityUser
    {
        public DateTime CreatedDate { get; set; }
        public ICollection<FileHistory> FileHistory { get; set; }
    }
}
