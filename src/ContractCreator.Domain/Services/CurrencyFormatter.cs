using ContractCreator.Domain.Models.Dictionaries;
using System.Globalization;

namespace ContractCreator.Domain.Services
{
    public static class CurrencyFormatter
    {
        /// <summary>
        /// Преобразует сумму в слова (прописью)
        /// </summary>
        /// <param name="amount">Сумма</param>
        /// <param name="currencyId">Id валюты из <see cref="ClassifierOkv"/></param>
        /// <returns>Сумма прописью</returns>
        public static string AmountToWords(decimal amount, int currencyId)
        {
            int mainUnit = (int)Math.Floor(amount);
            int subUnit = (int)Math.Round((amount - mainUnit) * 100);

            var currencyInfo = GetCurrencyInfo(currencyId);

            string mainUnitWord = GetMainUnitWord(mainUnit, currencyInfo);
            string subUnitWord = GetSubUnitWord(subUnit, currencyInfo);
            string mainUnitInWords = ConvertNumberToWords(mainUnit, currencyInfo.IsFeminine);

            return $"{mainUnitInWords} {mainUnitWord} {subUnit:00} {subUnitWord}";
        }

        /// <summary>
        /// Преобразует сумму в текст, а так же добавляет НДС
        /// </summary>
        /// <param name="amount">Сумма</param>
        /// <param name="currencyId">Id валюты из <see cref="ClassifierOkv"/></param>
        /// <param name="isVATApplicable">НДС (есть или нет)</param>
        /// <returns></returns>
        public static string PriceText(decimal amount, int currencyId, bool isVATApplicable)
        {
            var formatInfo = new NumberFormatInfo
            {
                NumberGroupSeparator = " ",
                NumberDecimalDigits = 0
            };
            string formattedNumber = amount.ToString("#,0", formatInfo);

            int mainUnit = (int)Math.Floor(amount);
            var currencyInfo = GetCurrencyInfo(currencyId);
            string mainUnitInWords = ConvertNumberToWords(mainUnit, currencyInfo.IsFeminine);

            mainUnitInWords = char.ToUpper(mainUnitInWords[0]) + mainUnitInWords.Substring(1);

            string mainUnitWord = GetMainUnitWord(mainUnit, currencyInfo);
            string vatText = isVATApplicable ? "облагается НДС" : "НДС не облагается";

            return $"{formattedNumber} ({mainUnitInWords}) {mainUnitWord}, {vatText}";
        }

        /// <summary>
        /// Получает сокращенное название валюты
        /// </summary>
        /// <param name="currencyId">Id валюты из <see cref="ClassifierOkv"/></param>
        /// <returns>Сокращенное название, например "руб.", "долл."</returns>
        public static string GetCurrencyShortName(int currencyId)
        {
            var currencyInfo = GetCurrencyInfo(currencyId);
            return currencyInfo.ShortName;
        }

        /// <summary>
        /// Получает символ валюты
        /// </summary>
        /// <param name="currencyId">Id валюты из <see cref="ClassifierOkv"/></param>
        /// <returns>Символ валюты, например "₽", "$", "€"</returns>
        public static string GetCurrencySymbol(int currencyId)
        {
            var currencyInfo = GetCurrencyInfo(currencyId);
            return currencyInfo.Symbol;
        }

        /// <summary> Форматирует сумму с сокращенным обозначением валюты </summary>
        public static string ShortFormatCurrency(decimal amount, int currencyId, bool useSymbol = false, int decimals = 2)
        {
            var currencyInfo = GetCurrencyInfo(currencyId);
            string format = $"{{0:N{decimals}}}";
            string currencyDisplay = useSymbol ? currencyInfo.Symbol : currencyInfo.ShortName;
            return string.Format($"{format} {currencyDisplay}", amount);
        }

        private static CurrencyInfo GetCurrencyInfo(int currencyId)
        {
            return currencyId switch
            {
                ClassifierOkv.RUB => new CurrencyInfo
                {
                    IsFeminine = false,
                    MainUnit = new[] { "рубль", "рубля", "рублей" },
                    SubUnit = new[] { "копейка", "копейки", "копеек" },
                    ShortName = "руб.",
                    Symbol = "₽"
                },
                ClassifierOkv.USD => new CurrencyInfo
                {
                    IsFeminine = false,
                    MainUnit = new[] { "доллар", "доллара", "долларов" },
                    SubUnit = new[] { "цент", "цента", "центов" },
                    ShortName = "долл.",
                    Symbol = "$"
                },
                ClassifierOkv.EUR => new CurrencyInfo
                {
                    IsFeminine = false,
                    MainUnit = new[] { "евро", "евро", "евро" },
                    SubUnit = new[] { "цент", "цента", "центов" },
                    ShortName = "евро",
                    Symbol = "€"
                },
                ClassifierOkv.KZT => new CurrencyInfo
                {
                    IsFeminine = false,
                    MainUnit = new[] { "тенге", "тенге", "тенге" },
                    SubUnit = new[] { "тиын", "тиына", "тиынов" },
                    ShortName = "тнг.",
                    Symbol = "₸"
                },
                ClassifierOkv.BYN => new CurrencyInfo
                {
                    IsFeminine = false,
                    MainUnit = new[] { "белорусский рубль", "белорусских рубля", "белорусских рублей" },
                    SubUnit = new[] { "копейка", "копейки", "копеек" },
                    ShortName = "бел. руб.",
                    Symbol = "Br"
                },
                _ => new CurrencyInfo
                {
                    IsFeminine = false,
                    MainUnit = new[] { "рубль", "рубля", "рублей" },
                    SubUnit = new[] { "копейка", "копейки", "копеек" },
                    ShortName = "руб.",
                    Symbol = "₽"
                }
            };
        }

