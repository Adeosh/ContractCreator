using ContractCreator.Infrastructure.Persistence;
using ContractCreator.Shared.DTOs.Data;
using Microsoft.EntityFrameworkCore;

namespace ContractCreator.Infrastructure.Services.Classifiers
{
    public class ClassifierService : IClassifierService
    {
        private readonly AppDbContext _context;

        public ClassifierService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ClassifierDto>> GetOkopfsAsync()
        {
            return await _context.ClassifierOkopfs
                .AsNoTracking()
                .OrderBy(x => x.Code)
                .Select(x => new ClassifierDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name
                })
                .ToListAsync();
        }

        public async Task<List<ClassifierDto>> GetCurrenciesAsync()
        {
            return await _context.ClassifierOkvs
                .AsNoTracking()
                .OrderBy(x => x.CurrencyName)
                .Select(x => new ClassifierDto
                {
                    Id = x.Id,
                    Code = x.LetterCode,
                    Name = x.CurrencyName
                })
                .ToListAsync();
        }

        public async Task<List<ClassifierDto>> GetAllOkvedsAsync()
        {
            return await _context.ClassifierOkveds
                .AsNoTracking()
                .Select(x => new ClassifierDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name
                })
                .ToListAsync();
        }

        public async Task<List<ClassifierDto>> SearchOkvedsAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return new List<ClassifierDto>();

            return await _context.ClassifierOkveds
                .AsNoTracking()
                .Where(x => EF.Functions.Like(x.Code, $"{query}%") ||
                            EF.Functions.Like(x.Name.ToLower(), $"%{query.ToLower()}%"))
                .Take(20)
                .Select(x => new ClassifierDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name
                })
                .ToListAsync();
        }
    }
}
