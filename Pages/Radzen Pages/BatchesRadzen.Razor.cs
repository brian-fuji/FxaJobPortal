using FxaPortal.Models;
using FxaPortal.Pages.Radzen_Pages;
using FxaPortal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace FxaPortal.Pages
{
    public class BatchesRadzenBase : ComponentBase
    {
        protected IEnumerable<Batch> batchList;
        protected int batchCount;

        protected RadzenGrid<Batch> batchesGrid;

        [Inject] BatchApiService bapi { get; set; }

        [Inject]
        NotificationService notifyService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        protected async Task LoadBatchData(LoadDataArgs args)
        {
            var result = await bapi.GetResults<Batch>(filter: args.Filter, top: args.Top, skip: args.Skip, orderby: args.OrderBy, count: true, endMethod: "Batches");
            batchList = result.Value.AsODataEnumerable();
            batchCount = result.Count;

            StateHasChanged();
        }

        protected string GetShortDateLocal(DateTime? date)
        {
            if (date == null || date.HasValue == false)
                return string.Empty;

            var pattern = CultureInfo.CurrentCulture.DateTimeFormat;
            Console.WriteLine("Current culture = " + CultureInfo.CurrentCulture.Name);
            Console.WriteLine("Current UI culture = " + CultureInfo.CurrentUICulture.Name);
            string str = date.Value.ToShortDateString();
            Console.WriteLine("Short date string = {0}", pattern.ShortDatePattern);

            return str;
        }

        protected async Task BtnAddClick(MouseEventArgs args)
        {
            var dialogResult = await DialogService.OpenAsync<AddEditBatches>("Add Batch", null);

            if (dialogResult != null)
            {
                if (dialogResult.IsSuccess)
                {
                    await batchesGrid.Reload();

                    await InvokeAsync(() => { StateHasChanged(); });

                    notifyService.Notify(severity: NotificationSeverity.Success, summary: "Save was Successful");
                }
                else
                    notifyService.Notify(severity: NotificationSeverity.Error, summary: "Save Failed");
            }
        }

        protected async Task EditRow(Batch b)
        {
            var dialogResult = await DialogService.OpenAsync<AddEditBatches>("Edit Batch", new Dictionary<string, object>() { { "Batch", b } });

            if (dialogResult != null)
            {
                if (dialogResult.IsSuccess)
                {
                    await InvokeAsync(() => { StateHasChanged(); });
                    notifyService.Notify(new NotificationMessage { Severity = NotificationSeverity.Success, Summary = "Update Completed" });
                }
                else
                {
                    await batchesGrid.Reload();
                    notifyService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Update Failed" });
                }
            }
        }
    }
}
