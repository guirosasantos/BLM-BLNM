using System.Diagnostics;
using System.Globalization;
using Lib;


var instances = new List<Instance>();
for (int i = 0; i < 10; i++)
{
    instances.Add(GenerateRandomInstance());
}

for (int i = 0; i < 10; i++)
{
    var instance = instances[i];
    ExecuteBestImprovementSearch(instance, i, 0.9);
}


static void ExecuteBestImprovementSearch(Instance instance, int i, double scrambleCoeficient)
{
    var bestMakespan = instance.MakeSpan;
    var j = 0;
    var sw = new Stopwatch();
    var unimprovedIterations = 0;
    sw.Start();
    while (unimprovedIterations <= 1000)
    {
        j++;
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
                Console.WriteLine("ZERADO" + instance.MakeSpan);
                Console.WriteLine("MELHOROU: " + bestMakespan);
                bestMakespan = instance.MakeSpan;
                unimprovedIterations = 0;
            }
 
            ScrambleTasks(instance, scrambleCoeficient);
            Console.WriteLine("iteracao: " + unimprovedIterations);
        }
    }
    sw.Stop();
    ChartExtensions.PlotMachineTaskSums(instance, $"../../../Results/Iterada_{i+1}.html", sw.ElapsedMilliseconds, i, j, scrambleCoeficient.ToString(CultureInfo.InvariantCulture) , "Iterada");
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

static void ScrambleTasks(Instance instance, double scrambleCoeficient)
{
    var taskAmount = (int)Math.Pow(instance.NumberOfMachines, instance.R);
    var scrambledTaskAmount = taskAmount * scrambleCoeficient;
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
