using System.Text.Json;
using BudgetTracker.Models;

namespace BudgetTracker.Services;

public class StorageService
{
    private readonly string _dataDir;
    private readonly JsonSerializerOptions _jsonOptions =
        new() { WriteIndented = true };

    public StorageService(string dataDir)
    {
        _dataDir = dataDir;

        if (!Directory.Exists(_dataDir))
            Directory.CreateDirectory(_dataDir);
    }

    private string GetFilePath(DateOnly date)
        => Path.Combine(_dataDir, $"{date:yyyy-MM-dd}.json");

    public List<Transaction> Load(DateOnly date)
    {
        var path = GetFilePath(date);

        if (!File.Exists(path))
            return new List<Transaction>();

        var json = File.ReadAllText(path);

        return JsonSerializer.Deserialize<List<Transaction>>(json)
               ?? new List<Transaction>();
    }

    public void Save(DateOnly date, List<Transaction> transactions)
    {
        var path = GetFilePath(date);

        var json = JsonSerializer.Serialize(transactions, _jsonOptions);

        File.WriteAllText(path, json);
    }

    public void Append(Transaction transaction)
    {
        var transactions = Load(transaction.Date);

        transactions.Add(transaction);

        Save(transaction.Date, transactions);
    }
}
