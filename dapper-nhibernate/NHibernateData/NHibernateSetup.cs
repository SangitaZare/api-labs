using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Mapping.ByCode;

namespace DapperNHibernateApi.NHibernateData;

public static class NHibernateSetup
{
    /// <summary>
    /// Registers NHibernate in dependency injection:
    ///  - ISessionFactory: built ONCE (it is expensive) -> Singleton
    ///  - ISession: one per HTTP request (like a DbContext) -> Scoped
    /// </summary>
    public static IServiceCollection AddNHibernate(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<ISessionFactory>(_ => BuildSessionFactory(connectionString));
        services.AddScoped(sp => sp.GetRequiredService<ISessionFactory>().OpenSession());
        return services;
    }

    private static ISessionFactory BuildSessionFactory(string connectionString)
    {
        // 1. Collect the mappings
        var mapper = new ModelMapper();
        mapper.AddMapping<NhDepartmentMap>();
        mapper.AddMapping<NhEmployeeMap>();

        // 2. Database settings
        // (Full name used because ASP.NET Core also has types in a "Configuration" namespace)
        var configuration = new NHibernate.Cfg.Configuration();
        configuration.DataBaseIntegration(db =>
        {
            db.ConnectionString = connectionString;
            db.Dialect<MsSql2012Dialect>();                 // which SQL flavour to generate
            db.Driver<MicrosoftDataSqlClientDriver>();      // same SqlClient that EF Core uses
            db.LogSqlInConsole = true;                      // print generated SQL in the console
            db.LogFormattedSql = true;
        });

        // 3. Add the compiled mappings and build the factory
        configuration.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());

        return configuration.BuildSessionFactory();
    }
}
