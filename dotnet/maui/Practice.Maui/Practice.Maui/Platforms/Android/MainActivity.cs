using Android.App;
using Android.Content.PM;
using Android.OS;
using Practice.Maui.Application;
using Practice.Maui.Application.Services;

namespace Practice.Maui;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout |
                           ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
[IntentFilter(
    [Android.Content.Intent.ActionView],
    Categories =
    [
        Android.Content.Intent.CategoryDefault,
        Android.Content.Intent.CategoryBrowsable,
    ],
    DataScheme = "com.cookaperture.practice.maui"
)]
public sealed class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        var rawUri = Intent?.Data?.ToString();
        if (rawUri == null) return;
        LaunchUriHandler.TryHandle(new(rawUri));
    }
}