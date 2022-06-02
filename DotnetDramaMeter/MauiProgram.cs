using DotnetDramaMeter.View;
using DotnetDramaMeter.ViewModel;

namespace DotnetDramaMeter;

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
                fonts.AddFont("OpenSans-BoldItalic.ttf", "OpenSansBoldItalic");
                fonts.AddFont("OpenSans-Bold.ttf", "OpenSansBold");
            });

        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddSingleton<MainViewModel>();

        return builder.Build();
    }
}