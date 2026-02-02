using BudgetTracker.Events;

namespace BudgetTracker.Services;

public class LoggerService
{
    private readonly string _logFile;

    public LoggerService(string logDir, TransactionService transactionService)
    {
        if (!Directory.Exists(logDir))
            Directory.CreateDirectory(logDir);

        _logFile = Path.Combine(logDir, "transactions.log");

        // Subscribe to the event
        transactionService.TransactionAdded += OnTransactionAdded;
    }

    private void OnTransactionAdded(object? sender, TransactionAddedEventArgs e)
    {
        var t = e.Transaction;

        var line =
            $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {t.Type,-7} | {t.Amount,8:C} | {t.Description} | {t.Id}";

        File.AppendAllText(_logFile, line + Environment.NewLine);
    }
}
