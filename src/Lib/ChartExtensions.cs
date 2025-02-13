using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lib; // Certifique-se de usar o namespace correto

public static class HtmlReportGenerator
{
    /// <summary>
    /// Gera um arquivo HTML com um gráfico para cada FinalReportGroup.
    /// Cada gráfico terá o título "X maquinas ; Y R" (extraído do BLMReportGroup) e exibirá:
    /// - No eixo X: as barras correspondentes aos valores de Alpha (para o BLMReportGroup será usado "0")
    /// - No eixo Y: o InstanceTotalMakeSpan.
    /// Ao clicar em uma barra, um quadradinho abaixo do gráfico mostrará os detalhes do grupo.
    /// A coluna com o menor valor de InstanceTotalMakeSpan será destacada em vermelho.
    /// </summary>
    /// <param name="finalReportGroups">Lista de FinalReportGroup</param>
    /// <param name="outputFilePath">Caminho do arquivo HTML de saída</param>
    public static void GerarRelatoriosFinais(List<FinalReportGroup> finalReportGroups, string outputFilePath)
    {
        // Para cada FinalReportGroup, preparamos um objeto com as informações necessárias para o gráfico.
        var chartJsonItems = new List<string>();

        foreach (var finalGroup in finalReportGroups)
        {
            var blm = finalGroup.BLMReportGroup;

            // Listas para armazenar os dados do gráfico
            var labelsList = new List<string>();
            var dataList = new List<int>();
            var tooltipsList = new List<string>();

            // 1) Adiciona os dados do BLMReportGroup – como Alpha não se aplica, usamos "0"
            labelsList.Add("0");
            dataList.Add(blm.InstanceTotalMakeSpan);
            string tooltipBLM =
                $"NumberOfMachines: {blm.NumberOfMachines}\\n" +
                $"R: {blm.R}\\n" +
                $"NumberOfTasks: {blm.NumberOfTasks}\\n" +
                $"Variance: {blm.Variance}\\n" +
                $"OptimumMakeSpan: {blm.OptimumMakeSpan}\\n" +
                $"InstanceTotalMakeSpan: {blm.InstanceTotalMakeSpan}\\n" +
                $"OptimumMinusMakeSpan: {blm.OptimumMinusMakeSpan}\\n" +
                $"MaxTime: {blm.MaxTime}\\n" +
                $"MinTime: {blm.MinTime}\\n" +
                $"AvgTime: {blm.AvgTime}";
            tooltipsList.Add(tooltipBLM);

            // 2) Adiciona os dados dos BLNMReportGroup (ordenados por Alpha)
            var sortedBLNM = finalGroup.BLNMReportGroups.OrderBy(r => r.Alpha).ToList();
            foreach (var blnm in sortedBLNM)
            {
                labelsList.Add(blnm.Alpha.ToString());
                dataList.Add(blnm.InstanceTotalMakeSpan);
                string tooltipBLNM =
                    $"NumberOfMachines: {blnm.NumberOfMachines}\\n" +
                    $"R: {blnm.R}\\n" +
                    $"Alpha: {blnm.Alpha}\\n" +
                    $"NumberOfTasks: {blnm.NumberOfTasks}\\n" +
                    $"Variance: {blnm.Variance}\\n" +
                    $"OptimumMakeSpan: {blnm.OptimumMakeSpan}\\n" +
                    $"InstanceTotalMakeSpan: {blnm.InstanceTotalMakeSpan}\\n" +
                    $"OptimumMinusMakeSpan: {blnm.OptimumMinusMakeSpan}\\n" +
                    $"MaxTime: {blnm.MaxTime}\\n" +
                    $"MinTime: {blnm.MinTime}\\n" +
                    $"AvgTime: {blnm.AvgTime}";
                tooltipsList.Add(tooltipBLNM);
            }

            // Converte as listas para arrays JSON (strings)
            string labelsJson = "[" + string.Join(", ", labelsList.Select(s => $"\"{s}\"")) + "]";
            string dataJson = "[" + string.Join(", ", dataList) + "]";
            string tooltipsJson = "[" + string.Join(", ", tooltipsList.Select(t => $"\"{t}\"")) + "]";

            // Define o título do gráfico usando os dados do BLMReportGroup
            string title = $"M = {blm.NumberOfMachines} | R = {blm.R}";
            string chartJson = $"{{\"title\": \"{title}\", \"labels\": {labelsJson}, \"data\": {dataJson}, \"tooltips\": {tooltipsJson}}}";
            chartJsonItems.Add(chartJson);
        }

        // Constrói a string JSON representando o array de gráficos
        string chartsJson = "[" + string.Join(", ", chartJsonItems) + "]";

        // Monta o conteúdo HTML com os gráficos organizados em mosaico (2 por linha)
        string htmlContent = $@"
<!DOCTYPE html>
<html>
<head>
  <meta charset='utf-8'>
  <title>Relatório Final</title>
  <!-- Carrega o Chart.js a partir do CDN -->
  <script src='https://cdn.jsdelivr.net/npm/chart.js'></script>
  <style>
    body {{
      background-color: #f9f9f9;
      font-family: Arial, sans-serif;
      padding: 20px;
    }}
    /* Mosaico: container flex com quebra de linha */
    #chartsContainer {{
      display: flex;
      flex-wrap: wrap;
      gap: 20px;
      justify-content: center;
    }}
    .chart-container {{
      width: calc(50% - 20px);
      border: 1px solid #ddd;
      padding: 10px;
      box-shadow: 2px 2px 5px rgba(0,0,0,0.1);
    }}
    .chart-container h2 {{
      text-align: center;
    }}
    .info-box {{
      margin-top: 10px;
      padding: 10px;
      background-color: #eee;
      border: 1px solid #ccc;
      display: none;
      white-space: pre-wrap; /* preserva as quebras de linha */
    }}
  </style>
