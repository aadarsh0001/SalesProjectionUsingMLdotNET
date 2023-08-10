using System;
using System.Collections.Generic;
using System.Linq;
using OxyPlot.Annotations;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace HourlySalesReport.ViewModel
{
    public class HourlySalesVisualizationViewModel : HourlySalesReportViewModel
    {
        public HourlySalesVisualizationViewModel()
        {
            PlotLineModel = new PlotModel();
            PlotBarModel = new PlotModel();
            PlotBarGraph();
            PlotLineChart();
        }

        public void PlotLineChart()
        {
            var hours = ComparisionDataWithML.Select(x => x.Hours).ToList();
            var mlPredictData = ComparisionDataWithML.Select(x => x.ProjectedGuestByML).ToList();
            var sdmActualData = ComparisionDataWithML.Select(x => x.ActualGuestThroughSDM).ToList();
            var sdmPredictData = ComparisionDataWithML.Select(x => x.ProjectedGuestThroughSDM).ToList();

            var xAxis = new LinearAxis
            {
                Title = "Hours",
                Position = AxisPosition.Bottom, // Specify the position of the X-axis
                MajorStep = 1,
                Minimum = 6,
                Maximum = 23,
                TickStyle = TickStyle.Crossing // Display the ticks crossing the axis
            };
            PlotLineModel.Axes.Add(xAxis);

            // Y-axis with manual ticks
            var yAxis = new LinearAxis
            {
                Title = "Guest Count",
                MajorStep = 20, // Controls the spacing of ticks
                MajorTickSize = 20 // Controls the size of major ticks
            };
            PlotLineModel.Axes.Add(yAxis);

            var mlPredictSeries = new LineSeries { Title = "ML Prediction" };
            var sdmActualSeries = new LineSeries { Title = "Actual Prediction" };
            var sdmPredictSeries = new LineSeries { Title = "SDM Prediction" };

            for (int i = 0; i < hours.Count; i++)
            {
                mlPredictSeries.Points.Add(new DataPoint(hours[i], mlPredictData[i]));
                sdmActualSeries.Points.Add(new DataPoint(hours[i], sdmActualData[i]));
                sdmPredictSeries.Points.Add(new DataPoint(hours[i], sdmPredictData[i]));
            }

            PlotLineModel.Series.Add(mlPredictSeries);
            PlotLineModel.Series.Add(sdmActualSeries);
            PlotLineModel.Series.Add(sdmPredictSeries);

            PlotLineModel.Legends.Add(new OxyPlot.Legends.Legend
            {
                LegendPlacement = OxyPlot.Legends.LegendPlacement.Outside,
                LegendPosition = OxyPlot.Legends.LegendPosition.TopRight,
                LegendBackground = OxyColor.FromAColor(200, OxyColors.White),
                LegendBorder = OxyColors.Black
            });
        }

        public void PlotBarGraph()
        {
            var hours = ComparisionDataWithML.Select(x => x.Hours).ToList();
            var mlPredictData = ComparisionDataWithML.Select(x => x.ProjectedGuestByML).ToList();
            var sdmActualData = ComparisionDataWithML.Select(x => x.ActualGuestThroughSDM).ToList();
            var sdmPredictData = ComparisionDataWithML.Select(x => x.ProjectedGuestThroughSDM).ToList();

            var yAxis = new CategoryAxis
            {
                Title = "Hours",
                Position = AxisPosition.Left,
            };
            for (int i = 0; i < hours.Count; i++)
            {
                yAxis.Labels.Add(hours[i].ToString()); // Adding hour labels
            }
            PlotBarModel.Axes.Add(yAxis);

            var xAxis = new LinearAxis
            {
                Title = "Guest Count",
                Position = AxisPosition.Bottom,
                MajorStep = 20,
                MajorTickSize = 20,
                StartPosition = 0,
                EndPosition = 1
            };
            PlotBarModel.Axes.Add(xAxis);
            var mlPredictSeries = new BarSeries { Title = "ML Prediction" };
            var sdmActualSeries = new BarSeries { Title = "Actual Prediction" };
            var sdmPredictSeries = new BarSeries { Title = "SDM Prediction" };

            for (int i = 0; i < hours.Count; i++)
            {
                mlPredictSeries.Items.Add(new BarItem(mlPredictData[i]));
                sdmActualSeries.Items.Add(new BarItem(sdmActualData[i]));
                sdmPredictSeries.Items.Add(new BarItem(sdmPredictData[i]));

            }

            PlotBarModel.Series.Add(mlPredictSeries);
            PlotBarModel.Series.Add(sdmActualSeries);
            PlotBarModel.Series.Add(sdmPredictSeries);

            PlotBarModel.Legends.Add(new OxyPlot.Legends.Legend
            {
                LegendPlacement = OxyPlot.Legends.LegendPlacement.Outside,
                LegendPosition = OxyPlot.Legends.LegendPosition.TopRight,
                LegendBackground = OxyColor.FromAColor(200, OxyColors.White),
                LegendBorder = OxyColors.Black
            });
        }

        public PlotModel PlotLineModel { get; private set; }

        public PlotModel PlotBarModel{ get; private set; }

    }
 
}
