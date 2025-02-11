using System.Diagnostics;
using BLM_BLNM;
using Lib;


var instances = new List<Instance>();
for (int i = 0; i < 10; i++)
{
    instances.Add(GenerateRandomInstance());
}

for (int i = 0; i < 10; i++)
{
    var instance = instances[i];
    ExecuteBestImprovementSearch(instance, i);
}


static void ExecuteBestImprovementSearch(Instance instance, int i)
{
    var j = 0;
    var sw = new Stopwatch();
    sw.Start();
    while (IsPossibleToImprove(instance))
    {
        j++;
        Instance.MoveTask(
            instance.MachineWithHighestMakeSpan,
            instance.MachineWithLowestMakeSpan,
            instance.MachineWithHighestMakeSpan.HighestTask!);
    }
    sw.Stop();
    ChartExtensions.PlotMachineTaskSums(instance, $"../../../Results/NaoMonotono_{i+1}.html", sw.ElapsedMilliseconds, i, j);
}

static bool IsPossibleToImprove(Instance instance)
{
    var lowestMachineDuration = instance.MachineWithLowestMakeSpan.MakeSpan;
    var highestMachineHighestTaskDuration = instance.MachineWithHighestMakeSpan.HighestTask!.Duration;

    return lowestMachineDuration + highestMachineHighestTaskDuration < instance.MachineWithHighestMakeSpan.MakeSpan;
}

Instance GenerateRandomInstance()
{
    var numberOfMachines = RandomHelper.GetRandomFromValues([10, 20, 50]);
    var r = RandomHelper.GetRandomFromValuesDecimal([1.5, 2]);
    var instance1 = new Instance(numberOfMachines, r);
    return instance1;
}
