using TBA.Models.Entities;
using TBA.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace TBA.Business
{
    public class BusinessList : IBusinessList
    {
        private readonly TrelloDbContext _context;

        public BusinessList(TrelloDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<List>> GetAllListsAsync()
        {
            return await _context.Lists.ToListAsync();
        }

        public async Task<List?> GetListAsync(int id)
        {
            return await _context.Lists.FindAsync(id);
        }

        public async Task<bool> SaveListAsync(List list)     
        {
            if (list.ListId == 0)
                _context.Lists.Add(list);
            else
                _context.Lists.Update(list);

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<List?> GetListWithBoardAsync(int listId)
        {
            return await _context.Lists
                .Include(l => l.Board)
                .FirstOrDefaultAsync(l => l.ListId == listId);
        }

        public async Task<bool> DeleteListAsync(int listId)
        {
            var list = await _context.Lists.FindAsync(listId);
            if (list == null) return false;

            _context.Lists.Remove(list);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}


