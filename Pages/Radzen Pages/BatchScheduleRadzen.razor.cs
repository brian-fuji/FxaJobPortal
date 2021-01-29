using Blazorise;
using Blazorise.DataGrid;
using Blazorise.Snackbar;
using FxaPortal.Models;
using FxaPortal.Services;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Radzen;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FxaPortal.Pages
{
    public class BatchScheduleRadzenBase : ComponentBase
    {
        [Inject] BatchApiService bapi { get; set; }

        [Inject] NotificationService notifyService { get; set; }

        protected int batchCount;
        protected int batchScheduleCount;
        protected int scheduleCount;
        protected IList<Batch> batchList;
        protected IList<BatchSchedule> batchScheduleList;
        protected IList<Schedule> scheduleList;

        protected bool isSaving = false;
        protected Batch selectedBatch;
        protected Schedule selectedSchedule;

        protected async Task LoadBatchData(LoadDataArgs args)
        {
            var result = await bapi.GetResults<Batch>(filter: args.Filter, top: args.Top, skip: args.Skip, orderby: args.OrderBy, count: true, endMethod: "batches");
            batchList = result.Value.AsODataEnumerable().ToList();
            batchCount = result.Count;

            await LoadBatchScheduleData();

            StateHasChanged();
        }

        protected async Task LoadBatchScheduleData()
        {
            var result = await bapi.GetResults<BatchSchedule>("batchschedule");
            batchScheduleList = result.Value.AsODataEnumerable().ToList();

            StateHasChanged();
        }

        protected async Task LoadScheduleData(LoadDataArgs args)
        {
            var result = await bapi.GetResults<Schedule>(filter: args.Filter, top: args.Top, skip: args.Skip, orderby: args.OrderBy, expand: "ScheduleDays($orderby=DayOfWeek)", count: true, endMethod: "Schedule");
            scheduleList = result.Value.AsODataEnumerable().ToList();
            scheduleCount = result.Count;

            StateHasChanged();
        }

        protected void RowRender(RowRenderEventArgs<Schedule> args)
        {
            args.Expandable = args.Data.ScheduleDays.Count > 0;
        }

        protected void RowSelected(Batch batch)
        {
            selectedBatch = batch;

            UpdateScheduleList();
            
        }

        protected void OnScheduleSelected(Schedule s)
        {
            selectedSchedule = s;
        }

        private void UpdateScheduleList()
        {
            var newList = batchScheduleList.Where(bs => bs.BatchId == selectedBatch.BatchId).Select(bs1 => bs1.ScheduleId).ToList();

            scheduleList.ToList().ForEach(s => s.IsSelected = newList.Contains(s.ScheduleId));

            StateHasChanged();
        }

        protected async void OnScheduleCheckChanged(bool? value, Schedule s)
        {
            selectedSchedule = s;
            isSaving = true;
            if (selectedBatch == null)
            {
                notifyService.Notify(severity: NotificationSeverity.Warning, "No Batch Selected");
                isSaving = false;
                return;
            }
            // If it has been selected we need to add it to the list
            if (value == true)
            {
                var batchSchdeule = new BatchSchedule { BatchId = selectedBatch.BatchId, ScheduleId = s.ScheduleId };
                await DoSave(batchSchdeule);
            }
            else
            {
                var batchSchedule = batchScheduleList.FirstOrDefault(bs => bs.BatchId == selectedBatch.BatchId && bs.ScheduleId == s.ScheduleId);

                await DoDelete(batchSchedule);
            }

            isSaving = false;

            UpdateScheduleList();
        }

        // This is a dynamic save that happens each time you make a schedule selection
        protected async Task DoSave(BatchSchedule batchSchedule)
        {
            var response = await bapi.SimpleSave<BatchSchedule>(batchSchedule, "batchschedule");

            if (response != null && response.IsSuccess)
            {
                batchSchedule.BatchScheduleId = response.Body.BatchScheduleId;
                batchScheduleList.Add(batchSchedule);
                notifyService.Notify(severity: NotificationSeverity.Success, summary: "Saved", duration: 500);
            }
            else
            {
                notifyService.Notify(severity: NotificationSeverity.Error, summary: "Save Failed", duration: 1000);
                batchScheduleList.Remove(batchSchedule);
            }

        }

        protected async Task DoDelete(BatchSchedule batchSchedule)
        {
            var response = await bapi.SimpleDelete("batchschedule", batchSchedule.BatchScheduleId.ToString());

            if (response != null && response.IsSuccess)
            {
                batchScheduleList.Remove(batchSchedule);
                UpdateScheduleList();
                notifyService.Notify(severity: NotificationSeverity.Success, summary:"Saved", duration:500);
            }
            else
            {
                notifyService.Notify(severity: NotificationSeverity.Error, summary: "Save Failed", duration: 1000);
            }

        }

    }
}
