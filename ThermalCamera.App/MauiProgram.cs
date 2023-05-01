using ThermalCamera.App.Data;
using ThermalCamera.App.Data.Interfaces;

namespace ThermalCamera.App;

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
            });

        builder.Services.AddBlazorWebView();
        builder.Services.AddSingleton<IUsbConnectionService, UsbConnectionService>();

        return builder.Build();
    }
}
