namespace ContractCreator.UI.Services.Navigation
{
    public class InMemoryEditorParams<T> where T : class
    {
        public ObservableCollection<T> TargetCollection { get; set; } = null!;
        public T? ItemToEdit { get; set; }
    }
}
