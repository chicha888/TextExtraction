using OpenCvSharp;
using System.ComponentModel.DataAnnotations.Schema;

namespace TextExtraction.Models
{
    public class FileHistory
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public string FileName { get; set; }
        public DateTime UploadedAt { get; set; }
        public string ExtractedText { get; set; }

        public AppUser User { get; set; }
    }
}
