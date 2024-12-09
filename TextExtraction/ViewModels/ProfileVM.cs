using TextExtraction.Models;

namespace TextExtraction.ViewModels
{
    public class ProfileVM
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public ICollection<FileHistory> fileHistory { get; set; }
    }
}
