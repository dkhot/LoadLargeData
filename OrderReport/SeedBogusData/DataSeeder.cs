using Bogus;
using Microsoft.EntityFrameworkCore;
using OrderReportApi;

// Data/DataSeeder.cs
public class DataSeeder
{
    private readonly AppDbContext _context;
    private const int CustomerCount = 500;
    private const int OrderCount = 40_000;
    private const int BatchSize = 2000;

    public DataSeeder(AppDbContext context) => _context = context;

    public async Task SeedAsync()
    {
        // Idempotency check — don't reseed if data already exists
        if (await _context.Customers.AnyAsync())
        {
            Console.WriteLine("Data already seeded. Skipping.");
            return;
        }

        Console.WriteLine("Seeding customers...");
        var customerIds = await SeedCustomersAsync();

        Console.WriteLine("Seeding orders + order items...");
        await SeedOrdersAsync(customerIds);

        Console.WriteLine("Seeding complete.");
    }

    private async Task<List<int>> SeedCustomersAsync()
    {
        var customerFaker = new Faker<Customer>()
            .RuleFor(c => c.Name, f => f.Company.CompanyName());

        var customers = customerFaker.Generate(CustomerCount);

        // Small enough to insert in one go
        await _context.Customers.AddRangeAsync(customers);
        await _context.SaveChangesAsync();

        return customers.Select(c => c.CustomerId).ToList();
    }

    private static async Task SeedOrdersAsync(List<int> customerIds)
    {
        var random = new Random();
        var orderFaker = new Faker<Order>()
            .RuleFor(o => o.CustomerId, f => f.PickRandom(customerIds))
            .RuleFor(o => o.OrderDate, f => f.Date.Between(DateTime.Now.AddYears(-2), DateTime.Now))
            .RuleFor(o => o.Total, f => 0m); // computed after items are added

        var productNames = new[] { "Widget", "Gadget", "Gizmo", "Doohickey", "Thingamajig", "Contraption" };

        int totalInserted = 0;

        while (totalInserted < OrderCount)
        {
            int currentBatchSize = Math.Min(BatchSize, OrderCount - totalInserted);

            // IMPORTANT: fresh context per batch to avoid change-tracker buildup
            await using var batchContext = new AppDbContext();

            var orders = orderFaker.Generate(currentBatchSize);

            foreach (var order in orders)
            {
                int itemCount = random.Next(1, 5);
                var items = new List<OrderItem>();

                for (int i = 0; i < itemCount; i++)
                {
                    var price = Math.Round((decimal)(random.NextDouble() * 100 + 5), 2);
                    var qty = random.Next(1, 10);

                    items.Add(new OrderItem
                    {
                        ProductName = productNames[random.Next(productNames.Length)],
                        Price = price,
                        Quantity = qty
                    });
                }

                order.OrderItems = items;
                order.Total = items.Sum(i => i.Price * i.Quantity);
            }

            await batchContext.Orders.AddRangeAsync(orders);
            await batchContext.SaveChangesAsync();

            totalInserted += currentBatchSize;
            Console.WriteLine($"Inserted {totalInserted}/{OrderCount} orders...");
        }
    }
}

