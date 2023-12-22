using Avalonia.Controls;
using LiveChartsCore.SkiaSharpView.Avalonia;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using System.Collections.Generic;
using System;
using System.Linq;
using LiveChartsCore.SkiaSharpView.VisualElements;
using Avalonia.Threading;
using System.Threading.Tasks;

namespace GraphsGUI
{
   /// <summary>
   /// <b>Author: Samuel Gardner</b></br></br>
   /// </summary>
   public partial class MainWindow : Window
   {
      public MainWindow()
      {
         InitializeComponent();

         _ = Task.Run(async () =>
         {
            var r = await Analysis.StepThreeAnalysis();
            decimal min = r.Select(x => x.percentages.Select(y => y.Value)).SelectMany(x => x).MinBy(x => 
            {
               if (x < -1000000m)
                  return decimal.MaxValue;
               else
                  return x;
            }) * 100m, max = r.Select(x => x.percentages.Select(y => y.Value)).SelectMany(x => x).MaxBy(x =>
            {
               if (x > 1000000m)
                  return decimal.MinValue;
               else
                  return x;
            }) * 100m;
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
               graphName.Text = "Ready";
               previousButton.IsEnabled = true;
               nextButton.IsEnabled = true;
               int ind = 0;
               void CreateGraph()
               {
                  var val = r[ind];
                  List<(string label, double val)> data = new(val.percentages.Select(x =>
                  {
                     if (x.Value > 10000m) //infinity
                        return ($"{x.Key.formerYear}-{x.Key.latterYear} (+∞)", 0d);
                     else
                        return ($"{x.Key.formerYear}-{x.Key.latterYear}", ((double)x.Value) * 100d);
                  }));
                  string title = $"Percentage deltas {Analysis.tagsNamesLookupTable[val.l.source]} <-> {Analysis.tagsNamesLookupTable[val.l.target]}";
                  graphName.Text = title;
                  Control c = CreateBarGraph(data, title, "Time", title, (vertNormaliz.IsChecked ?? false) ? null : min, (vertNormaliz.IsChecked ?? false) ? null : max);
                  contentPanel.Children.Clear();
                  contentPanel.Children.Add(c);
               }
               previousButton.Click += (_, _) =>
               {
                  ind--;
                  ind = ind < 0 ? r.Count - 1 : ind;
                  CreateGraph();
               };
               nextButton.Click += (_, _) =>
               {
                  ind++;
                  ind = ind > r.Count - 1 ? 0 : ind;
                  CreateGraph();
               };
            });
         });
      }

      private static Control CreateBarGraph(List<(string label, double val)> data, string? title = null, string? xAxisLabel = null, string? yAxisLabel = null, decimal? min = null, decimal? max = null)
      {
         IEnumerable<ISeries> ds = new[]
         {
            new ColumnSeries<double>()
            {
               Values = data.Select(x => x.val)
            }
         };

         CartesianChart c = new CartesianChart()
         {
            Title = new LabelVisual() { Text = title ?? "Bar Graph" },
            Series = ds,
            XAxes = new[] { new Axis()
            {
               Name = xAxisLabel ?? "X Axis",
               Labels = data.Select(x => x.label).ToList(),
               MinLimit = -.5,
               ForceStepToMin = false,
               MinStep = 1,
               LabelsRotation = 60
            } },
            YAxes = new[] { new Axis()
            {
               Name = yAxisLabel ?? "Y Axis",
               //Labels = yLabels ?? null,
               MinLimit = ((double?)min),
               MaxLimit = ((double?)max),
               ForceStepToMin = false,
               MinStep = 1
            } },
         };

         return c;
      }
   }
}