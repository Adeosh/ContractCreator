namespace ContractCreator.Shared.Interfaces
{
    /// <summary>
	/// Интерфейс указывающий на наличие полей для ФИО (<see cref="FirstName"/>, <see cref="LastName"/>, <see cref="MiddleName"/>,  )
	/// </summary>
	public interface IFio
    {
        /// <summary> Имя </summary>
        string FirstName { get; set; }
        /// <summary> Фамилия </summary>
        string LastName { get; set; }
        /// <summary> Отчество </summary>
        string MiddleName { get; set; }
    }
}
