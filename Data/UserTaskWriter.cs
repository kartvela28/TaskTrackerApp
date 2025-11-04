using System.Text.Json;

namespace TaskTrackerApp.Data;

public class UserTaskWriter
{
    private readonly TaskRepository _repo;
    public UserTaskWriter(TaskRepository repo)
    {
        _repo = repo;
    }

    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        MaxDepth = 64,
        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
    };
    public void WriteUserTask()
    {
        string taskJson = JsonSerializer.Serialize(_repo.UserTasks, _options);
        File.WriteAllText(_repo.TasksFile, taskJson);
    }
}

