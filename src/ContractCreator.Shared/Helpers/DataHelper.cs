using ContractCreator.Shared.Interfaces;

namespace ContractCreator.Shared.Helpers
{
    public static class DataHelper
    {
        private static readonly HashSet<string> Vowels = new HashSet<string> { "а", "е", "ё", "и", "о", "у", "ы", "э", "ю", "я" };

        /// <summary>
        /// Собирает строку ФИО, из данных переданного объекта, в указанном формате
        /// </summary>
        /// <param name="fioOwner">Объект реализующий интерфейс <see cref="IFio"/> с которого будут взяты фамилия имя отчество</param>
        /// <param name="additionalInfo">Дополнительная информация которая будет указана после ФИО (например СНИЛС, Название отделения, Специальность и т.п.)</param>
        /// <param name="format">Целевой формат <list type="table">
        /// <item> <term>f</term> <description>Полное ФИО (FullName) (например: Иванов Иван Иванович)</description> </item>
        /// <item> <term>s</term> <description>Фамилия и инициалы (ShortName) (например: Иванов И.И.)</description> </item></list>
        /// </param>
        /// <remarks>
        /// <para>Поддерживаемые форматы:</para>
        /// <list type="table">
        /// <item> <term>f</term> <description>Полное ФИО (FullName) (например: Иванов Иван Иванович)</description> </item>
        /// <item> <term>s</term> <description>Фамилия и инициалы (ShortName) (например: Иванов И.И.)</description> </item>
        /// </list>
        /// </remarks>
        /// <returns>ФИО в заданном формате одной строкой</returns>
        public static string CreateFIOString(IFio fioOwner, string format, string? additionalInfo = null)
        {
            return CreateFIOString(fioOwner.LastName, fioOwner.FirstName, fioOwner?.MiddleName ?? string.Empty, format, additionalInfo);
        }

        /// <summary>
        /// Собирает строку ФИО, из переданных элементов, в указанном формате
        /// </summary>
        /// <param name="sName">Фамилия</param>
        /// <param name="name">Имя</param>
        /// <param name="fName">Отчество</param>
        /// <param name="additionalInfo">Дополнительная информация которая будет указана после ФИО (например СНИЛС, Название отделения, Специальность и т.п.)</param>
        /// <param name="format">Целевой формат <list type="table">
        /// <item> <term>f</term> <description>Полное ФИО (FullName) (например: Иванов Иван Иванович)</description> </item>
        /// <item> <term>s</term> <description>Фамилия и инициалы (ShortName) (например: Иванов И.И.)</description> </item></list>
        /// </param>
        /// <remarks>
        /// <para>Поддерживаемые форматы:</para>
        /// <list type="table">
        /// <item> <term>f</term> <description>Полное ФИО (FullName) (например: Иванов Иван Иванович)</description> </item>
        /// <item> <term>s</term> <description>Фамилия и инициалы (ShortName) (например: Иванов И.И.)</description> </item>
        /// </list>
        /// </remarks>
        /// <returns>ФИО в заданном формате одной строкой</returns>
        public static string CreateFIOString(string sName, string name, string fName, string format, string? additionalInfo = null)
        {
            if (string.IsNullOrEmpty(format) == true)
            {
                format = "s";
            }

            string ret = string.Empty;

            switch (format.ToLower())
            {
                case "f": //FullName Полное ФИО
                    {
                        if (!string.IsNullOrEmpty(sName))
                            ret = sName;

                        if (!string.IsNullOrEmpty(name))
                        {
                            if (!string.IsNullOrEmpty(ret))
                                ret += " ";

                            ret += name;
                        }
                        if (!string.IsNullOrEmpty(fName))
                        {
                            if (!string.IsNullOrEmpty(ret))
                                ret += " ";

                            ret += fName;
                        }

                        break;
                    }
                case "s": //ShortName Фамилия и инициалы
                    {
                        if (!string.IsNullOrEmpty(sName))
                            ret = sName;

                        if (!string.IsNullOrEmpty(name))
                        {
                            if (!string.IsNullOrEmpty(ret))
                                ret += " ";

                            ret += name[0] + ".";
                        }
                        if (!string.IsNullOrEmpty(fName))
                        {
                            if (!string.IsNullOrEmpty(ret))
                            {
                                if (ret[ret.Length - 1] != '.')
                                    ret += " ";
                            }
                            ret += fName[0] + ".";
                        }
                        break;
                    }
                default:
                    throw new FormatException($"Неизвестный формат - \"{format}\"");
            }

            if (!string.IsNullOrEmpty(additionalInfo))
            {
                if (!string.IsNullOrEmpty(ret))
                    ret += " ";

                ret += additionalInfo;
            }

            return ret;
        }

