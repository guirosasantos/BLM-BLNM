using System.Diagnostics;
using System.Globalization;
using Lib;

namespace BLNM;

public static class Program
{
    public static void Main()
    {

        var blnmExecutionReports = BLNM.Run().ToList(); // 54 grupos
        var blmExecutionReports = BLM.Run().ToList(); // 6 grupos
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
}


