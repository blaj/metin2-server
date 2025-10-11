using Metin2Server.Database.Data;
using Metin2Server.Database.Domain.Repositories;
using Metin2Server.Database.Grpc;
using Metin2Server.Database.Interceptors;
using Metin2Server.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StackExchange.Redis;

namespace Metin2Server.Database;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog((context, config) =>
            config.ReadFrom.Configuration(context.Configuration));

        builder.Services.AddSingleton(TimeProvider.System);

        builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<GameDbContext>());
        builder.Services.AddScoped<AuditingEntityInterceptor>();
        builder.Services.AddScoped<ArchiveEntityInterceptor>();
        builder.Services.AddScoped<IAccountRepository, AccountRepository>();
        builder.Services.AddScoped<ILoginKeyRepository, LoginKeyRepository>();
        builder.Services.AddScoped<ICharacterRepository, CharacterRepository>();
        builder.Services.AddScoped<ICharacterSkillRepository, CharacterSkillRepository>();
        builder.Services.AddScoped<ICharacterQuickSlotRepository, CharacterQuickSlotRepository>();
        builder.Services.AddScoped<IBannedWordRepository, BannedWordRepository>();
        builder.Services.AddScoped<ICharacterCreationTimeRepository, CharacterCreationTimeRepository>();
        builder.Services.AddScoped<IChannelInformationRepository, ChannelInformationRepository>();
        builder.Services.AddScoped<ICharacterItemRepository, CharacterItemRepository>();
        builder.Services.AddScoped<IConnectionMultiplexer>(_ =>
        {
            var configuration =
                ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Redis")!, true);
            return ConnectionMultiplexer.Connect(configuration);
        });
        builder.Services.AddGrpc();
        builder.Services.AddDbContext<GameDbContext>();

        builder.WebHost.ConfigureKestrel(k =>
        {
            k.ListenAnyIP(5000,
                o => { o.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2; });
        });

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<GameDbContext>();
            context.Database.SetConnectionString(builder.Configuration.GetConnectionString("MigrationConnection"));

            if ((await context.Database.GetPendingMigrationsAsync()).Any())
            {
                await context.Database.MigrateAsync();
            }

            await context.DisposeAsync();
        }

        app.MapGrpcService<AccountServiceImpl>();
        app.MapGrpcService<LoginKeyServiceImpl>();
        app.MapGrpcService<BannedWordServiceImpl>();
        app.MapGrpcService<CharacterServiceImpl>();
        app.MapGrpcService<ChannelInformationServiceImpl>();
        app.MapGrpcService<CharacterItemServiceImpl>();
        app.MapGrpcService<ItemDefinitionServiceImpl>();

        app.MapGet("/", () => "Metin2 DB Service running...");

        await app.RunAsync();
    }
}