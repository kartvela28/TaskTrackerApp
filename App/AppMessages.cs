namespace TaskTrackerApp.App;

public static class AppMessages
{
    public static void PrintBanner()
    {
        Console.Clear();

        string banner = @"
===========================================
         TASK TRACKER APPLICATION
===========================================
 Manage your daily tasks quickly and safely
-------------------------------------------
";

        Console.WriteLine(banner);
        Console.WriteLine("Type a command or 'q' to quit.\n");
    }

    public static void PrintUsage()
    {
        string usage = @"
Available Commands:

    add ""description""          → Create a new task
    update <id> ""description""  → Update a task's description
    mark <id> <status>           → Change task status (todo, inprogress, pending, done)
    delete <id>                  → Delete a task by ID
    list [status]                → List all tasks or filter by status
    q                            → Quit the application
";
        Console.WriteLine(usage);
    }

    public static void PrintHint()
    {
        Console.WriteLine("Type 'help' to see available commands.");
    }
}

