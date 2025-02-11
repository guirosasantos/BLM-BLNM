﻿using Lib;

var numberOfMachines = RandomHelper.GetRandomFromValues([10, 20, 50]);
var r = RandomHelper.GetRandomFromValuesDecimal([1.5, 2]);
var instance = new Instance(numberOfMachines, r);

var oldMakeSpan = instance.MakeSpan;

ExecuteBestImprovementSearch(instance);

PrintTasksByMachine(instance, oldMakeSpan);

static void ExecuteBestImprovementSearch(Instance instance)
{
    while (IsPossibleToImprove(instance))
    {
        Instance.MoveTask(
            instance.MachineWithHighestMakeSpan,
            instance.MachineWithLowestMakeSpan,
            instance.MachineWithHighestMakeSpan.HighestTask!);
    }
}

/* static bool IsPossibleToImprove(Instance instance)
{
    var highestMakeSpanMachine = instance.MachineWithHighestMakeSpan;
    var lowesttask = highestMakeSpanMachine.LowestTask;
    var makeSpanWithoutLowestTask = highestMakeSpanMachine.MakeSpan - lowesttask?.Duration ?? 0;

    var lowestMakeSpanMachine = instance.MachineWithLowestMakeSpan;
    var makeSpanWithLowestTask = lowestMakeSpanMachine.MakeSpan + lowesttask?.Duration ?? 0;

    return makeSpanWithoutLowestTask < instance.MakeSpan &&
           makeSpanWithLowestTask < instance.MakeSpan;
} */
//teste git

static bool IsPossibleToImprove(Instance instance)
{
    var lowestMachineDuration = instance.MachineWithLowestMakeSpan.MakeSpan;
    var highestMachineHighestTaskDuration = instance.MachineWithHighestMakeSpan.HighestTask!.Duration;

    return lowestMachineDuration + highestMachineHighestTaskDuration < instance.MachineWithHighestMakeSpan.MakeSpan;
}

static void PrintTasksByMachine(Instance instance, int oldMakeSpan)
{
    Console.WriteLine($"Old MakeSpan: {oldMakeSpan}");

    foreach (var machine in instance.Machines)
    {
        Console.WriteLine("-------------------------------------------------");
        Console.WriteLine($"Machine {machine.Id} tasks:");

        foreach (var task in machine.Tasks)
            Console.WriteLine($"Task {task.Id} with duration {task.Duration}");

        Console.WriteLine($"Machine Total MakeSpan: {machine.MakeSpan}");
        Console.WriteLine("-------------------------------------------------");
    }

    Console.WriteLine($"Instance Total MakeSpan: {instance.MakeSpan}");
}
