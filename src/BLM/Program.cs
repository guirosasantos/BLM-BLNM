using System.Diagnostics;
using BLM_BLNM;
using Lib;

var instances = GenerateValidInstances(10).ToList();

ExecuteBestImprovementSearchOnInstances(instances);

static IEnumerable<Instance> GenerateValidInstances(int count)
{
    for (int i = 0; i < count; i++)
        yield return GenerateRandomInstance();
}

static Instance GenerateRandomInstance()
{
    var numberOfMachines = RandomHelper.GetRandomFromValues([10, 20, 50]);
    var r = RandomHelper.GetRandomFromValuesDecimal([1.5, 2]);
    var instance1 = new Instance(numberOfMachines, r);
    return instance1;
}

static void ExecuteBestImprovementSearchOnInstances(List<Instance> instances)
{
    for (int i = 0; i < instances.Count; i++)
        ExecuteBestImprovementSearch(instances[i], i);
}

static void ExecuteBestImprovementSearch(Instance instance, int index)
{
    var iterations = 0;
    var sw = new Stopwatch();
    sw.Start();
    while (IsPossibleToImprove(instance))
    {
        iterations++;
        Instance.MoveTask(
            instance.MachineWithHighestMakeSpan,
            instance.MachineWithLowestMakeSpan,
            instance.MachineWithHighestMakeSpan.HighestTask!);
    }
    sw.Stop();
    ChartExtensions.PlotMachineTaskSums(instance, $"../../../Results/NaoMonotono_{index + 1}.html", sw.ElapsedMilliseconds, index, iterations);
}

static bool IsPossibleToImprove(Instance instance)
{
    var lowestMachineDuration = instance.MachineWithLowestMakeSpan.MakeSpan;
    var highestMachineHighestTaskDuration = instance.MachineWithHighestMakeSpan.HighestTask!.Duration;

    return lowestMachineDuration + highestMachineHighestTaskDuration < instance.MachineWithHighestMakeSpan.MakeSpan;
}
