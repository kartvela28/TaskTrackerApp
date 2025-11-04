using TaskTrackerApp.Core;
using TaskTrackerApp.Data;

namespace TaskTrackerApp.App;

public class TaskApp
{
    private bool _exit = false;
    private readonly TaskRepository _repo;

    public TaskApp(TaskRepository repo)
    {
        _repo = repo;
    }

    public void Run()
    {
        _repo.Reader.ReadAllTaskFromFile();
        _repo.SetCounter();

        AppMessages.PrintBanner();
        AppMessages.PrintUsage();
        AppMessages.PrintHint();

        do
        {
            Console.Write("\n> ");
            string userInput = Console.ReadLine() ?? "";
            ProcessCommand(userInput);
        } while (!_exit);
    }

    public void ProcessCommand(string command)
    {

        try
        {
            string[] action = command.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (action.Length == 0) return;

            switch (action[0])
            {
                case "q":
                    _exit = true;
                    _repo.Writer.WriteUserTask();
                    break;

                case "add":
                    string description = ParseDescription(command);
                    if (string.IsNullOrWhiteSpace(description))
                    {
                        Console.WriteLine("Invalid description.");
                        break;
                    }
                    _repo.AddTask(description);
                    Console.WriteLine("Task successfully Added");
                    break;

                case "update":
                    if (action.Length < 2 || !int.TryParse(action[1], out int taskId))
                    {
                        Console.WriteLine("Invalid task ID.");
                        break;
                    }
                    string newDescription = ParseDescription(command);
                    bool updated = _repo.UpdateTaskDescription(taskId, newDescription);
                    Console.WriteLine($"update operation {(updated ? "Succeeded" : "Failed")}");
                    break;

                case "mark":
                    if (action.Length < 3 || !int.TryParse(action[1], out int statusTaskId))
                    {
                        Console.WriteLine("Invalid command. Usage: mark <id> <status>");
                        break;
                    }
                    UserTaskStatus? newStatus = ParseStatus(command);
                    if (newStatus == null) break;
                    bool statusupdate = _repo.ChangeTaskStatus(statusTaskId, (UserTaskStatus)newStatus);
                    Console.WriteLine($"update operation of task status {(statusupdate ? "Succeeded" : "Failed")}");
                    break;

                case "delete":
                    if (action.Length < 2) { Console.WriteLine($"Specify task ID to remove"); break; }
                    bool deletestatus = _repo.DeleteUserTask(action[1]);
                    Console.WriteLine($"update operation {(deletestatus ? "Succeeded" : "Failed")}");
                    break;

                case "list":
                    UserTaskStatus? filter = ParseStatus(command);
                    foreach (var task in _repo.ListUserTasks(filter))
                        Console.WriteLine(task);

                    break;

                case "help":
                    AppMessages.PrintUsage();
                    break;

                default:
                    Console.WriteLine($"Unsuported command.");
                    AppMessages.PrintUsage();
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing command: {ex.Message}");
        }
    }

    public string ParseDescription(string command)
    {
        char[] descriptionDelimiter = ['"', '\''];


        int firstQuote = command.IndexOfAny(descriptionDelimiter);
        int lastQuote = command.LastIndexOfAny(descriptionDelimiter);

        if (firstQuote == -1 || lastQuote == -1 || firstQuote == lastQuote)
            return string.Empty;

        return command.Substring(firstQuote + 1, lastQuote - firstQuote - 1).Trim();
    }

    public UserTaskStatus? ParseStatus(string command)
    {
        string[] parts = command.Split(" ");
        if (parts.Length < 2) return null;

        if (Enum.TryParse<UserTaskStatus>(parts[parts.Length-1], true, out var filter))
            return filter;

        Console.WriteLine($"Invalid status \"{parts[parts.Length-1]}\". Valid: Todo, InProgress, Pending, Done");
        return null;
    }

}
