namespace ContractCreator.UI.Services.Navigation
{
    public enum EditorMode
    {
        Create,
        Edit
    }

    public class EditorParams
    {
        public EditorMode Mode { get; set; }

        /// <summary>
        /// Id сущности, которую редактируем (например, WorkerId).
        /// При создании равен 0.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Id родителя (например, FirmId), к которому привязываем новую запись.
        /// </summary>
        public int ParentId { get; set; }
    }
}
