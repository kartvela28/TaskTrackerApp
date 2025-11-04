using System.Text.Json;
using TaskTrackerApp.Core;

namespace TaskTrackerApp.Data;

public class UserTaskReader
{
    private readonly TaskRepository _repo;

    public UserTaskReader(TaskRepository repo)
    {
        _repo = repo;
    }
    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        MaxDepth = 64,
        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
    };
    public void ReadAllTaskFromFile()
    {
        if (!File.Exists(_repo.TasksFile))
        {
            return;
        }

        try
        {
            string json = File.ReadAllText(_repo.TasksFile);
            List<UserTaskDTO>? task = JsonSerializer.Deserialize<List<UserTaskDTO>>(json, _options);
            if (task != null)
            {
                _repo.UserTasks = task;
            }
        }
        catch (NullReferenceException ex)
        {
            Console.WriteLine($"No User Tasks detected\n{ex.Message}");
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"No Json inside file\n{ex.Message}");
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine($"TK - Argument null exception {ex.Message}");
        }
    }
}

