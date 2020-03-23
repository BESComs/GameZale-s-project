public interface IStoppableTask
{
    bool StopRequested { get; set; }
    void StopTask();
}