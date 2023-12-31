using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using BlazorApp.Models;

namespace BlazorApp;

internal sealed class PostgreSqlSeeder
{
    public static async Task CreateSampleDataAsync(IServiceProvider serviceProvider)
    {
        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();

        try
        {
            await using var dbContext = scope.ServiceProvider.GetRequiredService<BlazorAppContext>();

            await DropCreateTablesAsync(dbContext);
            await InsertSampleDataAsync(dbContext);
        }
        catch (DbException exception)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<PostgreSqlSeeder>>();
            logger.LogError(exception, "An error occurred seeding the DB.");
        }
    }

    private static async Task DropCreateTablesAsync(DbContext dbContext)
    {
        bool wasCreated = await dbContext.Database.EnsureCreatedAsync();

        if (!wasCreated)
        {
            // The database already existed. Because apps usually don't have permission to drop the database,
            // we drop and recreate all the tables in the DbContext instead.
            var databaseCreator = (RelationalDatabaseCreator)dbContext.Database.GetService<IDatabaseCreator>();

            await DropTablesAsync(dbContext);
            await databaseCreator.CreateTablesAsync();
        }
    }

    private static async Task DropTablesAsync(DbContext dbContext)
    {
        IEnumerable<string> tableNames = dbContext.Model.GetEntityTypes().Select(type => type.GetSchemaQualifiedTableName()!);
        IEnumerable<string> dropStatements = tableNames.Select(tableName => "DROP TABLE IF EXISTS \"" + tableName + "\";");

        string sqlStatement = string.Join(Environment.NewLine, dropStatements);
        await dbContext.Database.ExecuteSqlRawAsync(sqlStatement);
    }

    private static async Task InsertSampleDataAsync(BlazorAppContext dbContext)
    {
        dbContext.Contacts.AddRange(
            new Contact { FirstName = "John", LastName = "Doe", Email = "john@doe.com" },
            new Contact { FirstName = "Bill", LastName = "Doe", Email = "bill@doe.com" },
            new Contact { FirstName = "Mary", LastName = "Edwards", Email = "medwards@gmail.com" },
            new Contact { FirstName = "Jack", LastName = "Rogers", Email = "jrogers@gmail.com" },
            new Contact { FirstName = "Jane", LastName = "Doe", Email = "jane@doe.com" });

        await dbContext.SaveChangesAsync();
    }
}