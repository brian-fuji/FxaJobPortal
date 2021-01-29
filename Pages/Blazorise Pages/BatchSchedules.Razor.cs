using Blazorise;
using Blazorise.DataGrid;
using Blazorise.Snackbar;
using FxaPortal.Models;
using FxaPortal.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FxaPortal.Pages
{
    public class BatchSchedulesBase : ComponentBase
    {
        // ReadData="@OnReadData" - To Read Data in Grid Above
        protected IList<Batch> batchList;
        protected IList<Schedule> scheduleList;
        protected Batch selectedBatch;
        protected Schedule selectedSchedule;
        protected IList<BatchSchedule> batchScheduleList;
        protected BatchSchedule selectedBatchSchedule;
        protected int totalBatchSchedules;
        protected Snackbar snackbar;
        protected Snackbar snackbarFail;
        protected Snackbar snackbarNoBatch;
        protected List<int> expandedItems = new List<int>();
        protected bool isSaving = false;

        [Inject] BatchApiService bapi { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await ReadBatchData();
            await ReadScheduleData();
            await ReadBatchScheduleData();

            StateHasChanged();
        }

        async Task ReadBatchData()
        {
            var response = await bapi.SimplePagedGet<BatchResponse>(1, 500, "batches");

            if (response != null)
                batchList = response.Body.Data;
        }

        async Task ReadScheduleData()
        {
            var response = await bapi.SimplePagedGet<ScheduleResponse>(1, 1000, "schedule");

            if (response != null)
                scheduleList = response.Body.Data;
        }


        async Task ReadBatchScheduleData()
        {
            var response = await bapi.SimplePagedGet<BatchScheduleResponse>(1, 1000, "batchschedule");

            if (response != null)
                batchScheduleList = response.Body.Data;
        }

        protected void OnRowClicked(DataGridRowMouseEventArgs<Batch> e)
        {
            selectedBatch = e.Item;

            UpdateScheduleList();

            StateHasChanged();

        }

        protected bool UpdateRowTrigger(int count, int id)
        {
            return count > 0 && expandedItems.Contains(id);
        }

        protected void UpdateExpandedButton(int id)
        {
            if (expandedItems.Contains(id))
                expandedItems.Remove(id);
            else
                expandedItems.Add(id);
        }

        private void UpdateScheduleList()
        {
            var newList = batchScheduleList.Where(bs => bs.BatchId == selectedBatch.BatchId).Select(bs1 => bs1.ScheduleId).ToList();

            //scheduleList.ForEach(s => s.IsSelected = newList.Contains(s.ScheduleId));

            StateHasChanged();
        }

        protected void OnScheduleRowClicked(DataGridRowMouseEventArgs<Schedule> e)
        {
            selectedSchedule = e.Item;
        }

        protected async void OnScheduleCheckChanged(bool value)
        {
            isSaving = true;
            if (selectedBatch == null)
            {
                snackbarNoBatch.Show();
                return;
            }
            // If it has been selected we need to add it to the list
            if (value == true)
            {
                var batchSchdeule = new BatchSchedule { BatchId = selectedBatch.BatchId, ScheduleId = selectedSchedule.ScheduleId };
                await DoSave(batchSchdeule);
            }
            else
            {
                var batchSchedule = batchScheduleList.FirstOrDefault(bs => bs.BatchId == selectedBatch.BatchId && bs.ScheduleId == selectedSchedule.ScheduleId);

                await DoDelete(batchSchedule);
            }
            isSaving = false;
            UpdateScheduleList();
        }

        protected void OnSelectedBatchRowStyling(Batch batch, DataGridRowStyling styling)
        {
            styling.Background = Background.Secondary;
        }

        protected void OnSelectedScheduleRowStyling(Schedule schedule, DataGridRowStyling styling)
        {
            styling.Background = Background.Secondary;
        }

        protected void OnDayRowStyling(ScheduleDay scheduleday, DataGridRowStyling styling)
        {
            styling.Background = Background.White;
        }

        // This is a dynamic save that happens each time you make a schedule selection
        protected async Task DoSave(BatchSchedule batchSchedule)
        {
            var response = await bapi.SimpleSave<BatchSchedule>(batchSchedule, "batchschedule");

            if (response != null && response.IsSuccess)
            {
                batchSchedule.BatchScheduleId = response.Body.BatchScheduleId;
                batchScheduleList.Add(batchSchedule);
                snackbar.Show();
            }
            else
            {
                snackbarFail.Show();
                batchScheduleList.Remove(batchSchedule);
            }
        }

        protected async Task DoDelete(BatchSchedule batchSchedule)
        {
            var response = await bapi.SimpleDelete("batchschedule", batchSchedule.BatchScheduleId.ToString());

            if (response != null && response.IsSuccess)
            {
                batchScheduleList.Remove(batchSchedule);
                snackbar.Show();
            }
            else
            {
                snackbarFail.Show();
            }
        }
    }
}
