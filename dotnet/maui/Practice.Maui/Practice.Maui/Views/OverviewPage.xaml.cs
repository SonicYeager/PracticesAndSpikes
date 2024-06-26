﻿using Practice.Maui.ViewModels;

namespace Practice.Maui.Views;

public sealed partial class OverviewPage : ContentPage
{
    public OverviewPage(OverviewViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }

    private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
    {
        FuelStops.SelectedItem = null;
    }
}