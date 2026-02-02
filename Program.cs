using BudgetTracker.Models;
using BudgetTracker.Services;

Directory.CreateDirectory("data");
Directory.CreateDirectory("logs");

var storage = new StorageService("data");
var transactionService = new TransactionService(storage);
var logger = new LoggerService("logs", transactionService);

while (true)
{
    Console.Clear();
    Console.WriteLine("==== Budget Tracker ====");
    Console.WriteLine("1) Add transaction");
    Console.WriteLine("2) Remove transaction");
    Console.WriteLine("3) Report");
    Console.WriteLine("0) Exit");
    Console.Write("Select: ");

    var input = Console.ReadLine();

    switch (input)
    {
        case "1":
            AddTransaction(transactionService);
            break;

        case "2":
            RemoveTransaction(transactionService);
            break;

        case "3":
            GenerateReport(transactionService);
            break;

        case "0":
            return;

        default:
            Console.WriteLine("Invalid choice");
            break;
    }

    Console.WriteLine("\nPress any key...");
    Console.ReadKey();
}

static void AddTransaction(TransactionService service)
{
    try
    {
        Console.Write("Type (income/expense): ");
        var typeInput = Console.ReadLine()?.Trim().ToLower();

        var type = typeInput switch
        {
            "income" or "i"=> TransactionType.Income,
            "expense" or "e" => TransactionType.Expense,
            _ => throw new Exception("Invalid type.")
        };

        Console.Write("Description: ");
        var description = Console.ReadLine() ?? "";

        Console.Write("Amount: ");
        if (!decimal.TryParse(Console.ReadLine(), out var amount))
            throw new Exception("Invalid amount.");

        var t = service.Add(type, description, amount);

        Console.WriteLine($"\nSaved with Id: {t.Id}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\nError: {ex.Message}");
    }
}
static void RemoveTransaction(TransactionService service)
{
    try
    {
        Console.Write("Date (yyyy-MM-dd): ");
        var dateInput = Console.ReadLine();

        if (!DateOnly.TryParse(dateInput, out var date))
            throw new Exception("Invalid date format.");

        Console.Write("Transaction Id: ");
        var idInput = Console.ReadLine();

        if (!Guid.TryParse(idInput, out var id))
            throw new Exception("Invalid Id format.");

        var removed = service.Remove(id, date);

        if (removed)
            Console.WriteLine("\nTransaction removed.");
        else
            Console.WriteLine("\nTransaction not found.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\nError: {ex.Message}");
    }
}
static void GenerateReport(TransactionService service)
{
    try
    {
        Console.Write("Start date (yyyy-MM-dd): ");
        if (!DateOnly.TryParse(Console.ReadLine(), out var start))
            throw new Exception("Invalid start date.");

        Console.Write("End date (yyyy-MM-dd): ");
        if (!DateOnly.TryParse(Console.ReadLine(), out var end))
            throw new Exception("Invalid end date.");

        if (end < start)
            throw new Exception("End date cannot be before start date.");

        var transactions = service.QueryRange(start, end);

        var income = transactions
            .Where(t => t.Type == TransactionType.Income)
            .Sum(t => t.Amount);

        var expense = transactions
            .Where(t => t.Type == TransactionType.Expense)
            .Sum(t => t.Amount);

        var net = income - expense;

        Console.WriteLine("\n==== Report ====");
        Console.WriteLine($"Income:  {income:C}");
        Console.WriteLine($"Expense: {expense:C}");
        Console.WriteLine($"Net:     {net:C}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\nError: {ex.Message}");
    }
}