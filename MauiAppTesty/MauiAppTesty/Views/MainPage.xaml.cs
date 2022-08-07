using Microsoft.Extensions.Logging;
using System.ComponentModel;
using static Java.Util.Jar.Attributes;

namespace MauiAppTesty;

public partial class MainPage : ContentPage
{
	private readonly ILogger<MainPage> _logger;

    private string _senderField = "";
	public BindableProperty<string> SenderField {
        get
        {
            return _senderField;
        }
        private set
        {
            if (_senderField != value)
            {
                _senderField = value;
                Proper this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
            }
        }
    }

	public MainPage(ILogger<MainPage> logger)
	{
		InitializeComponent();
		_logger = logger;

		BindingContext = this;

		
	}
}

