using BudgetTracker.Models;
using BudgetTracker.Services;

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
            Console.WriteLine("Report coming soon");
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
