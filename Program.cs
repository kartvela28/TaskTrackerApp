using System.Diagnostics.Metrics;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Linq;

public class Program
{
    static void Main(string[] args)
    {
        string usage = $"Usage: \n\tAdd | Update | Delete | List [option] | Analytics [option]\n\n\tOptions: \n\t\t done | in-Progress | todo";
        bool exit = false;

        Console.Clear();
        Console.WriteLine($"Task Tracker App started");
        Console.WriteLine($"\nFeel safe and sound with your tasks");
        Console.WriteLine(usage);
        Console.WriteLine($"\nExit charaqter 'q'");


        // TODO refactor
        UserTaskReader.ReadAllTaskFromFile();
        
        int counter = (from x in FileServicesHelper.UserTasks
                       select x.Id).DefaultIfEmpty(0).Max();
        
        char[] descriptionDelimiter = ['"', '\''];


        do
        {
            string userInput = Console.ReadLine() ?? "";
            string[] action = userInput.ToLower().Split(" ");
            switch (action[0])
            {
                case "q":
                    exit = true;
                    UserTaskWriter.WriteUserTask();
                    break;
                case "add":

                    // Parsing of task description 
                    int descriptionStart = userInput.IndexOfAny(descriptionDelimiter) + 1;
                    int descriptionEnd = userInput.IndexOfAny(descriptionDelimiter, descriptionStart + 1);
                    string description = userInput.Substring(descriptionStart, descriptionEnd - descriptionStart);

                    //temp task creation
                    new UserTaskDTO()
                    {
                        Id = ++counter,
                        Description = description,
                        UserTaskStatus = UserTaskStatus.Todo,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    // Console.WriteLine($"Task is created by ID = {task.Id} , Status = {task.Status} ");
                    break;


                case "update":
                    int.TryParse(action[1], out int taskid);
                    int newDescriptionStart = userInput.IndexOfAny(descriptionDelimiter) + 1;
                    int newDescriptionEnd = userInput.IndexOfAny(descriptionDelimiter, newDescriptionStart + 1);
                    string newDescription = userInput.Substring(newDescriptionStart, newDescriptionEnd - newDescriptionStart);
                    bool updated = FileServicesHelper.UpdateTaskDescription(taskid, newDescription);
                    Console.WriteLine($"update operation {(updated ? "Succeed" : "Failed")}");
                    // Console.WriteLine($"Coming soon");
                    break;

                case "mark":
                    // Console.WriteLine($"Enter task ID and status.(7 3) \n\t1 - in progress \n\t2 - pending \n\t3 - done");
                    int.TryParse(action[1], out int statustaskid);
                    bool statusupdate = FileServicesHelper.ChangeTaskStatus(statustaskid, action[2]);
                    Console.WriteLine($"update operation of task status {(statusupdate ? "Succeed" : "Failed")}");


                    break;
                case "delete":
                    if (action.Length == 1) { Console.WriteLine($"Specify task id want to remove"); break; }

                    bool deletestatus = FileServicesHelper.DeleteUserTask(action[1]);
                    Console.WriteLine($"update operation {(deletestatus ? "Succeed" : "Failed")}");
                    // Console.WriteLine("delete operation");
                    // Console.WriteLine($"Coming soon");
                    break;
                case "list":
                    if (action.Length == 1) FileServicesHelper.ListUserTasks();
                    else FileServicesHelper.ListUserTasks(action[1]);
                    break;
                case "analytics":
                    Console.WriteLine($"Analytics of {action[1]}");
                    Console.WriteLine($"Coming soon");
                    break;
                default:
                    Console.WriteLine($"Unsuported command, \n{usage}");
                    break;
            }
        } while (!exit);
    }
}

public enum UserTaskStatus
{
    Todo,
    InProgress,
    Pending,
    Done
}

public sealed class UserTaskDTO
{
    public int Id { get; init; }
    public string Description { get; set; } = "";
    public UserTaskStatus UserTaskStatus { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }

    public UserTaskDTO()
    {
        FileServicesHelper.UserTasks.Add(this);
        // UserTaskWriter.WriteUserTask();
    }

    //New ToString for better visibility
    public override string ToString()
    {
        return $"\n\tID: {Id}; \n\tDescription: {Description}; \n\tStatus: {UserTaskStatus};\n\tlast update time: {UpdatedAt}\n";
    }

}

public abstract class FileServicesHelper
{
    public static string AppDirectory = Directory.GetCurrentDirectory();
    public static string TasksDirectory = Path.Combine(AppDirectory, "Tasks");
    public static string TasksFile = Path.Combine(TasksDirectory, "Tasks.json");
    protected int IdCounter = 0;
    public static List<UserTaskDTO> UserTasks = new List<UserTaskDTO>();
    static FileServicesHelper()
    {
        if (!Directory.Exists(TasksDirectory))
        {
            Directory.CreateDirectory(TasksDirectory);
        }
    }

    public static bool UpdateTaskDescription(int? id, string? description)
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

    public static void ListUserTasks()
    {
        foreach (UserTaskDTO task in UserTasks)
        {
            Console.WriteLine(task.ToString());
        }
    }

    public static void ListUserTasks(string status)
    {
        UserTaskStatus filter = Enum.Parse<UserTaskStatus>(status, true);
        List<UserTaskDTO> tempTasks = UserTasks.FindAll(fi => fi.UserTaskStatus == filter);
        foreach (UserTaskDTO task in tempTasks)
        {
            Console.WriteLine(task.ToString());
        }
        tempTasks.Clear();
    }

    public static bool ChangeTaskStatus(int taskid, string status)
    {
        UserTaskDTO? updateStatus = UserTasks.Find(task => task.Id == taskid);
        if (updateStatus == null) return false;

        updateStatus.UserTaskStatus = Enum.Parse<UserTaskStatus>(status, true);
        updateStatus.UpdatedAt = DateTime.UtcNow;
        return true;
    }

    public static bool DeleteUserTask(string arg)
    {
        int.TryParse(arg, out int deleteId);
        UserTaskDTO? tasktoremove = UserTasks.Find(task => task.Id == deleteId);
        if (tasktoremove == null) return false;
        UserTasks.Remove(tasktoremove);
        return true;
    }
}

public class UserTaskReader : FileServicesHelper
{
    private static readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        MaxDepth = 64,
        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
    };
    public static void ReadAllTaskFromFile()
    {
        // If UserTasks file does not exist, stop reading
        if (!File.Exists(TasksFile))
        {
            return;
        }

        try
        {
            string json = File.ReadAllText(TasksFile);
            JsonSerializer.Deserialize<List<UserTaskDTO>>(json, _options);
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

public class UserTaskWriter : FileServicesHelper
{
    private static readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        MaxDepth = 64,
        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
    };
    public static void WriteUserTask()
    {
        string taskJson = JsonSerializer.Serialize(UserTasks, _options);
        File.WriteAllText(TasksFile, taskJson);
    }
}
