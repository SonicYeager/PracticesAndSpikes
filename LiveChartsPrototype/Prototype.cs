using Avalonia;
using Avalonia.Controls;

namespace Prototype
{
    public static class PrototypeApp
    {
        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>().UsePlatformDetect().LogToTrace();
        public static void Init(string[] args, OnReady onReadyDelegate)
        {
            // Initialization code. Don't use any Avalonia, third-party APIs or any
            // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
            // yet and stuff might break.
            onReady += onReadyDelegate;
            AppBuilder = BuildAvaloniaApp();
            AppBuilder.Start(AppMain, args);
        }

        static void AppMain(Application app, string[] args)
        {
            // Do you startup code here
            MainUI = onReady?.Invoke();
            MainUI.Show();
            Application = app;

            // Start the main loop
            app.Run(MainUI);
        }

        public static Window MainUI { get; set; }
        public static AppBuilder AppBuilder { get; set; }
        public static Application Application { get; set; }

        public delegate Window OnReady();
        public static event OnReady onReady;
    }
}