using Lib;

var numberOfMachines = GetRandomFromValues([10, 20, 50]);
var r = GetRandomFromValuesDecimal([1.5, 2]);

var instance = CreateRandomInstance(numberOfMachines, r);

var makeSpan = CalculateMakespan(instance);

ExecuteLocalSearch(instance, makeSpan);

#region Helper methods
static int GetRandomFromValues(int[] values)
{
    var rnd = new Random();
    return values[rnd.Next(values.Length)];
}

static double GetRandomFromValuesDecimal(double[] values)
{
    var rnd = new Random();
    return values[rnd.Next(values.Length)];
}

static Instance CreateRandomInstance(int m, double r)
{
    var instance = new Instance(m, r);
    CreateRandomTasks(instance);
    return instance;
}

static void CreateRandomTasks(Instance instance)
{
    for (var i = 0; i < instance.Tasks.Length; i++)
    {
        var duration = GetRandomFromValuesRange(1, 100);
        instance.Tasks[i] = new Task(i, duration);
    }
}

static int GetRandomFromValuesRange(int min, int max)
{
    var rnd = new Random();
    return rnd.Next(min, max);
}

static int CalculateMakespan(Instance instance)
    => instance.Tasks.Sum(t => t.Duration);

static void ExecuteLocalSearch(Instance instance, int makeSpan)
{
    var s = instance;
    var sPrime = s;

    do
    {
        sPrime = GetNeighbor(s);
        if (CalculateMakespan(sPrime) < CalculateMakespan(s))
        {
            s = sPrime;
        }
    } while (CalculateMakespan(sPrime) < CalculateMakespan(s));

    return s;
}

#endregion
