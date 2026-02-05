namespace ContractCreator.Domain.Models.Dictionaries
{
    /// <summary> Государственный адресный реестр </summary>
    /// <a href="https://fias.nalog.ru/Frontend"></a>
    public class ClassifierGar
    {
        public int Id { get; set; }
        public long ObjectId { get; set; }
        public Guid? ObjectGuid { get; set; }
        public int RegionCode { get; set; }
        public required string Name { get; set; }
        public required string TypeName { get; set; }
        public short Level { get; set; }
        public required string FullAddress { get; set; }
        public bool LiveInTown { get; set; }
        public string? PostalIndex { get; set; }
    }
}
