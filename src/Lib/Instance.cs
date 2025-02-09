namespace Lib;

public sealed record Instance(
    int NumberOfMachines, //M
    double R
)
{
    public int MakeSpan => Machines.Max(m => m.MakeSpan);
    public Machine MachineWithLowestMakeSpan => Machines.OrderBy(m => m.MakeSpan).First();
    public Machine MachineWithHighestMakeSpan => Machines.OrderByDescending(m => m.MakeSpan).First();

    public Machine[] Machines { get; set; } = CreateInstanceMachines(
        NumberOfMachines, R);

    private static Machine[] CreateInstanceMachines(
        int NumberOfMachines, double r)
    {
        var machines = new Machine[NumberOfMachines];
        var tasks = CreateTasks(NumberOfMachines, r);

        machines[0] = new Machine(0, tasks);

        for (var i = 1; i < NumberOfMachines; i++)
            machines[i] = new Machine(i, []);

        return machines;
    }

    private static Task[] CreateTasks(int numberOfMachines, double r)
    {
        var numberOfTasks = (int)Math.Pow(numberOfMachines, r);
        var tasks = new Task[numberOfTasks];

        for (var i = 0; i < numberOfTasks; i++)
        {
            var duration = RandomHelper.GetRandomFromValuesRange(1, 100);
            tasks[i] = new Task(i, duration);
        }

        return tasks;
    }

    public static void MoveTask(Machine from, Machine to, Task task)
    {
        from.Tasks = from.Tasks.Where(t => t.Id != task.Id).ToArray();
        to.Tasks = to.Tasks.Append(task).ToArray();
    }
};