namespace TaskTrackerApp.Core;

public sealed class UserTaskDTO
{
    public int Id { get; init; }
    public string Description { get; set; } = "";
    public UserTaskStatus UserTaskStatus { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }


    //New ToString for better visibility
    public override string ToString()
    {
        return $"\n\tID: {Id}; \n\tDescription: {Description}; \n\tStatus: {UserTaskStatus};\n\tlast update time: {UpdatedAt}\n";
    }

}

