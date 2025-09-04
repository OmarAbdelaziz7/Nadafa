using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly NadafaDbContext _context;

        public PurchaseRepository(NadafaDbContext context)
        {
            _context = context;
        }

        public async Task<Purchase> CreateAsync(Purchase purchase)
        {
            purchase.UpdateTimestamp();
            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();
            return purchase;
        }

        public async Task<IEnumerable<Purchase>> GetByFactoryIdAsync(int factoryId, int page, int pageSize)
        {
            return await _context.Purchases
                .Include(p => p.MarketplaceItem)
                    .ThenInclude(mi => mi.User)
                .Include(p => p.MarketplaceItem)
                    .ThenInclude(mi => mi.PickupRequest)
                .Include(p => p.Factory)
                .Where(p => p.FactoryId == factoryId)
                .OrderByDescending(p => p.PurchaseDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Purchase> GetByMarketplaceItemIdAsync(Guid itemId)
        {
            return await _context.Purchases
                .Include(p => p.MarketplaceItem)
                .Include(p => p.Factory)
                .FirstOrDefaultAsync(p => p.MarketplaceItemId == itemId);
        }

        public async Task<Purchase> UpdatePaymentStatusAsync(Guid purchaseId, PaymentStatus status)
        {
            var purchase = await _context.Purchases.FindAsync(purchaseId);
            if (purchase != null)
            {
                purchase.PaymentStatus = status;
                purchase.UpdateTimestamp();
                await _context.SaveChangesAsync();
            }
            return purchase;
        }

        public async Task<Purchase> GetByIdAsync(Guid id)
        {
            return await _context.Purchases
                .Include(p => p.MarketplaceItem)
                    .ThenInclude(mi => mi.User)
                .Include(p => p.MarketplaceItem)
                    .ThenInclude(mi => mi.PickupRequest)
                .Include(p => p.Factory)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<int> GetTotalCountByFactoryIdAsync(int factoryId)
        {
            return await _context.Purchases
                .Where(p => p.FactoryId == factoryId)
                .CountAsync();
        }

        public async Task<Purchase> UpdateAsync(Purchase purchase)
        {
            purchase.UpdateTimestamp();
            _context.Purchases.Update(purchase);
            await _context.SaveChangesAsync();
            return purchase;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null)
                return false;

            _context.Purchases.Remove(purchase);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
