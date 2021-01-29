using Blazorise;
using Blazorise.DataGrid;
using Blazorise.Snackbar;
using FxaPortal.Models;
using FxaPortal.Pages.Radzen_Pages;
using FxaPortal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Radzen;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FxaPortal.Pages
{
    public class SchedulesRadzenBase : ComponentBase
    {
        // ReadData="@OnReadData" - To Read Data in Grid Above
        protected RadzenGrid<Schedule> schedulesGrid;
        protected IList<Schedule> scheduleList;
        protected Schedule selectedSchedule;
        protected List<ScheduleDay> selectedDays = new List<ScheduleDay>();
        protected int totalSchedules;
        protected DateTime? scheduleStartDate = DateTime.Now;
        protected DateTime? scheduleEndDate = DateTime.Now.AddYears(5);
        protected string runType = "Print";
        protected int status;
        protected string ScheduleDays;
        protected IconName currentIconName;

        [Inject] 
        BatchApiService bapi { get; set; }

        [Inject]
        NotificationService notifyService { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        //[Inject]
        //protected NavigationManager UriHelper { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        protected int count;
        protected IEnumerable<Schedule> schedules;

        protected async Task LoadData(LoadDataArgs args)
        {
            var result = await bapi.GetResults<Schedule>(filter: args.Filter, top: args.Top, skip: args.Skip, orderby: args.OrderBy, expand: "ScheduleDays($orderby=DayOfWeek)", count: true, endMethod: "Schedule");
            schedules = result.Value.AsODataEnumerable();
            count = result.Count;

            StateHasChanged();
        }

        protected async Task BtnAddClick(MouseEventArgs args)
        {
            var dialogResult = await DialogService.OpenAsync<AddEditSchedule>("Add Schedule", null);

            if (dialogResult != null)
            {
                if (dialogResult.IsSuccess)
                {
                    await schedulesGrid.Reload();

                    await InvokeAsync(() => { StateHasChanged(); });

                    notifyService.Notify(severity: NotificationSeverity.Success, summary: "Save was Successful");
                }
                else
                    notifyService.Notify(severity: NotificationSeverity.Error, summary: "Save Failed");
            }
        }

        protected void RowRender(RowRenderEventArgs<Schedule> args)
        {
            args.Expandable = args.Data.ScheduleDays.Count > 0;
        }

        protected async Task EditRow(Schedule s)
        {
            var dialogResult = await DialogService.OpenAsync<AddEditSchedule>("Edit Schedule", new Dictionary<string, object>() { { "Schedule", s } });

            if (dialogResult != null)
            {
                if (dialogResult.IsSuccess)
                {
                    await InvokeAsync(() => { StateHasChanged(); });
                    notifyService.Notify(new NotificationMessage {Severity = NotificationSeverity.Success, Summary = "Update Completed" });
                }
                else
                {
                    notifyService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Update Failed" });
                }
            }
        }

        protected void SaveRow(Schedule s)
        {
            schedulesGrid.UpdateRow(s);
        }

        protected void CancelEdit(Schedule s)
        {
            schedulesGrid.CancelEditRow(s);
        }

    }
}
