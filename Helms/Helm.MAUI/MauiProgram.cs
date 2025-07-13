using Helm.Shared.Services;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;

namespace Helm.MAUI
{
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
            builder.Services.AddMudServices();

            string castleAddress = @"http://localhost:5000/";
            //string castleAddress = @"https://localhost:7058/"; 
            //string castleAddress = @"http://localhost:5203/";
            builder.Services.AddSingleton<CommanderService>(castleUrl => new CommanderService(castleAddress));
            builder.Services.AddSingleton<KnightService>(castleUrl => new KnightService(castleAddress));
            //builder.Services.AddSingleton<CastleService>(castleUrl => new CastleService(castleAddress));

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
