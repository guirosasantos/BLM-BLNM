using Lib;

var numberOfMachines = RandomHelper.GetRandomFromValues([10, 20, 50]);
var r = RandomHelper.GetRandomFromValuesDecimal([1.5, 2]);
var instance = CreateRandomInstance(numberOfMachines, r);

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
