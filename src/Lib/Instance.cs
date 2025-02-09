namespace Lib;

public sealed record Instance(
    int NumberOfMachines, //M
    double R
)
{
    public Machine[] Machines { get; set; } = CreateInstanceMachines(
        NumberOfMachines, R);

    private static Machine[] CreateInstanceMachines(
        int NumberOfMachines, double r)
    {
        var machines = new Machine[NumberOfMachines];
        var tasks = CreateTasks(NumberOfMachines, r);

        var tasksPerMachine = tasks.Length / NumberOfMachines;
        var remainder = tasks.Length % NumberOfMachines;
        var startIndex = 0;

        for (var i = 0; i < NumberOfMachines; i++)
        {
            var count = tasksPerMachine + (i < remainder ? 1 : 0);
            var tasksForMachine = tasks.Skip(startIndex).Take(count).ToArray();
            machines[i] = new Machine(i, tasksForMachine);
            startIndex += count;
        }

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
};