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

public sealed record AlexReport(string heuristica, int n, int m, int replicacao, int tempo, int iteracoes, int valor, double parametro);

public sealed record FinalReportGroup(
    BLMReportGroup BLMReportGroup,
    List<BLNMReportGroup> BLNMReportGroups);

public sealed record BLMReportGroup((int NumberOfMachines, double R) Key, IEnumerable<ExecutionReport> Reports)
{
    public int NumberOfMachines => Key.NumberOfMachines;
    public double R => Key.R;
    public int NumberOfTasks => Reports.First().NumberOfTasks;
    public int AvgVariance => Reports.Sum(r => r.Variance) / Reports.Count();
    public int AvgOptimumMakeSpan => Reports.Sum(r => r.OptimumMakeSpan) / Reports.Count();
    public int AvgFinalMakeSpan => Reports.Sum(r => r.InstanceTotalMakeSpan) / Reports.Count();
    public int AvgOptimumMinusMakeSpan => Reports.Sum(r => r.OptimumMinusMakeSpan) / Reports.Count();
    public IEnumerable<AlexReport> AlexReports => Reports.Select(r => r.AlexReport);
    public int MaxTime => AlexReports.Max(r => r.tempo);
    public int MinTime => AlexReports.Min(r => r.tempo);
    public double AvgTime => AlexReports.Sum(r => r.tempo) / AlexReports.Count();
    public double AvgIterations => AlexReports.Sum(r => r.iteracoes) / AlexReports.Count();
}

public sealed record BLNMReportGroup((int NumberOfMachines, double R, double? A) Key, IEnumerable<ExecutionReport> Reports)
{
    public int NumberOfMachines => Key.NumberOfMachines;
    public double R => Key.R;
    public double Alpha => Key.A ?? throw new Exception("ALPHA NAO VEIO NO BLNM REPORT GROUP");
    public int NumberOfTasks => Reports.First().NumberOfTasks;
    public int AvgVariance => Reports.Sum(r => r.Variance) / Reports.Count();
    public int AvgOptimumMakeSpan => Reports.Sum(r => r.OptimumMakeSpan) / Reports.Count();
    public int AvgFinalMakeSpan => Reports.Sum(r => r.InstanceTotalMakeSpan) / Reports.Count();
    public int AvgOptimumMinusMakeSpan => Reports.Sum(r => r.OptimumMinusMakeSpan) / Reports.Count();
    public IEnumerable<AlexReport> AlexReports => Reports.Select(r => r.AlexReport);
    public int MaxTime => AlexReports.Max(r => r.tempo);
    public int MinTime => AlexReports.Min(r => r.tempo);
    public double AvgTime => AlexReports.Sum(r => r.tempo) / AlexReports.Count();
    public double AvgIterations => AlexReports.Sum(r => r.iteracoes) / AlexReports.Count();
}