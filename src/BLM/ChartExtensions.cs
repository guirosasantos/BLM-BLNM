using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lib;  // Assumed to contain the Instance class and its properties

namespace BLM_BLNM;

public static class ChartExtensions
{
  /// <summary>
  /// Generates an HTML file that displays a vertical bar chart (using Chart.js) of machine task sums.
  /// The maximum task sum is highlighted in red. Extra annotations are added below the chart.
  /// Also appends a CSV-formatted line to a CSV file without erasing previous content.
  /// </summary>
  /// <param name="instance">The instance containing machine data.</param>
  /// <param name="outputFilePath">The path (including filename) where the HTML file will be saved.</param>
  /// <param name="elapsedTime">Elapsed time for execution.</param>
  /// <param name="execNumber">Execution number.</param>
  /// <param name="iterations">Number of iterations.</param>
  public static void PlotMachineTaskSums(
    Instance instance,
    string outputFilePath,
    long elapsedTime,
    int execNumber,
    int iterations)
  {
    // Gather data from the instance.
    var machines = instance.Machines;
    var labels = new List<string>();
    var data = new List<double>();

    double maxTaskSum = double.MinValue;
    int maxIndex = -1;
    int index = 0;

    foreach (var machine in machines)
    {
      // Assuming machine.Id is an integer identifier and machine.MakeSpan is the task sum.
      labels.Add(machine.Id.ToString());
      data.Add(machine.MakeSpan);

      if (machine.MakeSpan > maxTaskSum)
      {
        maxTaskSum = machine.MakeSpan;
        maxIndex = index;
      }
      index++;
    }
            
    var variance = instance.MachineWithHighestMakeSpan.MakeSpan - instance.MachineWithLowestMakeSpan.MakeSpan;
    var optimumMakespan = instance.OriginalMakeSpan / instance.NumberOfMachines;
            
    Console.WriteLine($"Number of Machines: {instance.NumberOfMachines}");
    Console.WriteLine($"R: {instance.R}");
    Console.WriteLine($"Makespan Variance: {variance}");
    Console.WriteLine($"Optimum Makespan: {optimumMakespan}");
    Console.WriteLine($"Instance Total MakeSpan: {instance.MakeSpan}");
    Console.WriteLine($"Optimum - Makespan : {instance.MakeSpan - optimumMakespan}");
    Console.WriteLine("-------------------------------------------------");

    // Build color arrays so that the maximum value is highlighted.
    var backgroundColors = new List<string>();
    var borderColors = new List<string>();

    for (int i = 0; i < data.Count; i++)
    {
      if (i == maxIndex)
      {
        backgroundColors.Add("rgba(255, 99, 132, 0.2)"); // red-ish background
        borderColors.Add("rgba(255, 99, 132, 1)");        // red border
      }
      else
      {
        backgroundColors.Add("rgba(54, 162, 235, 0.2)");  // blue-ish background
        borderColors.Add("rgba(54, 162, 235, 1)");         // blue border
      }
    }

    // Convert lists to JavaScript array literals.
    string jsLabels = "[" + string.Join(", ", labels.Select(l => $"\"{l}\"")) + "]";
    string jsData = "[" + string.Join(", ", data) + "]";
    string jsBackgroundColors = "[" + string.Join(", ", backgroundColors.Select(c => $"\"{c}\"")) + "]";
    string jsBorderColors = "[" + string.Join(", ", borderColors.Select(c => $"\"{c}\"")) + "]";

    // Create the HTML content.
    string htmlContent = $@"
<!DOCTYPE html>
<html>
<head>
  <meta charset='utf-8'>
  <title>Machine Task Sums</title>
  <!-- Load Chart.js from CDN -->
  <script src='https://cdn.jsdelivr.net/npm/chart.js'></script>
  <style>
    body {{
      background-color: #f9f9f9;
      font-family: Arial, sans-serif;
      padding: 20px;
    }}
    .chart-container {{
      width: 600px;
      margin: auto;
    }}
    .footer-annotations {{
      text-align: center;
      margin-top: 20px;
      font-size: 14px;
      color: #333;
    }}
  </style>
</head>
<body>
  <div class='chart-container'>
    <canvas id='myChart' width='600' height='400'></canvas>
  </div>
  <div class='footer-annotations'>
    <p>Number of Machines: {instance.NumberOfMachines}</p>
    <p>R: {instance.R}</p>
    <p>Makespan Variance: {variance}</p>
    <p>Optimum Makespan: {optimumMakespan}</p>
    <p>Instance Total MakeSpan: {instance.MakeSpan}</p>
    <p>Optimum - Makespan : {instance.MakeSpan - optimumMakespan}</p>
  </div>
  <script>
    var ctx = document.getElementById('myChart').getContext('2d');
    var myChart = new Chart(ctx, {{
      type: 'bar',
      data: {{
        labels: {jsLabels},
        datasets: [{{
          label: 'Task Sum',
          data: {jsData},
          backgroundColor: {jsBackgroundColors},
          borderColor: {jsBorderColors},
          borderWidth: 1
        }}]
      }},
      options: {{
        responsive: false,
        scales: {{
          y: {{
            beginAtZero: true
          }}
        }},
        plugins: {{
          legend: {{
            display: true,
            position: 'top'
          }},
          title: {{
            display: true,
            text: 'Machine Task Sums'
          }}
        }}
      }}
    }});
  </script>
</body>
</html>";

    // Write the HTML content to the output file.
    File.WriteAllText(outputFilePath, htmlContent);
    // Append a CSV line to the results file (without overwriting previous content).
    string csvFilePath = "../../../Results/results.csv";
    string csvLine = $"Melhor Melhora; {(int)Math.Pow(instance.NumberOfMachines, instance.R)}; {instance.NumberOfMachines}; {execNumber}; {elapsedTime}; {iterations}; {instance.MakeSpan}; NA";
    File.AppendAllText(csvFilePath, csvLine + Environment.NewLine);
  }
}