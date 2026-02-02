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
            Console.WriteLine("Add transaction (coming soon)");
            break;

        case "2":
            Console.WriteLine("Remove transaction (coming soon)");
            break;

        case "3":
            Console.WriteLine("Report (coming soon)");
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
