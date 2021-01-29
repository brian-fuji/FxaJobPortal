using Blazorise;
using Blazorise.DataGrid;
using Blazorise.Snackbar;
using FxaPortal.Models;
using FxaPortal.Services;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FxaPortal.Pages
{
    public class SchedulesBase : ComponentBase
    {
        // ReadData="@OnReadData" - To Read Data in Grid Above
        protected IList<Schedule> scheduleList;
        protected Schedule selectedSchedule;
        protected List<ScheduleDay> selectedDays = new List<ScheduleDay>();
        protected int totalSchedules;
        protected Snackbar snackbar;
        protected Snackbar snackbarFail;
        protected DateTime? scheduleStartDate = DateTime.Now;
        protected DateTime? scheduleEndDate = DateTime.Now.AddYears(5);
        protected string runType = "Print";
        protected Schedule lastResponse;
        protected int status;
        protected string ScheduleDays;
        protected List<int> expandedItems = new List<int>();
        protected IconName currentIconName;

        [Inject] BatchApiService bapi { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var args = new DataGridReadDataEventArgs<Schedule>(1, 30, null);
            await OnReadData(args);
        }

        protected async Task OnReadData(DataGridReadDataEventArgs<Schedule> e)
        {
            var response = await bapi.SimplePagedGet<ScheduleResponse>(e.Page, e.PageSize, "schedule");

            if (response != null)
                scheduleList = response.Body.Data;

            totalSchedules = response.Body.Paging.TotalRecordCount; // this is used to tell datagrid how many items are available so that pagination will work

            selectedSchedule = new Schedule();
            selectedSchedule.StartDate = DateTime.Today;
            selectedSchedule.StartDate = DateTime.Today.AddDays(30);
            selectedSchedule.ScheduleDays = new List<ScheduleDay>();

            // always call StateHasChanged!
            StateHasChanged();
        }

        protected void OnStartDateChanged(DateTime? dt)
        {
            if (dt.HasValue)
            {
                scheduleStartDate = dt;
                selectedSchedule.StartDate = dt.Value;
            }
        }

        protected void OnEndDateChanged(DateTime? dt)
        {
            if (dt.HasValue)
            {
                scheduleEndDate = dt;
                selectedSchedule.EndDate = dt.Value;
            }
        }

        protected void OnStatusChanged(int value)
        {
            selectedSchedule.Status = value;
        }

        protected void OnRowStyling(Schedule schedule, DataGridRowStyling styling)
        {
            if (schedule.Status != 1)
                styling.Style = "color: red;";
        }

        protected void OnDayRowStyling(ScheduleDay scheduleday, DataGridRowStyling styling)
        {
            styling.Background = Background.White;
        }

        protected void OnSelectedRowStyling(Schedule schedule, DataGridRowStyling styling)
        {
            styling.Background = Background.Secondary;
        }

        protected async Task OnRowInserting(CancellableRowChange<Schedule, Dictionary<string, object>> e)
        {
            try
            {                
                if (selectedSchedule != null)
                    Console.WriteLine(selectedSchedule.Name);

                var schedule = e.Item;
                schedule.Name = (string)e.Values["Name"];
                schedule.Description = (string)e.Values["Description"];
                schedule.Status = 1;
                schedule.RunType = runType;
                schedule.StartDate = scheduleStartDate.HasValue ? scheduleStartDate.Value : DateTime.Now;
                schedule.EndDate = scheduleEndDate.HasValue ? scheduleEndDate.Value : DateTime.Now;

                schedule.ScheduleDays = selectedDays;

                var response = await bapi.SimpleSave<Schedule>(e.Item, "schedule");

                if (response != null && response.IsSuccess)
                {
                    scheduleList.Add(response.Body);
                    snackbar.Show();
                    StateHasChanged();
                }
                else
                    snackbarFail.Show();

            }catch (Exception ex)
            {
                Console.WriteLine("***************************" + ex.Message);
            }
        }

        protected void OnRowClicked(DataGridRowMouseEventArgs<Schedule> e)
        {
            selectedSchedule = e.Item;

            selectedDays.Clear();

            foreach (var d in selectedSchedule.ScheduleDays)
            {
                d.IsSelected = true;
                selectedDays.Add(d);
            }
        }

        protected async void OnRowUpdating(CancellableRowChange<Schedule, Dictionary<string, object>> e)
        {
            var schedule = new Schedule();

            schedule.ScheduleId = selectedSchedule.ScheduleId;
            schedule.Name = (string)e.Values["Name"];
            schedule.Description = (string)e.Values["Description"];
            schedule.RunType = runType;
            schedule.StartDate = scheduleStartDate.Value;
            schedule.EndDate = scheduleEndDate.Value;
            schedule.Status = 1;

            schedule.ScheduleDays = selectedDays.Where(sd => sd.IsSelected == true).ToList();

            var x = selectedDays;
            var y = schedule;

            var response = await bapi.SimpleUpdate<Schedule>(schedule, "schedule", selectedSchedule.ScheduleId.ToString());

            if (response != null && response.IsSuccess)
            {
                var oldSchedule = scheduleList.FirstOrDefault(s => s.ScheduleId == schedule.ScheduleId);
                if (oldSchedule != null)
                    oldSchedule.UpdateFromSchedule(schedule);

                StateHasChanged();
                snackbar.Show();
            }
            else
            {
                snackbarFail.Show();
            }
        }

        protected void UpdateExpandedButton(int id)
        {
            if (expandedItems.Contains(id))
                expandedItems.Remove(id);
            else
                expandedItems.Add(id);
        }

        protected bool UpdateRowTrigger(int count, int id)
        {
            return count > 0 && expandedItems.Contains(id);
        }

        #region Day Change Events
        protected void MonChanged(bool value)
        {
            AddOrUpdateDay(1, value);
        }

        protected void TueChanged(bool value)
        {
            AddOrUpdateDay(2, value);
        }

        protected void WedChanged(bool value)
        {
            AddOrUpdateDay(3, value);
        }

        protected void ThuChanged(bool value)
        {
            AddOrUpdateDay(4, value);
        }

        protected void FriChanged(bool value)
        {
            AddOrUpdateDay(5, value);
        }

        protected void SatChanged(bool value)
        {
            AddOrUpdateDay(6, value);
        }

        protected void SunChanged(bool value)
        {
            AddOrUpdateDay(0, value);
        }
        #endregion

        #region Time Changed Events

        protected void MonStartChanged(TimeSpan? value)
        {
            UpdateTime(1, true, value);
        }

        protected void MonEndChanged(TimeSpan? value)
        {
            UpdateTime(1, false, value);
        }

        protected void TueStartChanged(TimeSpan? value)
        {
            UpdateTime(2, true, value);
        }

        protected void TueEndChanged(TimeSpan? value)
        {
            UpdateTime(2, false, value);
        }

        protected void WedStartChanged(TimeSpan? value)
        {
            UpdateTime(3, true, value);
        }

        protected void WedEndChanged(TimeSpan? value)
        {
            UpdateTime(3, false, value);
        }

        protected void ThuStartChanged(TimeSpan? value)
        {
            UpdateTime(4, true, value);
        }

        protected void ThuEndChanged(TimeSpan? value)
        {
            UpdateTime(4, false, value);
        }

        protected void FriStartChanged(TimeSpan? value)
        {
            UpdateTime(5, true, value);
        }

        protected void FriEndChanged(TimeSpan? value)
        {
            UpdateTime(5, false, value);
        }

        protected void SatStartChanged(TimeSpan? value)
        {
            UpdateTime(6, true, value);
        }

        protected void SatEndChanged(TimeSpan? value)
        {
            UpdateTime(6, false, value);
        }

        protected void SunStartChanged(TimeSpan? value)
        {
            UpdateTime(0, true, value);
        }

        protected void SunEndChanged(TimeSpan? value)
        {
            UpdateTime(0, false, value);
        }
        #endregion

        #region Schedule Day Helper Functions
        protected private bool CheckForDay(int num)
        {
            if (selectedDays == null || selectedDays.Count == 0)
                return false;

            return selectedDays.Exists(d => d.DayOfWeek == num && d.IsSelected);

        }

        protected void AddOrUpdateDay(int dayNum, bool value)
        {
            var day = selectedDays.FirstOrDefault(d => d.DayOfWeek == dayNum);

            if (day != null)
            {
                day.IsSelected = value;
            }
            else
            {
                selectedDays.Add(new ScheduleDay { ScheduleId = selectedSchedule.ScheduleId, DayOfWeek = dayNum, IsSelected = true });
            }

        }

        protected void UpdateTime(int dayNum, bool isStart, TimeSpan? value)
        {
            var day = selectedDays.FirstOrDefault(d => d.DayOfWeek == dayNum);

            if (day == null)
            {
                day = new ScheduleDay { ScheduleId = selectedSchedule.ScheduleId, DayOfWeek = dayNum, IsSelected = true };
                selectedDays.Add(day);
            }

            if (isStart)
                day.StartTime = value.Value.Hours * 60 + value.Value.Minutes;
            else
                day.EndTime = value.Value.Hours * 60 + value.Value.Minutes;
        }

        protected TimeSpan? GetStartEndTime(int dayNum, bool isStart)
        {
            if (selectedDays == null || selectedDays.Count == 0)
                return null;

            var day = selectedDays.FirstOrDefault(d => d.DayOfWeek == dayNum);

            if (day == null)
                return null;

            return isStart ? TimeSpan.FromMinutes(day.StartTime) : TimeSpan.FromMinutes(day.EndTime);
        }

        #endregion
    }
}
