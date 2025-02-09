using Lib;

var numberOfMachines = RandomHelper.GetRandomFromValues([10, 20, 50]);
var r = RandomHelper.GetRandomFromValuesDecimal([1.5, 2]);
var instance = new Instance(numberOfMachines, r);

ExecuteBestImprovementSearch(instance);

static void ExecuteBestImprovementSearch(Instance instance)
{
    bool improvementIsPossible = IsPossibleToImprove(instance);

    while (improvementIsPossible)
    {
        
    }
}

static bool IsPossibleToImprove(Instance instance)
{
    var machineWithLowestMakeSpan = instance.MachineWithLowestMakeSpan;
    var machineWithHighestMakeSpan = instance.MachineWithHighestMakeSpan;

    var lowestTask = machineWithLowestMakeSpan.LowestTask;
    var highestTask = machineWithHighestMakeSpan.HighestTask;

    if (lowestTask is null || highestTask is null)
        return false;

    return lowestTask.Duration < highestTask.Duration;
}