        private static string GetMainUnitWord(int number, CurrencyInfo info)
        {
            int lastDigit = number % 10;
            int lastTwoDigits = number % 100;

            if (lastTwoDigits >= 11 && lastTwoDigits <= 14)
                return info.MainUnit[2]; // "рублей"

            return lastDigit switch
            {
                1 => info.MainUnit[0], // "рубль"
                2 or 3 or 4 => info.MainUnit[1], // "рубля"
                _ => info.MainUnit[2] // "рублей"
            };
        }

        private static string GetSubUnitWord(int number, CurrencyInfo info)
        {
            int lastDigit = number % 10;
            int lastTwoDigits = number % 100;

            if (lastTwoDigits >= 11 && lastTwoDigits <= 14)
                return info.SubUnit[2];

            return lastDigit switch
            {
                1 => info.SubUnit[0],
                2 or 3 or 4 => info.SubUnit[1],
                _ => info.SubUnit[2]
            };
        }

        private static string ConvertNumberToWords(int number, bool isFeminine = false)
        {
            if (number == 0)
                return "ноль";

            if (number < 0)
                return "минус " + ConvertNumberToWords(-number, isFeminine);

            string[] units = isFeminine
                ? new[] { "", "одна", "две", "три", "четыре", "пять", "шесть", "семь", "восемь", "девять" }
                : new[] { "", "один", "два", "три", "четыре", "пять", "шесть", "семь", "восемь", "девять" };

            string[] teens = { "десять", "одиннадцать", "двенадцать", "тринадцать", "четырнадцать",
                              "пятнадцать", "шестнадцать", "семнадцать", "восемнадцать", "девятнадцать" };

            string[] tens = { "", "", "двадцать", "тридцать", "сорок", "пятьдесят",
                             "шестьдесят", "семьдесят", "восемьдесят", "девяносто" };

            string[] hundreds = { "", "сто", "двести", "триста", "четыреста",
                                 "пятьсот", "шестьсот", "семьсот", "восемьсот", "девятьсот" };

            if (number < 10)
                return units[number];

            if (number < 20)
                return teens[number - 10];

            if (number < 100)
            {
                int tensDigit = number / 10;
                int unitsDigit = number % 10;
                return tens[tensDigit] + (unitsDigit > 0 ? " " + units[unitsDigit] : "");
            }

            if (number < 1000)
            {
                int hundredsDigit = number / 100;
                int remainder = number % 100;
                return hundreds[hundredsDigit] + (remainder > 0 ? " " + ConvertNumberToWords(remainder, isFeminine) : "");
            }

            if (number < 1000000)
            {
                int thousands = number / 1000;
                int remainder = number % 1000;

                string thousandsWord = GetThousandsWord(thousands);
                string thousandsText = ConvertNumberToWords(thousands, true) + " " + thousandsWord;

                return thousandsText + (remainder > 0 ? " " + ConvertNumberToWords(remainder, isFeminine) : "");
            }

            if (number < 1000000000)
            {
                int millions = number / 1000000;
                int remainder = number % 1000000;

                string millionsWord = GetMillionsWord(millions);
                string millionsText = ConvertNumberToWords(millions, false) + " " + millionsWord;

                return millionsText + (remainder > 0 ? " " + ConvertNumberToWords(remainder, isFeminine) : "");
            }

            return number.ToString();
        }

        private static string GetThousandsWord(int number)
        {
            int lastDigit = number % 10;
            int lastTwoDigits = number % 100;

            if (lastTwoDigits >= 11 && lastTwoDigits <= 14)
                return "тысяч";

            return lastDigit switch
            {
                1 => "тысяча",
                2 or 3 or 4 => "тысячи",
                _ => "тысяч"
            };
        }

        private static string GetMillionsWord(int number)
        {
            int lastDigit = number % 10;
            int lastTwoDigits = number % 100;

            if (lastTwoDigits >= 11 && lastTwoDigits <= 14)
                return "миллионов";

            return lastDigit switch
            {
                1 => "миллион",
                2 or 3 or 4 => "миллиона",
                _ => "миллионов"
            };
        }
    }

    internal sealed class CurrencyInfo
    {
        /// <summary>
        /// Определяет род числительных при преобразовании в слова.<br/>
        /// true - женский род, используется для валют женского рода и тысяч.<br/>
        /// false - мужской род, используется для большинства валют.
        /// </summary>
        public bool IsFeminine { get; set; }
        /// <summary> Массив склонений основной единицы валюты для согласования с числительными.<br/>  
        /// Пример: ["рубль", "рубля", "рублей"]
        /// </summary>
        public string[] MainUnit { get; set; }
        /// <summary> 
        /// Массив склонений дробной единицы валюты для согласования с числительными.<br/> 
        /// Пример: ["копейка", "копейки", "копеек"]
        /// </summary>
        public string[] SubUnit { get; set; }
        /// <summary>
        /// Сокращенное обозначение валюты.<br/>
        /// Пример: "руб.", "долл."
        /// </summary>
        public string ShortName { get; set; }
        /// <summary>
        /// Символ валюты.<br/>
        /// Пример: "₽", "$", "€"
        /// </summary>
        public string Symbol { get; set; }
    }
}