</head>
<body>
  <div id='chartsContainer'></div>
  <script>
    // Array de gráficos gerado no servidor
    var charts = {chartsJson};

    var container = document.getElementById('chartsContainer');
    charts.forEach(function(chart, index) {{
         // Cria um elemento div para cada gráfico
         var div = document.createElement('div');
         div.className = 'chart-container';

         // Cria o título do gráfico
         var title = document.createElement('h2');
         title.textContent = chart.title;
         div.appendChild(title);

         // Cria o canvas para o gráfico
         var canvas = document.createElement('canvas');
         canvas.id = 'chart_' + index;
         canvas.width = 600;
         canvas.height = 400;
         div.appendChild(canvas);

         // Cria a div que exibirá os detalhes ao clicar na barra
         var infoBox = document.createElement('div');
         infoBox.id = 'infoBox_' + index;
         infoBox.className = 'info-box';
         div.appendChild(infoBox);

         container.appendChild(div);

         // Calcula as cores: a barra com o menor valor em chart.data ficará em vermelho
         var minVal = Math.min.apply(null, chart.data);
         var backgroundColors = chart.data.map(function(val) {{
              return (val === minVal) ? 'rgba(255, 99, 132, 0.2)' : 'rgba(54, 162, 235, 0.2)';
         }});
         var borderColors = chart.data.map(function(val) {{
              return (val === minVal) ? 'rgba(255, 99, 132, 1)' : 'rgba(54, 162, 235, 1)';
         }});

         // Cria o gráfico de barras com Chart.js e define um callback para o clique
         var ctx = canvas.getContext('2d');
         var myChart = new Chart(ctx, {{
              type: 'bar',
              data: {{
                  labels: chart.labels,
                  datasets: [{{
                      label: 'Instance Total MakeSpan',
                      data: chart.data,
                      backgroundColor: backgroundColors,
                      borderColor: borderColors,
                      borderWidth: 1
                  }}]
              }},
              options: {{
                  scales: {{
                      y: {{
                          beginAtZero: true
                      }}
                  }},
                  onClick: function(evt, activeElements) {{
                      if(activeElements.length > 0) {{
                          var idx = activeElements[0].index;
                          var info = chart.tooltips[idx];
                          // Atualiza e exibe a info box com as informações detalhadas
                          var infoBox = document.getElementById('infoBox_' + index);
                          infoBox.innerHTML = info.replace(/\\n/g, '<br>');
                          infoBox.style.display = 'block';
                      }}
                  }},
                  plugins: {{
                      tooltip: {{
                          enabled: false // Desabilita o tooltip padrão
                      }}
                  }}
              }}
         }});
    }});
  </script>
</body>
</html>
";

        // Escreve o conteúdo HTML no arquivo de saída
        File.WriteAllText(outputFilePath, htmlContent);
    }
}
