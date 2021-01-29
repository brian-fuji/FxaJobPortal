using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using FxaPortal.Models;
using FxaPortal.Pages;
using System.ComponentModel;
using FxaPortal.Services;

namespace FxaPortal.Pages
{
    public class AddEditScheduleBase : ComponentBase
    {
        [Parameter(CaptureUnmatchedValues = true)]
        public IReadOnlyDictionary<string, dynamic> Attributes { get; set; }

        [Parameter]
        public Schedule Schedule { get; set; }

        public void Reload()
        {
            InvokeAsync(StateHasChanged);
        }

        public void OnPropertyChanged(PropertyChangedEventArgs args)
        {
        }

        protected string[] runTypeData = new string[] { "Print", "Email", "Sms", "Other" };

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager UriHelper { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected BatchApiService bapi { get; set; }

        protected IList<ScheduleDay> ScheduleDays;

        protected override async Task OnInitializedAsync()
        {
            CreateScheduleDays();
            await Task.Run(() => Load());
        }
        protected void Load()
        {
            if (Schedule == null)
                Schedule = new Schedule() { };
            else
            {
                foreach (var day in Schedule.ScheduleDays)
                {
                    var currDay = ScheduleDays.FirstOrDefault(d => d.DayOfWeek == day.DayOfWeek);
                    if (currDay != null)
                    {
                        currDay.UpdateFromScheduleDay(day);
                        currDay.IsSelected = true;
                    }
                }
                StateHasChanged();
            };
        }

        private void CreateScheduleDays()
        {
            ScheduleDays = new List<ScheduleDay>();
            for (int i = 0; i <= 6; i++)
            {
                ScheduleDays.Add(new ScheduleDay { DayOfWeek = i });
            }
        }

        protected void IsSelected(bool? value, int day)
        {
            var dayFound = ScheduleDays.FirstOrDefault(d => d.DayOfWeek == day);
            dayFound.IsSelected = value.Value;
        }

        protected void TimeChanged(DateTime? value, int day, bool isStart)
        {
            if (!value.HasValue)
            {
                value = new DateTime(0);
            }

            var dayFound = ScheduleDays.FirstOrDefault(d => d.DayOfWeek == day);
            if (dayFound != null)
            {
                if (isStart)
                    dayFound.StartTime = value.Value.TimeOfDay.Hours * 60 + value.Value.TimeOfDay.Minutes;
                else
                    dayFound.EndTime = value.Value.TimeOfDay.Hours * 60 + value.Value.TimeOfDay.Minutes;
            }
        }

        protected bool ValidateScheduleDates()
        {
            var x = Schedule.EndDate < Schedule.StartDate;
            return x;
        }

        protected async Task Form0Submit(Schedule schedule)
        {
            try
            {
                schedule.ScheduleDays = ScheduleDays.Where(x => x.IsSelected == true).ToList();
                if (Schedule.ScheduleId == 0)
                {
                    var result = await bapi.SimpleSave<Schedule>(schedule, "schedule");
                    DialogService.Close(result);
                }
                else
                {
                    var result = await bapi.SimpleUpdate<Schedule>(schedule, "schedule", schedule.ScheduleId.ToString());
                    DialogService.Close(result);
                }


            }
            catch (Exception)
            {
                NotificationService.Notify(NotificationSeverity.Error, $"Error", $"Unable to create new Schedule!");
            }
        }

        protected async Task Button2Click(MouseEventArgs args)
        {
            await Task.Run(() => DialogService.Close(null));
        }

    }

}

