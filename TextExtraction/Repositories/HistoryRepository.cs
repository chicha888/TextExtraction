using TextExtraction.Data;
using TextExtraction.Interfaces;
using TextExtraction.Models;

namespace TextExtraction.Repositories
{
    public class HistoryRepository : IHistoryRepository
    {
        private readonly AppDbContext _context;

        public HistoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public bool Add(FileHistory fileHistory)
        {
            _context.FileHistories.Add(fileHistory);
            return Save();
        }

        public bool Save()
        {
            var result = _context.SaveChanges();
            return result > 0;
        }
    }
}
