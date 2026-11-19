using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OLP.Application;
using OLP.Application.Contracts.Services;
using OLP.Application.Services;
using OLP.Domain.Entities;
using OLP.Infrastructure.Data;
using OLP.Infrastructure;

namespace OLP
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddInfrastructure(builder.Configuration);

            builder.Services.AddControllers();

            builder.Services.AddApplication();

            builder.Services.AddInfrastructure(builder.Configuration);

            // Simulated payments
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddSingleton<IPaymentGateway, MockPaymentGateway>();
                builder.Services.AddScoped<IPaymentService, PaymentService>();
            }

            builder.Host.UseDefaultServiceProvider(options =>
            {
                options.ValidateScopes = true;
                options.ValidateOnBuild = true;
            });

            var app = builder.Build();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
