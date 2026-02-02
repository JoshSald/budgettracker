# Budget Tracker (Console App)

A simple console-based budget tracker built with C# and .NET.

Track income and expenses from the terminal, store them as daily JSON files, and generate reports using LINQ.

Small. Fast. No database.

---

## Features

- Add income or expense transactions
- Remove transactions by Id
- Daily JSON storage (one file per day)
- Date range reports
- Quick shortcuts:
  - t → today
  - m → this month
  - y → this year
  - q → quit (on home screen)
- Color-coded console output
- Confirmation screen before saving
- Event-driven logging to `logs/transactions.log`

---

## Tech Used

- .NET Console App
- Top-level statements
- LINQ
- JSON serialization
- File I/O
- Event pattern

---

## Project Structure

```
BudgetTracker/
│
├── Models/
│   ├── Transaction.cs
│   └── TransactionType.cs
│
├── Services/
│   ├── StorageService.cs
│   ├── TransactionService.cs
│   └── LoggerService.cs
│
├── Events/
│   └── TransactionAddedEventArgs.cs
│
├── data/      # daily JSON files
├── logs/      # transaction logs
└── Program.cs
```

---

## Run the app

### Prerequisites

- .NET SDK 10.0+

### Build

```
dotnet build
```

### Run

```
dotnet run
```

---

## Menu

```
1) Add transaction
2) Remove transaction
3) Report
0) Exit
```

---

## Add Transaction

Prompts for:
- type (income / expense)
- description
- amount

Shows a confirmation table before saving.

---

## Remove Transaction

Requires:
- date (yyyy-MM-dd)
- transaction Id

Rewrites the day's JSON file after removal.

---

## Reports

Generate totals using LINQ.

You can enter:

| Input | Meaning |
|------|---------|
| t | today |
| m | this month |
| y | this year |
| yyyy-MM-dd | custom range start |

Example:

```
3
m
```

Shows current month's totals.

---

## Data Storage

Each day is stored separately:

```
data/2026-02-02.json
```

Example:

```json
[
  {
    "Id": "...",
    "Timestamp": "...",
    "Type": "Income",
    "Description": "Salary",
    "Amount": 2500
  }
]
```

---

## Logging

Every transaction is logged automatically:

```
logs/transactions.log
```

Example:

```
2026-02-02 14:21:09 | Income | £2500.00 | Salary | 1b7a...
```

---

## Why this project?

Practice with:

- file persistence
- JSON serialization
- LINQ queries
- events
- clean service architecture
- defensive input handling

---

## Future Improvements (ideas)

- list transactions per day
- CSV export
- edit transactions
- categories
- monthly summaries
- charts

---

## License

MIT