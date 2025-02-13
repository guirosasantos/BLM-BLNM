using System.Diagnostics;
using Lib;

namespace BLNM;

public static class BLNM
{
    public static IEnumerable<IGrouping<(int NumberOfMachines, double R, double? Alpha), ExecutionReport>> Run()
    {
        Instance instance;
        List<ExecutionReport> reports = [];

        for (int a = 0; a < 9; a++)
        {
            for (int m = 0; m < 3; m++)
            {
                for (int r = 0; r < 2; r++)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        var parameters = GetParameters(m, r, a);
                        instance = GenerateInstance(parameters.Machine, parameters.R);
                        reports.Add(ExecuteIteratedSearch(instance, i, parameters.Alpha));
                    }
                }
            }
        }

        return reports
            .GroupBy(r => (r.NumberOfMachines, r.R, r.Alpha));
    }
    
static ExecutionReport ExecuteIteratedSearch(Instance instance, int index, double alpha)
{
    var bestMakespan = instance.MakeSpan;
    var clone = instance with
    {
        Machines = instance.Machines.Select(m => new Machine(m.Id, m.Tasks.ToArray())).ToArray()
    };
    var iterations = 0;
    var sw = new Stopwatch();
    var unimprovedIterations = 0;
    sw.Start();
    while (unimprovedIterations <= 10)
    {
        iterations++;
        if (IsPossibleToImprove(instance))
        {
            Instance.MoveTask(
                instance.MachineWithHighestMakeSpan,
                instance.MachineWithLowestMakeSpan,
                instance.MachineWithHighestMakeSpan.HighestTask!);
        }
        else
        {
            if (instance.MakeSpan >= bestMakespan)
            {
                unimprovedIterations++;
            }
            else
            {
                bestMakespan = instance.MakeSpan;
                clone = instance with
                {
                    Machines = instance.Machines.Select(m => new Machine(m.Id, m.Tasks.ToArray())).ToArray()
                };
                unimprovedIterations = 0;
            }
 
            ScrambleTasks(instance, alpha);
        }
    }

    instance = clone;
    sw.Stop();
    return new ExecutionReport(
        instance,
        new AlexReport(
            "Melhor Melhora",
            instance.NumberOfTasks,
            instance.NumberOfMachines,
            index,
            sw.ElapsedMilliseconds,
            iterations,
            instance.MakeSpan,
            alpha),
        alpha);
}

static bool IsPossibleToImprove(Instance instance)
{
    var lowestMachineDuration = instance.MachineWithLowestMakeSpan.MakeSpan;
    var highestMachineHighestTaskDuration = instance.MachineWithHighestMakeSpan.HighestTask!.Duration;

    return lowestMachineDuration + highestMachineHighestTaskDuration < instance.MachineWithHighestMakeSpan.MakeSpan;
}

static (int Machine, double R, double Alpha) GetParameters(int m, int r, int a)
{
    int[] numberOfMachinesPossibilities = [10, 20, 50];
    double[] rPossibilities = [1.5, 2];
    double[] alphaPossibilities = [0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9];
    return (numberOfMachinesPossibilities[m], rPossibilities[r], alphaPossibilities[a]);
}

static Instance GenerateInstance(int m, double r)
{
    var instance = new Instance(m, r);
    return instance;
}

static void ScrambleTasks(Instance instance, double alpha)
{
    var taskAmount = (int)Math.Pow(instance.NumberOfMachines, instance.R);
    var scrambledTaskAmount = taskAmount * alpha;
    for (var i = 0; i < scrambledTaskAmount; i++)
    {
        var machines = instance.Machines.Where(m => m.Tasks.Length != 0).ToArray();
        var rand = new Random();
        var fromMachine = machines[rand.Next(machines.Length)];
        var toMachine = machines[rand.Next(machines.Length)];
        Instance.MoveTask(
                fromMachine,
                toMachine,
                fromMachine.Tasks[rand.Next(fromMachine.Tasks.Length)]);
    }
}
}