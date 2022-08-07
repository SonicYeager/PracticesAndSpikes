using MauiAppTesty.Views;
using Microsoft.Extensions.Logging;

namespace MauiAppTesty;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		builder.Services.AddLogging(o =>
		{
			o.AddConsole();
		});
		builder.Services
			.AddTransient<MailPage>()
			.AddTransient<StandartPage>()
			.AddTransient<FlyoutFooter>()
			.AddTransient<FlyoutHeader>();

		return builder.Build();
	}
}
