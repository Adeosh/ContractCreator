namespace ContractCreator.Domain.Models.Dictionaries
{
    /// <summary> ОКОПФ — Общероссийский классификатор организационно-правовых форм </summary>
    /// <a href="https://classifikators.ru/okopf"></a>
    public class ClassifierOkopf
    {
        public int Id { get; set; }
        public required string Code { get; set; }
        public required string Name { get; set; }
    }
}
