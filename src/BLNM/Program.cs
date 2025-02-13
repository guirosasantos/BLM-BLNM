using System.Diagnostics;
using Lib;

namespace BLNM;

public static class Program
{
    public static void Main()
    {
        var sw = new Stopwatch();
        sw.Start();

        var blmExecutionReports = BLM.Run().ToList(); // 6 grupos

        Console.WriteLine("BLM CONCLUÍDO");
        Console.WriteLine("Total Time Spent BLM : " + sw.Elapsed.Minutes + " m " + sw.Elapsed.Seconds + " s " + sw.Elapsed.Milliseconds + " ms");

        var blnmExecutionReports = BLNM.Run().ToList(); // 54 grupos
        Console.WriteLine("BLNM CONCLUÍDO");
        sw.Stop();
        Console.WriteLine("Total Time Spent : " + sw.Elapsed.Minutes + " m" + sw.Elapsed.Seconds + " s" + sw.Elapsed.Milliseconds + " ms");

        WriteReportsIntoCsvFile(blmExecutionReports, blnmExecutionReports);

        var blmReportGroups = blmExecutionReports.Select(g => new BLMReportGroup(g.Key, g)).ToList();
        var blnmReportGroups = blnmExecutionReports.Select(g => new BLNMReportGroup(g.Key, g)).ToList();

        List<FinalReportGroup> finalReports = [];

        blmReportGroups.ForEach(r =>
        {
            var eqiuvalentBLNM = blnmReportGroups.Where(nmr =>
                nmr.Key.NumberOfMachines == r.Key.NumberOfMachines && Math.Abs(nmr.Key.R - r.Key.R) < 0.01);

            finalReports.Add(new FinalReportGroup(r, eqiuvalentBLNM.ToList()));
        });

        HtmlReportGenerator.GerarRelatoriosFinais(finalReports, "../../../../Lib/Results/FinalReport.html");
    }

    private static void WriteReportsIntoCsvFile(
        List<IGrouping<(int NumberOfMachines, double R), ExecutionReport>> blmExecutionReports,
        List<IGrouping<(int NumberOfMachines, double R, double? Alpha), ExecutionReport>> blnmExecutionReports)
    {
        WriteToCsvFile("../../../../Lib/Results/Report.csv", blmExecutionReports.SelectMany(g => g.Select(er => er.AlexReport)));
        WriteToCsvFile("../../../../Lib/Results/Report.csv", blnmExecutionReports.SelectMany(g => g.Select(er => er.AlexReport)));
    }

    private static void WriteToCsvFile(string path, IEnumerable<AlexReport> reports)
    {
        var directory = Path.GetDirectoryName(path);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory!);

        var fileExists = File.Exists(path);

        using var writer = new StreamWriter(path, true);

        if (!fileExists)
        {
            writer.WriteLine("heuristica,n,m,replicacao,tempo,iteracoes,valor,parametro");
        }

        foreach (var report in reports)
        {
            var line = $"{report.heuristica},{report.n},{report.m},{report.replicacao}," +
                       $"{report.tempo},{report.iteracoes},{report.valor},{report.parametro}";
            writer.WriteLine(line);
        }
    }
}


