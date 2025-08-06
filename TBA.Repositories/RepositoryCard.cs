using TBA.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBA.Repositories;
using Microsoft.EntityFrameworkCore;
using TBA.Data.Models;

namespace TBA.Repositories
{
    public interface IRepositoryCard
    {
        Task<bool> UpsertAsync(Card entity, bool isUpdating);

        Task<bool> CreateAsync(Card entity);

        Task<bool> DeleteAsync(Card entity);

        Task<IEnumerable<Card>> ReadAsync();

        Task<Card> FindAsync(int id);

        Task<bool> UpdateAsync(Card entity);

        Task<bool> UpdateManyAsync(IEnumerable<Card> entities);

        Task<bool> ExistsAsync(Card entity);
        Task<bool> CheckBeforeSavingAsync(Card entity);
        Task<List<Card>> GetCardsWithIncludesAsync();
        Task<User?> GetUserByUsernameAsync(string username);
        Task<Card?> GetCardAsync(int cardId);
        Task<bool> UpdateCardAsync(Card card);
        Task<Card?> GetCardWithListAndBoardAsync(int cardId);
        Task RemoveCardRelationsAsync(int cardId);
    }
    public class RepositoryCard : RepositoryBase<Card>, IRepositoryCard
    {
        public RepositoryCard(TrelloDbContext context) : base(context)
        {
        }
        public async Task<bool> CheckBeforeSavingAsync(Card entity)
        {
            var exists = await ExistsAsync(entity);
            if (exists)
            {
                // algo más
            }

            return await UpsertAsync(entity, exists);
        }
        public async Task<List<Card>> GetCardsWithIncludesAsync()
        {
            return await DbContext.Cards
                .Include(c => c.Users)
                .Include(c => c.Comments)
                .Include(c => c.Labels)
                .Include(c => c.Attachments)
                .Include(c => c.List)
                    .ThenInclude(l => l.Board)
                .ToListAsync();
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await DbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        }


        public async Task<Card?> GetCardAsync(int cardId)
        {
            return await DbContext.Cards
                .Include(c => c.Users) 
                .FirstOrDefaultAsync(c => c.CardId == cardId);
        }


        public async Task<bool> UpdateCardAsync(Card card)
        {
            var existing = await DbContext.Cards
                .Include(c => c.List)
                    .ThenInclude(l => l.Board) 
                .FirstOrDefaultAsync(c => c.CardId == card.CardId);

            if (existing == null) return false;

            existing.ListId = card.ListId;

            var result = await DbContext.SaveChangesAsync() > 0;

            if (result)
            {
                await DbContext.Entry(existing).Reference(c => c.List).Query().Include(l => l.Board).LoadAsync();
            }

            return result;
        }

        public async Task<Card?> GetCardWithListAndBoardAsync(int cardId)
        {
            return await DbContext.Cards
                .AsNoTracking()
                .Include(c => c.List)
                .ThenInclude(l => l.Board)
                .FirstOrDefaultAsync(c => c.CardId == cardId);
        }

        public async Task RemoveCardRelationsAsync(int cardId)
        {
            var card = await DbContext.Cards
                .Include(c => c.Users)
                .Include(c => c.Comments)
                .Include(c => c.Labels)
                .Include(c => c.Attachments)
                .FirstOrDefaultAsync(c => c.CardId == cardId);

            if (card == null) return;

            card.Users?.Clear();
            card.Comments?.Clear();
            card.Labels?.Clear();
            card.Attachments?.Clear();

            await DbContext.SaveChangesAsync();
        }

        public virtual async Task<bool> ExistsAsync(Card entity)
        {
            return await DbContext.Cards.AnyAsync(c => c.CardId == entity.CardId);
        }

        public async Task<bool> UpsertAsync(Card entity, bool isUpdating)
        {
            if (isUpdating)
                return await UpdateAsync(entity);  // Lógica de UPDATE
            else
                return await CreateAsync(entity);  // Lógica de INSERT
        }



    }
}