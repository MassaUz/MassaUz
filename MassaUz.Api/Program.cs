
using Microsoft.AspNetCore.Identity;
using MassUz.Application;
using MassaUz.Infrastructure;
using MassaUz.Infrastructure.Persistance;
using Serilog;
//using MassaUz.Domain.Entities.Auth;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                       .AllowAnyHeader()
                       .AllowAnyMethod();

            });
        });
        // Add services to the container.

        //builder.Services.AddApplicationServices();
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .CreateLogger();
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog();
            builder.Services.AddControllers();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseCors();
        app.UseStaticFiles();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        using (var scope = app.Services.CreateScope())
        {
            var roleManager =
                scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            var roles = new[] { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!roleManager.RoleExistsAsync(role).Result)
                    roleManager.CreateAsync(new IdentityRole<Guid>(role)).Wait();
            }
        }

        using (var scope = app.Services.CreateScope())
        {
            var userManager =
                scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            string email = "admintoga@massaUz.com";
            string password = "Test123!";

            if (userManager.FindByEmailAsync(email).Result == null)
            {
                //var user = new User()
                //{
                //    UserName = "Admin",
                //    Name = "Admin",
                //    Surname = "Admin",
                //    Email = email,
                //    Password = password,
                //    PhoneNumber = "+998958136252",
                //    Role = "Admin"
                //};

                //userManager.CreateAsync(user, password).Wait();

                //userManager.AddToRoleAsync(user, "Admin").Wait();
            }
        }

        app.Run();
    }
}