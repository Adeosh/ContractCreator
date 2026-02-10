using Avalonia.Controls;
using ContractCreator.UI.ViewModels.UserControls;

namespace ContractCreator.UI.Views.UserControls;

public partial class AddressControl : UserControl
{
    public AddressControl()
    {
        InitializeComponent();
    }

    private void OnAddressSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox listBox &&
            listBox.SelectedItem is AddressSearchResultDto selectedDto &&
            DataContext is AddressViewModel vm)
        {
            listBox.SelectedItem = null;
            vm.SelectAddress(selectedDto);
        }
    }
}