        /// <summary>
        /// Попытка вернуть строку в родительном падеже
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static string ToGenitive(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
                return word;

            bool isCapitalized = char.IsUpper(word[0]); // Нормализация (работаем с нижним регистром для анализа, но возвращаем исходный кейс)
            string lowerWord = word.ToLower();
            int len = lowerWord.Length;

            if (len < 2) // Слишком короткие слова не трогаем (обычно аббревиатуры или предлоги)
                return word;

            string result = lowerWord;

            if (lowerWord.EndsWith("а")) // Существительные на -а / -я
            {
                char preLast = lowerWord[len - 2];
                if (IsHushingOrVelar(preLast))
                    result = lowerWord.Substring(0, len - 1) + "и";
                else
                    result = lowerWord.Substring(0, len - 1) + "ы";
            }
            else if (lowerWord.EndsWith("я"))
            {
                // Если оканчивается на -ия (Армия, Лаборатория) -> -ии
                if (lowerWord.EndsWith("ия"))
                {
                    result = lowerWord.Substring(0, len - 1) + "и";
                }
                else
                {
                    // Няня -> Няни, Земля -> Земли
                    result = lowerWord.Substring(0, len - 1) + "и";
                }
            }
            else if (!Vowels.Contains(lowerWord.Substring(len - 1)) && lowerWord.Last() != 'ь' && lowerWord.Last() != 'й') // Существительные на согласный, исключаем ь и й, они обрабатываются отдельно
            {
                result = lowerWord + "а";
            }
            else if (lowerWord.EndsWith("й")) // Существительные на -й
            {
                if (lowerWord.EndsWith("ий")) // Если -ий
                    result = lowerWord.Substring(0, len - 1) + "я";
                else
                    result = lowerWord.Substring(0, len - 1) + "я";
            }
            else if (lowerWord.EndsWith("ь")) // Существительные на -Ь
            {
                if (lowerWord.EndsWith("тель"))
                {
                    result = lowerWord.Substring(0, len - 1) + "я";
                }
                else
                {
                    if (lowerWord.EndsWith("арь")) // Попробуем обработать популярные мужские окончания на -арь
                        result = lowerWord.Substring(0, len - 1) + "я";
                    else
                        result = lowerWord.Substring(0, len - 1) + "и";
                }
            }
            else if (lowerWord.EndsWith("о")) // Попробуем обработать окончания на -о
            {
                result = lowerWord.Substring(0, len - 1) + "а";
            }
            else if (lowerWord.EndsWith("е")) // Попробуем обработать окончания на -е
            {
                if (lowerWord.EndsWith("ие"))
                    result = lowerWord.Substring(0, len - 1) + "я";
                else
                    result = lowerWord.Substring(0, len - 1) + "я";
            }

            return isCapitalized ? char.ToUpper(result[0]) + result.Substring(1) : result;

            /// <summary>
            /// Проверка на шипящие (ж, ш, ч, щ) и заднеязычные (г, к, х)
            /// </summary>
            bool IsHushingOrVelar(char c)
            {
                return "гкхжшчщ".IndexOf(c) >= 0;
            }
        }

        /// <summary>
        /// Формирует строку адреса из объекта AddressData(Json)
        /// </summary>
        /// <param name="address">Объект адреса</param>
        /// <param name="includeHouseBuildingFlat">Если true, добавляет дом, корпус и квартиру в строку</param>
        /// <returns>Адрес в виде одной строки</returns>
        public static string CreateFullAddressString(IAddress address, bool includeHouseBuildingFlat = true)
        {
            if (address == null)
                return string.Empty;

            List<string> parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(address.PostalIndex))
                parts.Add(address.PostalIndex.Trim());

