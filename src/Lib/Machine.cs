namespace Lib;

public sealed class Machine(int id, Task[] tasks)
{
    public int Id { get; } = id;
    public Task[] Tasks { get; set; } = tasks;
    public int MakeSpan => Tasks.Sum(t => t.Duration);
    public Task? LowestTask => Tasks.MinBy(t => t.Duration);    
    public Task? HighestTask => Tasks.MaxBy(t => t.Duration);
}
