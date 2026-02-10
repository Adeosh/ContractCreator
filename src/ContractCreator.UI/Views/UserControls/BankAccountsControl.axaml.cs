using Avalonia.Controls;
using ContractCreator.UI.ViewModels.UserControls;

namespace ContractCreator.UI.Views.UserControls;

public partial class BankAccountsControl : UserControl
{
    public BankAccountsControl()
    {
        InitializeComponent();
    }

    private void OnBankSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox listBox &&
            listBox.SelectedItem is BankDto selectedBank &&
            DataContext is BankAccountsViewModel vm)
        {
            listBox.SelectedItem = null;
            vm.SelectBankFromClassifier(selectedBank);
        }
    }
}