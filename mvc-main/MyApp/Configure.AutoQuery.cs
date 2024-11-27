using ServiceStack;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;

[assembly: HostingStartup(typeof(MyApp.ConfigureAutoQuery))]

namespace MyApp;

public class ConfigureAutoQuery : IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices((context,services) =>
        {
            services.AddSingleton<ICrudEvents>(c =>
    new OrmLiteCrudEvents(c.GetRequiredService<IDbConnectionFactory>()));

            services.AddSingleton<IDbConnectionFactory>(c =>
    new OrmLiteConnectionFactory(
        context.Configuration.GetValue<string>("DefaultConnection"),
        SqlServerDialect.Provider));
        })
        .ConfigureAppHost(appHost => {
            var crudEvents = appHost.Resolve<ICrudEvents>();
            crudEvents.InitSchema();
        });
}