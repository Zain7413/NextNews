using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NextNews.Data;
using NextNews.Helper;
using NextNews.Services;

var host = new HostBuilder()
    .ConfigureAppConfiguration(builder =>
    builder.AddJsonFile("local.settings.json", true, true)
    )
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddDbContext<ApplicationDbContext>(options =>
          options.UseSqlServer(Environment.GetEnvironmentVariable("DefaultConnection")));
    })
    .Build();

host.Run();
