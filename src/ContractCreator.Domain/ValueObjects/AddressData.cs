namespace ContractCreator.Domain.ValueObjects
{
    public sealed class AddressData
    {
        /// <summary>Ссылка на ObjectId по ГАР <</summary>
        public long ObjectId { get; set; }
        /// <summary>Полная строка адреса до улицы</summary>
        public string FullAddress { get; set; } = string.Empty;
        /// <summary> Дом </summary>
        public string House { get; set; } = string.Empty;
        /// <summary> Строение/Корпус </summary>
        public string Building { get; set; } = string.Empty;
        /// <summary> Квартира/Офис </summary>
        public string Flat { get; set; } = string.Empty;
        /// <summary> Почтовый индекс </summary>
        public string PostalIndex { get; set; } = string.Empty;
    }
}
