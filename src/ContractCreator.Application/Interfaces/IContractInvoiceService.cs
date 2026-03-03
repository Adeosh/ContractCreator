using ContractCreator.Shared.DTOs;
using ContractCreator.Shared.DTOs.PrintForms;

namespace ContractCreator.Application.Interfaces
{
    public interface IContractInvoiceService
    {
        Task<IEnumerable<ContractInvoiceDto>> GetByContractIdAsync(int contractId);
        Task<ContractInvoiceDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(ContractInvoiceDto dto);
        Task UpdateAsync(ContractInvoiceDto dto);
        Task DeleteAsync(int id);
        Task<InvoicePrintDto> GetPrintDataAsync(int invoiceId);
    }
}
