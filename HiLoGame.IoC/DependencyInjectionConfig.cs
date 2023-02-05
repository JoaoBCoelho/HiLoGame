using Amazon.Auth.AccessControlPolicy;
using AutoMapper;
using HiLoGame.Crosscutting.Interfaces;
using HiLoGame.Crosscutting.Mapping;
using HiLoGame.Repository;
using HiLoGame.Service;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Diagnostics.CodeAnalysis;

namespace HiLoGame.IoC
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjectionConfig
    {
        //The services and repository were created as transient because they are lightweight objects that can be easily created and discarded
        public static void ConfigureServices(this IServiceCollection services) => services.AddTransient<IGameService, GameService>();
        public static void ConfigureRepositories(this IServiceCollection services) => services.AddTransient<IGameRepository, GameRepository>();
        public static void ConfigureAutoMapper(this IServiceCollection services)
        {
            var mapperConfig = new MapperConfiguration(mc => mc.AddProfile(new GameMappingProfile()));

            IMapper mapper = mapperConfig.CreateMapper();

            services.AddSingleton(mapper);
        }
        public static IServiceCollection ConfigureMongoDb(this IServiceCollection services, string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            
            //The database was created as a singleton because it is an expensive resource to create
            services.AddSingleton(database);

            return services;
        }
    }
}
