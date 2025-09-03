using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public interface IPurchaseRepository
    {
        Task<Purchase> CreateAsync(Purchase purchase);
        Task<IEnumerable<Purchase>> GetByFactoryIdAsync(int factoryId, int page, int pageSize);
        Task<Purchase> GetByMarketplaceItemIdAsync(Guid itemId);
        Task<Purchase> UpdatePaymentStatusAsync(Guid purchaseId, PaymentStatus status);
        Task<Purchase> GetByIdAsync(Guid id);
        Task<int> GetTotalCountByFactoryIdAsync(int factoryId);
    }
}
