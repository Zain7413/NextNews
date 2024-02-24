using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

using NextNews.Data;
using NextNews.Helper;
using NextNews.Models.Database;
using NextNews.Services;
using Stripe;
using SubscriptionService = NextNews.Services.SubscriptionService;

namespace NextNews
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddHttpClient("business", config=>
            { config.BaseAddress = new(builder.Configuration["MyStockAPIAddress"]); 
            });

            builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddTransient<IEmailSender, EmailSender>();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            });


            builder.Services.AddControllersWithViews();




            builder.Services.AddHttpClient("forecast", config =>
            {
                config.BaseAddress = new Uri(builder.Configuration["MyWeatherAPIAddress"]);
            });




            // SERVICES
            builder.Services.AddScoped<IUserService,UserService>();
            builder.Services.AddScoped<IArticleService, ArticleService>();
            builder.Services.AddScoped<ICategoryService,CategoryService>();
            builder.Services.AddScoped<IRoleManagementService, RoleManagementService>();
            builder.Services.AddScoped<IStockService, StockService>();
            builder.Services.AddScoped<IContactService, ContactService>();
            builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();

            builder.Services.AddScoped<IStatisticService, StatisticService>();

            builder.Services.AddScoped<IWheatherService, WeatherService>();
            builder.Services.AddScoped<INewsLetterService, NewsLetterService>();

            builder.Services.AddScoped<SeedData>();

            
         

            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential 
                // cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;

                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.ConsentCookieValue = "true";

            });




            // Configure role-based policies
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
                options.AddPolicy("Editor", policy => policy.RequireRole("Editor"));
                options.AddPolicy("User", policy => policy.RequireRole("User"));

                options.AddPolicy("BasicUser", policy =>
                {
                    policy.RequireRole("Basic");
                });

                options.AddPolicy("PremiumUser", policy =>
                {
                    policy.RequireRole("Premium");


                });


             




            });

 
            var app = builder.Build();


            // Seed data during application program
            // Create a scope to resolve scoped services
            using (var scope = app.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;

                // Initialize seed data
                SeedData.InitializeAsync(serviceProvider).Wait();
            }


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

      



            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCookiePolicy();

            app.UseRouting();


            app.UseAuthentication(); // to enable authentication



            StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();




            app.Run();

        }
    }
}
