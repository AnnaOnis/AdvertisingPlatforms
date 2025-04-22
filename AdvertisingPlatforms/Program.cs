using AdvertisingPlatforms.DAL;
using AdvertisingPlatforms.Domain.Entities;
using AdvertisingPlatforms.Domain.Interfaces;
using AdvertisingPlatforms.Domain.Repositories;
using AdvertisingPlatforms.Domain.Validators;
using AdvertisingPlatforms.Parser;
using AdvertisingPlatforms.Services;
using AdvertisingPlatforms.Web.Extensions;

namespace AdvertisingPlatforms
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddApplicationServices();

            var app = builder.Build();

            app.UseApplicationMiddleware();

            app.Run();
        }
    }
}
