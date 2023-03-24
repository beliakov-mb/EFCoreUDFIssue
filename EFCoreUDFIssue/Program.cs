using Microsoft.EntityFrameworkCore;

using var context = new DemoContext();

var q1 = context.Customers
    .Where(
        c =>
            MyDefinedFunctions.MyDefinedFunction(
                context.CustomerBalances
                    .Where(x => x.CustomerId == 1)
                    .Count()
            ).Any());

Console.WriteLine("q1 as SQL query:");
Console.WriteLine(q1.ToQueryString());
Console.WriteLine();

var q2 = context.CustomerBalances.Where(cb => cb.Customer.Id == 1);

Console.WriteLine("q2 as SQL query:");
Console.WriteLine(q2.ToQueryString());
Console.WriteLine();

var q3 = context.Customers
    .Where(
        c =>
            MyDefinedFunctions.MyDefinedFunction(
                context.CustomerBalances
                    .Where(x => x.Customer.Id == 1)
                    .Count()
            ).Any());

Console.WriteLine("q3 as SQL query:");
Console.WriteLine(q3.ToQueryString());
Console.WriteLine();

public class DemoContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<CustomerBalance> CustomerBalances { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ValueAsQueryableIntResultDto>().HasNoKey();

        modelBuilder.HasDbFunction(
            typeof(MyDefinedFunctions).GetMethod(nameof(MyDefinedFunctions.MyDefinedFunction), new[] { typeof(int) })!);
    }
}

public class Customer
{
    public int Id { get; set; }
    public int Name { get; set; }
}

public class CustomerBalance
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; }
}

public static class MyDefinedFunctions
{
    public static IQueryable<ValueAsQueryableIntResultDto> MyDefinedFunction(int value)
        => throw new NotSupportedException();
}

public class ValueAsQueryableIntResultDto
{
    public int? Value { get; set; }
}