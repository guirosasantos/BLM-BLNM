namespace Lib;

public class ExecutionReport
{
    public int NumberOfMachines { get; private set; }
    public double R { get; private set; }
    public int NumberOfTasks => (int)Math.Pow(NumberOfMachines, R);
    public double? Alpha { get; private set; }
    public int Variance { get; private set; }
    public int OptimumMakeSpan { get; private set; }
    public int InstanceTotalMakeSpan { get; private set; }
    public int OptimumMinusMakeSpan => InstanceTotalMakeSpan - OptimumMakeSpan;
    public AlexReport AlexReport { get; private set; }

    public ExecutionReport(Instance instance, AlexReport alexReport, double? alpha = null)
    {
        NumberOfMachines = instance.NumberOfMachines;
        R = instance.R;
        Variance = instance.MachineWithHighestMakeSpan.MakeSpan - instance.MachineWithLowestMakeSpan.MakeSpan;
        OptimumMakeSpan = instance.OriginalMakeSpan / NumberOfMachines;
        InstanceTotalMakeSpan = instance.MakeSpan;
        Alpha = alpha;
        AlexReport = alexReport;
    }
}

public sealed record AlexReport(string heuristica, int n, int m, int replicacao, long tempo, int iteracoes, int valor, double parametro);

public sealed record FinalReportGroup(
    BLMReportGroup BLMReportGroup,
    List<BLNMReportGroup> BLNMReportGroups);

public sealed record BLMReportGroup((int NumberOfMachines, double R) Key, IEnumerable<ExecutionReport> Reports)
{
    public int NumberOfMachines => Key.NumberOfMachines;
    public double R => Key.R;
    public int NumberOfTasks => Reports.First().NumberOfTasks;
    public int Variance => Reports.Sum(r => r.Variance) / 10;
    public int OptimumMakeSpan => Reports.Sum(r => r.OptimumMakeSpan) / 10;
    public int InstanceTotalMakeSpan => Reports.Sum(r => r.InstanceTotalMakeSpan) / 10;
    public int OptimumMinusMakeSpan => Reports.Sum(r => r.OptimumMinusMakeSpan) / 10;
    public IEnumerable<AlexReport> AlexReports => Reports.Select(r => r.AlexReport);
    public long MaxTime => AlexReports.Max(r => r.tempo);
    public long MinTime => AlexReports.Min(r => r.tempo);
    public double AvgTime => AlexReports.Average(r => r.tempo);
}

public sealed record BLNMReportGroup((int NumberOfMachines, double R, double? A) Key, IEnumerable<ExecutionReport> Reports)
{
    public int NumberOfMachines => Key.NumberOfMachines;
    public double R => Key.R;
    public double Alpha => Key.A ?? throw new Exception("ALPHA NAO VEIO NO BLNM REPORT GROUP");
    public int NumberOfTasks => Reports.First().NumberOfTasks;
    public int Variance => Reports.Sum(r => r.Variance) / 10;
    public int OptimumMakeSpan => Reports.Sum(r => r.OptimumMakeSpan) / 10;
    public int InstanceTotalMakeSpan => Reports.Sum(r => r.InstanceTotalMakeSpan) / 10;
    public int OptimumMinusMakeSpan => Reports.Sum(r => r.OptimumMinusMakeSpan) / 10;
    public IEnumerable<AlexReport> AlexReports => Reports.Select(r => r.AlexReport);
    public long MaxTime => AlexReports.Max(r => r.tempo);
    public long MinTime => AlexReports.Min(r => r.tempo);
    public double AvgTime => AlexReports.Average(r => r.tempo);
}