using Microsoft.EntityFrameworkCore;
using PasteTrue.Data;
using PasteTrue.Models;
using PasteTrue.Repositories.Interfaces;

namespace PasteTrue.Repositories
{
    public class PasteRepository : IPasteRepository
    {
        private ApplicationDbContext _context;

        public PasteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Paste> GetPasteById(int id)
        {
            return await _context.Pastes.FindAsync(id);
        }

        public async Task<IEnumerable<Paste>> GetUserPastes(string userId)
        {
            return await _context.Pastes.Where(p => p.UserId == userId).ToListAsync();
        }

        public async Task<Paste> CreatePaste(Paste newPaste)
        {
            _context.Pastes.Add(newPaste);
            await SaveChangesAsync();
            return newPaste;
        }

        public async Task DeletePaste(Paste paste)
        {
            _context.Pastes.Remove(paste);
            await SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
