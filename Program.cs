using TaskTrackerApp.App;
using TaskTrackerApp.Data;

public class Program
{
    static void Main(string[] args)
    {

        TaskRepository taskRepo = new TaskRepository();
        TaskApp app = new TaskApp(taskRepo);

        app.Run();
    }
}
