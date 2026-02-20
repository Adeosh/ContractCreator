using ContractCreator.Application.Interfaces.Infrastructure;
using ContractCreator.Infrastructure.Persistence;
using ContractCreator.Shared.DTOs.Data;
using Microsoft.EntityFrameworkCore;

namespace ContractCreator.Infrastructure.Services.Gar
{
    public class GarService : IGarService
    {
        #region TypeToSearch
        /// <summary> Словарь типов объекта адреса для наиболее точного поиска </summary>
        public static readonly Dictionary<string, string> TypeToSearch = new()
        {
            // Level 1
            {"область", "обл"},
            {"республика", "респ"},
            {"город", "г"},
            {"край", "край"},
            {"округ", "округ"},
            {"автономный округ", "ао"},
    
            // Level 2-3
            {"городской округ", "г.о."},
            {"район", "р-н"},
            {"поселок", "п"},
            {"федеральная территория", "ф.т."},
            {"муниципальное образование", "м.о."},
            {"муниципальный район", "м.р-н"},
            {"внутригородская территория", "вн.тер.г."},
    
            // Level 4
            {"внутригородской район", "вн.р-н"},
            {"городское поселение", "г.п."},
            {"сельское поселение", "с.п."},
            {"межселенная территория", "межсел.тер."},
    
            // Level 5
            {"село", "с"},
            {"сельская администрация", "с/а"},
            {"территория", "тер"},
            {"почтовое отделение", "п/о"},
            {"дачный поселок", "дп"},
            {"волость", "волость"},
            {"поселок городского типа", "пгт"},
            {"рабочий поселок", "рп"},
            {"сельсовет", "с/с"},
            {"массив", "массив"},
            {"сельское муниципальное образование", "с/мо"},
            {"сельский округ", "с/о"},
    
            // Level 6
            {"аал", "аал"},
            {"автодорога", "автодорога"},
            {"выселки", "высел"},
            {"железнодорожная платформа", "ж/д_платф"},
            {"жилая зона", "жилзона"},
            {"кордон", "кордон"},
            {"починок", "починок"},
            {"слобода", "сл"},
            {"садовое товарищество", "снт"},
            {"станция", "ст"},
            {"улус", "у"},
            {"аул", "аул"},
            {"деревня", "д"},
            {"железнодорожный разъезд", "ж/д рзд"},
            {"личное подсобное хозяйство", "лпх"},
            {"микрорайон", "мкр"},
            {"поселок разъезда", "пос.рзд."},
            {"городок", "г-к"},
            {"железнодорожная казарма", "ж/д_казарм"},
            {"железнодорожный пост", "ж/д_пост"},
            {"железнодорожная станция", "ж/д_ст"},
            {"заимка", "заимка"},
            {"казарма", "казарма"},
            {"остров", "остров"},
            {"поселок станции", "п. ст."},
            {"промышленная зона", "промзона"},
            {"хутор", "х"},
            {"железнодорожная блокпост", "ж/д бл-ст"},
            {"железнодорожная будка", "ж/д_будка"},
            {"жилой район", "жилрайон"},
            {"квартал", "кв-л"},
            {"коттеджный поселок", "кп"},
            {"разъезд", "рзд"},
            {"железнодорожный остановочный пункт", "ж/д_оп"},
            {"местечко", "м"},
            {"населенный пункт", "нп"},
            {"станица", "ст-ца"},
            {"арбан", "арбан"},
            {"планировочный район", "пл.р-н"},
            {"погост", "погост"},
            {"поселок железнодорожной станции", "п. ж/д ст."},
    
            // Level 7-8
            {"аллея", "ал."},
            {"гаражный кооператив", "гск"},
            {"зона", "зона"},
            {"местность", "местность"},
            {"проспект", "пр-кт"},
            {"территория огороднического товарищества", "тер. ОНТ"},
            {"территория садоводческого кооператива", "тер. СПК"},
            {"улица", "ул"},
            {"усадьба", "ус."},
            {"абонентский ящик", "а/я"},
            {"дачное товарищество", "днп"},
            {"километр", "км"},
            {"месторождение", "месторожд."},
            {"набережная", "наб"},
            {"парк", "парк"},
            {"площадь", "пл"},
            {"платформа", "платф"},
            {"территория потребительского кооператива", "тер. ПК"},
            {"порт", "порт"},
            {"строение", "стр"},
            {"территория товарищества", "тер. ТСН"},
            {"территория фермерского хозяйства", "тер.ф.х."},
            {"тупик", "туп"},
            {"бульвар", "б-р"},
            {"площадка", "пл-ка"},
            {"проезд", "пр-д"},
            {"сквер", "сквер"},
            {"территория дачного товарищества", "тер. ДНТ"},
            {"территория огороднического объединения", "тер. ОНО"},
            {"территория огороднического кооператива", "тер. ОПК"},
            {"территория садового объединения", "тер. СНО"},
            {"территория садового товарищества", "тер. СНТ"},
            {"территория товарищества собственников жилья", "тер. ТСЖ"},
            {"вал", "вал"},
            {"дорога", "дор"},
            {"сад", "сад"},
            {"территория дачного объединения", "тер. ДНО"},
            {"берег", "берег"},
            {"переулок", "пер"},
            {"территория гаражного кооператива", "тер. ГСК"},
            {"территория дачного кооператива", "тер. ДПК"},
            {"фермерское хозяйство", "ф/х"},
            {"шоссе", "ш"},
            {"заезд", "заезд"},
            {"проселок", "проселок"},
            {"тракт", "тракт"},
            {"мост", "мост"},
            {"просека", "просек"},
            {"железнодорожная ветка", "жт"},
            {"кольцо", "кольцо"},
            {"спуск", "спуск"},
            {"переезд", "переезд"},
            {"проулок", "пр-лок"},
            {"ряд", "ряд"},
            {"ряды", "ряды"},
            {"коса", "коса"},
            {"линия", "линия"},
            {"ферма", "ферма"},
            {"взвоз", "взв."},
            {"въезд", "въезд"},
            {"магистраль", "мгстр."},
            {"съезд", "сзд."},
            {"участок", "уч-к"},
        };
        #endregion

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public GarService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        /// <summary> Поиск адресов по ГАР </summary>
        /// <param name="query">Строка поиска</param>
        public async Task<IEnumerable<AddressSearchResultDto>> SearchAddressAsync(string query, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Enumerable.Empty<AddressSearchResultDto>();

            string[] searchTerms = query.ToLower()
                .Split(new[] { ' ', ',', '-', '/', '.' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(term => term.Length >= 2)
                .ToArray();

            if (searchTerms.Length == 0) return Enumerable.Empty<AddressSearchResultDto>();

            string[] distinctTerms = searchTerms
                .Select(t =>
                {
                    var lower = t.Trim();
                    return TypeToSearch.TryGetValue(lower, out var abbr) ? abbr : lower;
                })
                .Distinct()
                .ToArray();

            if (distinctTerms.Length == 0) return Enumerable.Empty<AddressSearchResultDto>();

            string hardQuery = string.Join(" & ", distinctTerms.Select(t => $"{t}:*")); // первая попытка — строгий поиск (AND)
            var results = await ExecuteSearch(hardQuery, 10, ct);

            if (results.Count == 0 && distinctTerms.Length > 1) // вторая попытка - мягкий поиск (OR)
            {
                var orQuery = string.Join(" | ", distinctTerms.Select(t => $"{t}:*"));
                results = await ExecuteSearch(orQuery, 10, ct);
            }

            return results;
        }

        private async Task<List<AddressSearchResultDto>> ExecuteSearch(string tsQueryString, int limit, CancellationToken ct)
        {
            await _semaphore.WaitAsync(ct);

            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                var rawData = await context.ClassifierGars
                    .AsNoTracking()
                    .Where(a => a.FullAddress != null &&
                                EF.Functions.ToTsVector("russian", a.FullAddress)
                                    .Matches(EF.Functions.ToTsQuery("russian", tsQueryString)))
                    .Select(x => new
                    {
                        x.ObjectId,
                        x.FullAddress,
                        x.PostalIndex,
                        x.Level,
                        Rank = EF.Functions.ToTsVector("russian", x.FullAddress)
                            .RankCoverDensity(EF.Functions.ToTsQuery("russian", tsQueryString),
                                              NpgsqlTsRankingNormalization.DivideByItselfPlusOne)
                    })
                    .OrderByDescending(x => x.Rank)
                    .ThenBy(x => x.Level)
                    .Take(limit)
                    .ToListAsync(ct);

                return rawData.Select(x => new AddressSearchResultDto
                {
                    ObjectId = x.ObjectId,
                    FullAddress = x.FullAddress,
                    PostalIndex = x.PostalIndex,
                }).ToList();
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
