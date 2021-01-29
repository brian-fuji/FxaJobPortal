using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using FxaPortal.Services;
using Radzen;

namespace FxaPortal
{
    public class Program
    {
        public static async Task Main(string[] args)
        {            
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            string baseUri = builder.Configuration["apiURL"];

            builder.Services
                    .AddBlazorise(options =>
                    {
                        options.ChangeTextOnKeyPress = true;
                    })
                    .AddSingleton<FileUploadService>()
                    .AddBootstrapProviders()
                    .AddFontAwesomeIcons()
                    .AddSingleton<BatchApiService>()
                    .AddScoped<DialogService>()
                    .AddScoped<NotificationService>();

            builder.Services.AddSingleton(new HttpClient
            {
                BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
            });

            var host = builder.Build();

            host.Services
              .UseBootstrapProviders()
              .UseFontAwesomeIcons();

            await host.RunAsync();

            //await builder.Build().RunAsync();
        }
    }
}
