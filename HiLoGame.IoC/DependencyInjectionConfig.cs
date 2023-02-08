using AutoMapper;
using HiLoGame.Crosscutting.Interfaces;
using HiLoGame.Crosscutting.Mapping;
using HiLoGame.Repository;
using HiLoGame.Repository.Storage;
using HiLoGame.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace HiLoGame.IoC
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjectionConfig
    {
        //The services and repository were created as transient because they are lightweight objects that can be easily created and discarded
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddTransient<IGameService, GameService>();
            services.AddTransient<IPlayerService, PlayerService>();
        }

        public static void ConfigureRepositories(this IServiceCollection services)
        {
            services.AddTransient<IGameRepository, GameRepository>();
            services.AddTransient<IGamePlayerInfoRepository, GamePlayerInfoRepository>();
            services.AddTransient<IPlayerRepository, PlayerRepository>();
        }

        public static void ConfigureAutoMapper(this IServiceCollection services)
        {
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new GameMappingProfile());
                mc.AddProfile(new PlayerMappingProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();

            services.AddSingleton(mapper);
        }

        public static IServiceCollection ConfigureDbContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<Context>(options => options.UseSqlServer(connectionString));

            return services;
        }
    }
}
