using TextExtraction.Models;

namespace TextExtraction.Interfaces
{
    public interface IHistoryRepository
    {
        bool Add(FileHistory fileHistory);
        bool Save();
    }
}
