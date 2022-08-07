using MauiAppTesty.ViewModels;
using Microsoft.Extensions.Logging;

namespace MauiAppTesty.Views;

public partial class MailPage : ContentPage
{
    private readonly ILogger<MailPage> _logger;
    private readonly MailPageViewModel _viewModel = new();

    public MailPage(ILogger<MailPage> logger)
    {
        InitializeComponent();
        _logger = logger;

        BindingContext = _viewModel;
    }
}