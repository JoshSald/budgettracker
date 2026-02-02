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
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("==== Budget Tracker ====");
    Console.ResetColor();
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

        case "0" or "q":
            return;

        default:
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid choice");
            Console.ResetColor();
            break;
    }

    Console.WriteLine("\nPress any key...");
    Console.ReadKey();
}

static void AddTransaction(TransactionService service)
{
    while (true) 
    {
        try
        {
            Console.Write("Type (income/expense or i/e): ");
            var typeInput = Console.ReadLine()?.Trim().ToLower();

            var type = typeInput switch
            {
                "income" or "i" => TransactionType.Income,
                "expense" or "e" => TransactionType.Expense,
                _ => throw new Exception("Invalid type.")
            };

            Console.Write("Description: ");
            var description = Console.ReadLine() ?? "";

            Console.Write("Amount: ");
            if (!decimal.TryParse(Console.ReadLine(), out var amount))
                throw new Exception("Invalid amount.");

            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("==== Confirm Transaction ====");
            Console.ResetColor();

            string amountStr = amount.ToString("C");

            Console.WriteLine("┌──────────────┬────────────────────────────┐");

            Console.Write("│ Type         │ ");
            Console.ForegroundColor =
                type == TransactionType.Income
                    ? ConsoleColor.Green
                    : ConsoleColor.Red;

            Console.Write($"{type,-26}");
            Console.ResetColor();
            Console.WriteLine(" │");

            Console.WriteLine($"│ Description  │ {description,-26} │");

            Console.Write("│ Amount       │ ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{amountStr,-26}");
            Console.ResetColor();
            Console.WriteLine(" │");

            Console.WriteLine("└──────────────┴────────────────────────────┘");

            Console.Write("\n(Y) Save    (B) Back: ");
            var confirm = Console.ReadLine()?.Trim().ToLower();

            if (confirm == "y")
            {
                var t = service.Add(type, description, amount);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nSaved with Id: {t.Id}");
                Console.ResetColor();

                return;
            }
            else if (confirm == "b")
            {
                Console.Clear();
                continue; 
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nError: {ex.Message}");
            Console.ResetColor();
            return;
        }
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
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nTransaction removed.");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nTransaction not found.");
            Console.ResetColor();
        }
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
        Console.Write("Start date (yyyy-MM-dd | t=today | m=month | y=year): ");
        var input = Console.ReadLine()?.Trim().ToLower();

        DateOnly start;
        DateOnly end;

        var today = DateOnly.FromDateTime(DateTime.Today);

        switch (input)
        {
            case "t":
                start = today;
                end = today;
                Console.WriteLine($"Using today: {start}");
                break;

            case "m":
                start = new DateOnly(today.Year, today.Month, 1);
                end = today;
                Console.WriteLine($"Using this month: {start} → {end}");
                break;

            case "y":
                start = new DateOnly(today.Year, 1, 1);
                end = today;
                Console.WriteLine($"Using this year: {start} → {end}");
                break;

            default:
                if (!DateOnly.TryParse(input, out start))
                    throw new Exception("Invalid start date.");

                Console.Write("End date (yyyy-MM-dd): ");
                if (!DateOnly.TryParse(Console.ReadLine(), out end))
                    throw new Exception("Invalid end date.");

                if (end < start)
                    throw new Exception("End date cannot be before start date.");
                break;
        }

        var transactions = service.QueryRange(start, end);

        var income = transactions
            .Where(t => t.Type == TransactionType.Income)
            .Sum(t => t.Amount);

        var expense = transactions
            .Where(t => t.Type == TransactionType.Expense)
            .Sum(t => t.Amount);

        var net = income - expense;

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\n==== Report ====");
        Console.ResetColor();

        string incomeStr = income.ToString("C");
        string expenseStr = expense.ToString("C");
        string netStr = net.ToString("C");

        Console.WriteLine("┌──────────────┬──────────────┬──────────────┐");
        Console.WriteLine("│ Income       │ Expense      │ Net          │");
        Console.WriteLine("├──────────────┼──────────────┼──────────────┤");

        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("│ ");

        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"{incomeStr,12}");

        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(" │ ");

        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write($"{expenseStr,12}");

        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(" │ ");

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"{netStr,12}");

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(" │");

        Console.WriteLine("└──────────────┴──────────────┴──────────────┘");

        Console.ResetColor();
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\nError: {ex.Message}");
        Console.ResetColor();
    }
}