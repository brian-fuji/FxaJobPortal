using Blazorise.DataGrid;
using FxaPortal.Models;
using FxaPortal.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace FxaPortal.Pages
{
    public class ScheduleDaysBase : ComponentBase
    {
        // ReadData="@OnReadData" - To Read Data in Grid Above
        protected IList<ScheduleDay> scheduleDayList;
        protected ScheduleDay selectedscheduleDay;
        protected int totalScheduleDays;
        protected string customFilterValue;

        [Inject] BatchApiService bapi { get; set; }
        protected override async Task OnInitializedAsync()
        {
            var args = new DataGridReadDataEventArgs<ScheduleDay>(1, 30, null);
            await OnReadData(args);
        }

        protected async Task OnReadData(DataGridReadDataEventArgs<ScheduleDay> e)
        {
            var response = await bapi.SimplePagedGet<ScheduleDayResponse>(e.Page, e.PageSize, "scheduledays");

            if (response != null)
                scheduleDayList = response.Body.Data;

            totalScheduleDays = response.Body.Paging.TotalRecordCount; // this is used to tell datagrid how many items are available so that pagination will work


            // always call StateHasChanged!
            StateHasChanged();
        }

        protected bool OnCustomFilter(ScheduleDay model)
        {
            // We want to accept empty value as valid or otherwise
            // datagrid will not show anything.
            if (string.IsNullOrEmpty(customFilterValue))
                return true;

            return
                model.Schedule.Name.Contains(customFilterValue);
        }
    }
}
