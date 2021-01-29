using Blazorise.Charts;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FxaPortal.Pages
{
    public class IndexBase : ComponentBase
    {
        protected LineChart<double> lineChart;
        protected Chart<double> barChart;
        protected Chart<double> pieChart;
        protected Chart<double> doughnutChart;
        protected Chart<double> polarAreaChart;
        protected Chart<double> radarChart;

        string[] Labels = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        string[] LabelsDays = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12" };
        List<string> backgroundColors = new List<string> { ChartColor.FromRgba(255, 99, 132, 0.2f), ChartColor.FromRgba(54, 162, 235, 0.2f), ChartColor.FromRgba(255, 206, 86, 0.2f), ChartColor.FromRgba(75, 192, 192, 0.2f), ChartColor.FromRgba(153, 102, 255, 0.2f), ChartColor.FromRgba(255, 159, 64, 0.2f), ChartColor.FromRgba(255, 99, 132, 0.2f), ChartColor.FromRgba(54, 162, 235, 0.2f), ChartColor.FromRgba(255, 206, 86, 0.2f), ChartColor.FromRgba(75, 192, 192, 0.2f), ChartColor.FromRgba(153, 102, 255, 0.2f), ChartColor.FromRgba(255, 159, 64, 0.2f) };
        List<string> borderColors = new List<string> { ChartColor.FromRgba(255, 99, 132, 1f), ChartColor.FromRgba(54, 162, 235, 1f), ChartColor.FromRgba(255, 206, 86, 1f), ChartColor.FromRgba(75, 192, 192, 1f), ChartColor.FromRgba(153, 102, 255, 1f), ChartColor.FromRgba(255, 159, 64, 1f), ChartColor.FromRgba(255, 99, 132, 1f), ChartColor.FromRgba(54, 162, 235, 1f), ChartColor.FromRgba(255, 206, 86, 1f), ChartColor.FromRgba(75, 192, 192, 1f), ChartColor.FromRgba(153, 102, 255, 1f), ChartColor.FromRgba(255, 159, 64, 1f) };

        bool isAlreadyInitialised;

        Random random = new Random(DateTime.Now.Millisecond);

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!isAlreadyInitialised)
            {
                isAlreadyInitialised = true;

                await Task.WhenAll(
                    HandleRedraw(lineChart, GetLineChartDataset),
                    HandleRedraw2(barChart, GetBarChartDataset));
            }
        }

        protected async Task HandleRedraw<TDataSet, TItem, TOptions, TModel>(Blazorise.Charts.BaseChart<TDataSet, TItem, TOptions, TModel> chart, Func<TDataSet> getDataSet)
            where TDataSet : ChartDataset<TItem>
            where TOptions : ChartOptions
            where TModel : ChartModel
        {
            await chart.Clear();

            await chart.AddLabelsDatasetsAndUpdate(Labels, getDataSet());
        }

        protected async Task HandleRedraw2<TDataSet, TItem, TOptions, TModel>(Blazorise.Charts.BaseChart<TDataSet, TItem, TOptions, TModel> chart, Func<TDataSet> getDataSet)
            where TDataSet : ChartDataset<TItem>
            where TOptions : ChartOptions
            where TModel : ChartModel
        {
            await chart.Clear();

            await chart.AddLabelsDatasetsAndUpdate(LabelsDays, getDataSet());
        }

        protected LineChartDataset<double> GetLineChartDataset()
        {
            return new LineChartDataset<double>
            {
                Label = "# of Emails by Month",
                Data = RandomizeData(),
                BackgroundColor = backgroundColors,
                BorderColor = borderColors,
                Fill = true,
                PointRadius = 3,
                BorderWidth = 1,
                PointBorderColor = Enumerable.Repeat(borderColors.First(), 6).ToList()
            };
        }

        protected BarChartDataset<double> GetBarChartDataset()
        {
            return new BarChartDataset<double>
            {
                Label = "# of Emails this Month",
                Data = RandomizeData2(),
                BackgroundColor = backgroundColors,
                BorderColor = borderColors,
                BorderWidth = 1
            };
        }



        protected List<double> RandomizeData()
        {
            return new List<double> { random.Next(1500, 2000), random.Next(1500, 2000), random.Next(1500, 2000), random.Next(1500, 2000), random.Next(1500, 2000), random.Next(1500, 2000), random.Next(1500, 2000), random.Next(1500, 2000), random.Next(1500, 2000), random.Next(1500, 2000), random.Next(1500, 2000), random.Next(1500, 2000) };
        }

        protected List<double> RandomizeData2()
        {
            return new List<double> { random.Next(30, 50), random.Next(30, 50), random.Next(30, 50), random.Next(30, 50), random.Next(30, 50), random.Next(30, 50), random.Next(30, 50), random.Next(30, 50), random.Next(30, 50), random.Next(30, 50), random.Next(30, 50), random.Next(30, 50) };
        }
    }
}
