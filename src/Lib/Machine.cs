namespace Lib;

public sealed record Machine(
    int Id,
    Task[] Tasks)
{
    public int MakeSpan => Tasks.Sum(t => t.Duration);
    public Task? LowestTask => Tasks.MinBy(t => t.Duration);    
    public Task? HighestTask => Tasks.MaxBy(t => t.Duration);
}
