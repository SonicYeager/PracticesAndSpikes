using Practice.Maui.ViewModels;

namespace Practice.Maui.Views;

public partial class FuelStopPage : ContentPage
{
    public FuelStopPage(FuelStopEntryViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}