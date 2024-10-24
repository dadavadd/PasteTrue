using Microsoft.EntityFrameworkCore;
using PasteTrue.Models;

namespace PasteTrue.Repositories.Interfaces
{
    public interface IPasteRepository
    {
        Task<Paste> GetPasteById(int id);
        Task<IEnumerable<Paste>> GetUserPastes(string userId);
        Task<Paste> CreatePaste(Paste newPaste);

        Task DeletePaste(Paste paste);

        Task SaveChangesAsync();

    }
}
