namespace Lib;

public sealed record Machine(
    int Id,
    Task[] Tasks)
{
    public int MakeSpan => Tasks.Sum(t => t.Duration);
}
