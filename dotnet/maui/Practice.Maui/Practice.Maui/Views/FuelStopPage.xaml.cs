using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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