            if (!string.IsNullOrWhiteSpace(address.FullAddress))
                parts.Add(address.FullAddress.Trim());

            if (includeHouseBuildingFlat)
            {
                if (!string.IsNullOrWhiteSpace(address.House))
                    parts.Add($"д. {address.House.Trim()}");

                if (!string.IsNullOrWhiteSpace(address.Building))
                    parts.Add($"корп. {address.Building.Trim()}");

                if (!string.IsNullOrWhiteSpace(address.Flat))
                    parts.Add($"кв./оф. {address.Flat.Trim()}");
            }

            return string.Join(", ", parts);
        }

        /// <summary>
        /// Формирует полную строку с данными организации
        /// </summary>
        /// <param name="fullName">Полное наименование организации</param>
        /// <param name="inn">ИНН</param>
        /// <param name="kpp">КПП (опционально)</param>
        /// <param name="ogrn">ОГРН(ИП)</param>
        /// <param name="erns">ЕГРС</param>
        /// <param name="oktmo">ОКТМО</param>
        /// <param name="okpo">ОКПО</param>
        /// <param name="address">Адрес организации</param>
        /// <param name="phone">Телефон</param>
        /// <param name="email">Email (опционально)</param>
        /// <returns>Отформатированная строка с данными организации</returns>
        public static string CreateOrganizationFullDataString(
            string fullName,
            string inn,
            string? kpp = null,
            string? ogrn = null,
            string? erns = null,
            string? oktmo = null,
            string? okpo = null,
            IAddress? address = null,
            string? phone = null,
            string? email = null)
        {
            if (string.IsNullOrEmpty(fullName))
                return string.Empty;

            List<string> lines = new List<string>();

            lines.Add(fullName);

            List<string> taxInfo = new List<string>();
            if (!string.IsNullOrEmpty(inn))
                taxInfo.Add($"ИНН {inn}");
            if (!string.IsNullOrEmpty(kpp))
                taxInfo.Add($"КПП {kpp}");
            if (!string.IsNullOrEmpty(ogrn))
                taxInfo.Add($"ОГРН(ИП) {ogrn}");
            if (!string.IsNullOrEmpty(erns))
                taxInfo.Add($"ЕРНС {erns}");
            if (!string.IsNullOrEmpty(oktmo))
                taxInfo.Add($"ОКТМО {oktmo}");
            if (!string.IsNullOrEmpty(okpo))
                taxInfo.Add($"ОКПО {okpo}");

            if (taxInfo.Any())
                lines.Add(string.Join(" / ", taxInfo));

            if (address != null)
            {
                string fullAddress = CreateFullAddressString(address);
                if (!string.IsNullOrEmpty(fullAddress))
                    lines.Add(fullAddress);
            }

            List<string> contacts = new List<string>();
            if (!string.IsNullOrEmpty(phone))
                contacts.Add($"Тел.: {phone}");
            if (!string.IsNullOrEmpty(email))
                contacts.Add($"Эл.почта: {email}");

            if (contacts.Any())
                lines.Add(string.Join(" | ", contacts));

            return string.Join(Environment.NewLine, lines);
        }

        /// <summary>
        /// Метод для форматирования банковских реквизитов строкой
        /// </summary>
        public static string CreateBankRequisitesString(
            string bankName,
            string bic,
            string correspondentAccountNumber,
            string paidAccountNumber,
            string? bankAddress = null)
        {
            if (string.IsNullOrEmpty(bankName) &&
                string.IsNullOrEmpty(paidAccountNumber))
                return string.Empty;

            List<string> lines = new List<string>();

            if (!string.IsNullOrEmpty(bankName))
                lines.Add($"Банк: {bankName}");

            if (!string.IsNullOrEmpty(bic))
                lines.Add($"БИК: {bic}");

            if (!string.IsNullOrEmpty(correspondentAccountNumber))
                lines.Add($"Кор/с: {correspondentAccountNumber}");

            if (!string.IsNullOrEmpty(paidAccountNumber))
                lines.Add($"Рас/с: {paidAccountNumber}");

            if (!string.IsNullOrEmpty(bankAddress))
                lines.Add($"Адрес банка: {bankAddress}");

            return string.Join(Environment.NewLine, lines);
        }
    }
}
