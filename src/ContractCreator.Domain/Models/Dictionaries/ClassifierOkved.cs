namespace ContractCreator.Domain.Models.Dictionaries
{
    /// <summary> ОКВЭД 2 — Общероссийский классификатор видов экономической деятельности </summary>
    /// <a href="https://classifikators.ru/okved"></a>
    public class ClassifierOkved
    {
        public int Id { get; set; }
        public required string Code { get; set; }
        public required string Name { get; set; }
    }
}
