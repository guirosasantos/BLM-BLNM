using System.Diagnostics;
using Lib;

namespace BLNM;

public static class BLM
{
    public static IEnumerable<IGrouping<(int NumberOfMachines, double R), ExecutionReport>> Run()
    {
        Instance instance;
        List<ExecutionReport> reports = [];

        for (int m = 0; m < 3; m++)
        {
            for (int r = 0; r < 2; r++)
            {
                for (int i = 0; i < 10; i++)
                {
                    var parameters = GetParameters(m, r);
                    instance = GenerateInstance(parameters.Machine, parameters.R);
                    reports.Add(ExecuteBestImprovementSearch(instance, i));
                }
            }
        }

        return reports
            .GroupBy(r => (r.NumberOfMachines, r.R));

        // var reportGroups = groupedReports.Select(g => new BLMReportGroup(g.Key, g)).ToList();
        // ChartExtensions.GerarRelatorioBLM(reportGroups, "../../../../Lib/Results/BLMReport.html");
        // Console.WriteLine();
    }


    static ExecutionReport ExecuteBestImprovementSearch(Instance instance, int index)
    {
        var iterations = 0;
        var sw = new Stopwatch();
        sw.Start();
        while (true)
        {
            if (IsPossibleToImproveHigh(instance))
            {
                iterations++;
                Instance.MoveTask(
                    instance.MachineWithHighestMakeSpan,
                    instance.MachineWithLowestMakeSpan,
                    instance.MachineWithHighestMakeSpan.HighestTask!);
            }
            else if (IsPossibleToImproveLow(instance))
            {
                iterations++;
                Instance.MoveTask(
                    instance.MachineWithHighestMakeSpan,
                    instance.MachineWithLowestMakeSpan,
                    instance.MachineWithHighestMakeSpan.LowestTask!);
            }
            else
            {
                break;
            }
        }

        sw.Stop();
        return new ExecutionReport(
            instance,
            new AlexReport(
                "Melhor Melhora",
                instance.NumberOfTasks,
                instance.NumberOfMachines,
                index,
                sw.Elapsed.Milliseconds,
                iterations,
                instance.MakeSpan,
                0
            ));
    }

    static bool IsPossibleToImproveHigh(Instance instance)
    {
        var lowestMachineDuration = instance.MachineWithLowestMakeSpan.MakeSpan;
        var highestMachineHighestTaskDuration = instance.MachineWithHighestMakeSpan.HighestTask!.Duration;

        return lowestMachineDuration + highestMachineHighestTaskDuration < instance.MachineWithHighestMakeSpan.MakeSpan;
    }

    static bool IsPossibleToImproveLow(Instance instance)
    {
        var highestMakeSpanMachine = instance.MachineWithHighestMakeSpan;
        var lowesttask = highestMakeSpanMachine.LowestTask;
        var makeSpanWithoutLowestTask = highestMakeSpanMachine.MakeSpan - lowesttask?.Duration ?? 0;
        var lowestMakeSpanMachine = instance.MachineWithLowestMakeSpan;
        var makeSpanWithLowestTask = lowestMakeSpanMachine.MakeSpan + lowesttask?.Duration ?? 0;
        return makeSpanWithoutLowestTask < instance.MakeSpan &&
               makeSpanWithLowestTask < instance.MakeSpan;
    } 

    static (int Machine, double R) GetParameters(int m, int r)
    {
        int[] numberOfMachinesPossibilities = [10, 20, 50];
        double[] rPossibilities = [1.5, 2];
        return (numberOfMachinesPossibilities[m], rPossibilities[r]);
    }

    static Instance GenerateInstance(int m, double r)
    {
        var instance = new Instance(m, r);
        return instance;
    }
}