namespace ContractCreator.Shared.Interfaces
{
    public interface IAddress
    {
        string FullAddress { get; }
        string House { get; }
        string Building { get; }
        string Flat { get; }
        string PostalIndex { get; }
    }
}
