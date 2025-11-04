using TaskTrackerApp.Core;

namespace TaskTrackerApp.Data;

public class TaskRepository
{
    public string AppDirectory;
    public string TasksDirectory;
    public string TasksFile;
    private int _idCounter;
    public List<UserTaskDTO> UserTasks = new List<UserTaskDTO>();
    public readonly UserTaskReader Reader;
    public readonly UserTaskWriter Writer;
    public TaskRepository()
    {
        AppDirectory = Directory.GetCurrentDirectory();
        TasksDirectory = Path.Combine(AppDirectory, "Tasks");
        TasksFile = Path.Combine(TasksDirectory, "Tasks.json");

        if (!Directory.Exists(TasksDirectory))
        {
            Directory.CreateDirectory(TasksDirectory);
        }

        _idCounter = UserTasks.Any() ? UserTasks.Max(t => t.Id) : 0;

        Reader = new UserTaskReader(this);
        Writer = new UserTaskWriter(this);
    }

    public UserTaskDTO AddTask(string description)
    {
        UserTaskDTO newTask = new UserTaskDTO
        {
            Id = ++_idCounter,
            Description = description,
            UserTaskStatus = UserTaskStatus.Todo,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        UserTasks.Add(newTask);
        return newTask;
    }

    public bool UpdateTaskDescription(int? id, string? description)
    {
        if (id == null || description == null)
        {
            return false;
        }
        UserTaskDTO? update = UserTasks.Find(task => task.Id == id);
        if (update == null)
        {
            return false;
        }
        update.Description = description;
        update.UpdatedAt = DateTime.UtcNow;
        Console.WriteLine(update.ToString());
        return true;
    }

    public IEnumerable<UserTaskDTO> ListUserTasks(UserTaskStatus? status = null)
    {
        if (status == null) return UserTasks;
        return UserTasks.Where(f => f.UserTaskStatus == status);
    }

    public bool ChangeTaskStatus(int taskid, UserTaskStatus status)
    {
        UserTaskDTO? updateStatus = UserTasks.Find(task => task.Id == taskid);
        if (updateStatus == null) return false;

        updateStatus.UserTaskStatus = status;
        updateStatus.UpdatedAt = DateTime.UtcNow;
        return true;
    }

    public bool DeleteUserTask(string arg)
    {
        int.TryParse(arg, out int deleteId);
        UserTaskDTO? tasktoremove = UserTasks.Find(task => task.Id == deleteId);
        if (tasktoremove == null) return false;
        UserTasks.Remove(tasktoremove);
        return true;
    }
}

