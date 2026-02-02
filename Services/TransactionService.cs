using BudgetTracker.Events;
using BudgetTracker.Models;

namespace BudgetTracker.Services;

public class TransactionService
{
    private readonly StorageService _storage;

    public event EventHandler<TransactionAddedEventArgs>? TransactionAdded;

    public TransactionService(StorageService storage)
    {
        _storage = storage;
    }

    public Transaction Add(
        TransactionType type,
        string description,
        decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Fool of a Took!Your amount must be greater than 0.");

        var now = DateTime.Now;

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Timestamp = now,
            Type = type,
            Description = description,
            Amount = amount,
            Date = DateOnly.FromDateTime(now)
        };

        _storage.Append(transaction);

        TransactionAdded?.Invoke(
            this,
            new TransactionAddedEventArgs(transaction)
        );

        return transaction;
    }

    public bool Remove(Guid id, DateOnly date)
    {
        var transactions = _storage.Load(date);

        var removed = transactions.RemoveAll(t => t.Id == id) > 0;

        if (removed)
            _storage.Save(date, transactions);

        return removed;
    }

    public IEnumerable<Transaction> QueryRange(DateOnly start, DateOnly end)
    {
        var current = start;

        while (current <= end)
        {
            foreach (var t in _storage.Load(current))
                yield return t;

            current = current.AddDays(1);
        }
    }
}
