using System.Diagnostics;
using System.Threading.Tasks;

public class TaskWrapper
{
    public async Task<T> ExecuteWithPerformance<T>(Func<Task<T>> taskFunc)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        T result = await taskFunc();

        stopwatch.Stop();
        long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

        // Log or process the elapsed time as needed
        Console.WriteLine($"Task executed in {elapsedMilliseconds} milliseconds");

        return result;
    }
}

using System.Threading;

public class ArcGISPro : ScriptBase
{
    private ManualResetEvent eventWaitHandle = new ManualResetEvent(false);

    void Execute() 
    {
        // Start the application
        START(mainWindowTitle: "ArcGIS Sign In", processName: "ArcGISPro");

        // Wait for the application to be ready
        WAIT(2);

        // Click the control
        STOP();

        // Wait for the event to be signaled
        eventWaitHandle.WaitOne();

        // Event has been signaled, continue with the rest of the code
        // ...
    }

    // Method to signal the event
    public void SignalEvent()
    {
        eventWaitHandle.Set();
    }
}