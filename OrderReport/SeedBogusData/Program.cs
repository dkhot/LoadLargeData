using Microsoft.EntityFrameworkCore;
using OrderReportApi;

Console.WriteLine("Seed data.");
if (args.Contains("--seed"))
{
    await using var context = new AppDbContext();
   
    await context.Database.MigrateAsync();
    var seeder = new DataSeeder(context);
    await seeder.SeedAsync();
    return;
}
