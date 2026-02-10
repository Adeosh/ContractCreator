using Avalonia.Controls;

namespace ContractCreator.UI.Views.Shared;

public partial class UnifiedDialogView : Window
{
    public UnifiedDialogView()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (DataContext is UnifiedDialogViewModel vm)
            vm.OnCloseRequest = () => Close();
    }
}