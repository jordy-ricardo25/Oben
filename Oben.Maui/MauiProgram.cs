using Microsoft.Extensions.Logging;
using Oben.Maui.Services;

namespace Oben.Maui;

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

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddSingleton(new ApiOptions("http://localhost:5277/"));
        builder.Services.AddSingleton<SessionState>();
        builder.Services.AddScoped<ObenApiClient>();
        builder.Services.AddScoped(sp =>
        {
            var options = sp.GetRequiredService<ApiOptions>();

            return new HttpClient
            {
                BaseAddress = options.BaseAddress
            };
        });

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
