using System.Text.Json;
using System.IO;
using System;
using System.Collections.Generic;
using System.Text;


using TaskTrackerApp.Models;
using System.IO.Enumeration;
using Microsoft.Win32.SafeHandles;
using Microsoft.VisualBasic;
using System.Text.Unicode;

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


        do
        {
            string userInput = Console.ReadLine() ?? "";
            string[] action = userInput.ToLower().Split(" ");
            switch (action[0])
            {
                case "q":
                    exit = true;
                    break;



                case "add":

                    // Parsing of task description 
                    char[] descriptionDelimiter = ['"', '\''];
                    int descriptionStart = userInput.IndexOfAny(descriptionDelimiter) + 1;
                    int descriptionEnd = userInput.IndexOfAny(descriptionDelimiter, descriptionStart + 1);
                    string description = userInput.Substring(descriptionStart, descriptionEnd - descriptionStart);


                    Tasks task = new Tasks(description); // ??????

                    int counter = 1;

                    //temp task creation
                    new UserTaskDTO()
                    {
                        Id = counter++,
                        Description = description,
                        UserTaskStatus = UserTaskStatus.Todo,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    // Console.WriteLine($"Task is created by ID = {task.Id} , Status = {task.Status} ");
                    break;






                case "update":
                    Console.WriteLine("update operation");
                    Console.WriteLine($"Coming soon");
                    break;
                case "delete":
                    Console.WriteLine("delete operation");
                    Console.WriteLine($"Coming soon");
                    break;
                case "list":
                    Console.WriteLine($"List {action[1]}");
                    Console.WriteLine($"Coming soon");
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
        UserTaskWriter.UserTasks.Add(this);
        UserTaskWriter.WriteUserTask();
    }

    //New ToString for better visibility
    public override string ToString()
    {
        return $"\n\tID: {Id}; \n\tDescription: {Description}; \n\tStatus: {UserTaskStatus};";
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
}

public class UserTaskReader : FileServicesHelper
{

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

