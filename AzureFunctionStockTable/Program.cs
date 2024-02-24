using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NextNews.Data;
using NextNews.Services;
using Stripe;

var host = new HostBuilder()

    .ConfigureAppConfiguration(builder=>
    builder.AddJsonFile("local.settings.json", true,true)
    )

    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context,services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddHttpClient("business", config =>
        {
            // Access the configuration using the context parameter
            string apiBaseAddress = context.Configuration["MyStockAPIAddress"];
            if (string.IsNullOrEmpty(apiBaseAddress))
            {
                throw new InvalidOperationException("The MyStockAPIAddress configuration must be set.");
            }
            config.BaseAddress = new Uri(apiBaseAddress);
        });
        services.AddScoped<IStockService, StockService>();
        services.AddDbContext<ApplicationDbContext>(options =>
         options.UseSqlServer(Environment.GetEnvironmentVariable("DefaultConnection")));

    })

    .Build();

host.Run();
