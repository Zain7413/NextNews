using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NextNews.Data;
using NextNews.Services;

var host = new HostBuilder()
    .ConfigureAppConfiguration(builder=>
    builder.AddJsonFile("local.settings.json",true, true)
    )
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddScoped<IArticleService, ArticleService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddDbContext<ApplicationDbContext>(options =>
           options.UseSqlServer(Environment.GetEnvironmentVariable("DefaultConnection")));
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    })
    .Build();

host.Run();
