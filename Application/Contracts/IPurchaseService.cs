using System;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Contracts
{
    public interface IPurchaseService
    {
        Task<PurchaseResponseDto> CreatePurchaseAsync(CreatePurchaseDto dto, int factoryId);
        Task<PaginatedPurchasesDto> GetByFactoryIdAsync(int factoryId, int page, int pageSize);
        Task<PurchaseResponseDto> GetByIdAsync(Guid id);
        Task<PurchaseResponseDto> UpdatePaymentStatusAsync(Guid purchaseId, UpdatePaymentStatusDto dto);
    }
}
