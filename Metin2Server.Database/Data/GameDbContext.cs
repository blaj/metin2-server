using Metin2Server.Database.Domain.Entities;
using Metin2Server.Database.Domain.Repositories;
using Metin2Server.Database.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace Metin2Server.Database.Data;

public class GameDbContext : DbContext, IUnitOfWork
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Character> Characters { get; set; }
    public DbSet<CharacterSkill> CharacterSkills { get; set; }
    public DbSet<CharacterQuickSlot> CharacterQuickSlots { get; set; }
    public DbSet<CharacterItem> CharacterItems { get; set; }
    public DbSet<BannedWord> BannedWords { get; set; }
    public DbSet<ItemDefinition> ItemDefinitions { get; set; }

    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public GameDbContext(
        DbContextOptions<GameDbContext> options,
        IConfiguration configuration,
        IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
        {
            return;
        }

        var connectionString = IsMigration()
            ? _configuration.GetConnectionString("MigrationConnection")
            : _configuration.GetConnectionString("DefaultConnection");

        optionsBuilder
            .UseNpgsql(
                connectionString,
                options => options.MigrationsHistoryTable("ef_migrations_history", "public"))
            .UseSnakeCaseNamingConvention()
            .AddInterceptors([
                _serviceProvider.GetRequiredService<AuditingEntityInterceptor>(),
                _serviceProvider.GetRequiredService<ArchiveEntityInterceptor>()
            ]);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    private bool IsMigration()
    {
        return Environment.GetCommandLineArgs().Any(arg => arg.Contains("database") || arg.Contains("migrations"));
    }
}