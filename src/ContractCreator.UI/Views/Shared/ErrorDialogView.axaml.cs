using Avalonia.Controls;

namespace ContractCreator.UI.Views.Shared;

public partial class ErrorDialogView : Window
{
    public ErrorDialogView()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is ErrorDialogViewModel viewModel)
        {
            viewModel.RequestClose += () => Close();
        }
    }
}