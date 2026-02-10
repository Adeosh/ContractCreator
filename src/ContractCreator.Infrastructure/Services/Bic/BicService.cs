using ContractCreator.Infrastructure.Persistence;
using ContractCreator.Shared.DTOs.Data;
using Microsoft.EntityFrameworkCore;

namespace ContractCreator.Infrastructure.Services.Bic
{
    public class BicService : IBicService
    {
        private readonly AppDbContext _context;

        public BicService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BankDto>> SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Enumerable.Empty<BankDto>();

            var q = query.ToLower().Trim();

            var rawData = await _context.ClassifierBics
                .AsNoTracking()
                .Where(b => EF.Functions.Like(b.BIC, $"%{q}%") ||
                            EF.Functions.Like(b.Name.ToLower(), $"%{q}%"))
                .OrderBy(b => b.Name)
                .Take(20)
                .Select(b => new
                {
                    b.BIC,
                    b.Name,
                    b.Account,
                    b.SettlementType,
                    b.SettlementName,
                    b.Address
                })
                .ToListAsync();

            return rawData.Select(b => new BankDto
            {
                Bic = b.BIC,
                Name = b.Name,
                CorrespondentAccount = b.Account ?? string.Empty,
                Address = string.Join(", ", new[] { b.SettlementType, b.SettlementName, b.Address }
                                            .Where(s => !string.IsNullOrWhiteSpace(s)))
            });
        }
    }
}
