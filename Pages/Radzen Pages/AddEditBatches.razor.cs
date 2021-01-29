using FxaPortal.Models;
using FxaPortal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FxaPortal.Pages
{
    public class AddEditBatchesBase : ComponentBase
    {
        [Parameter(CaptureUnmatchedValues = true)]
        public IReadOnlyDictionary<string, dynamic> Attributes { get; set; }

        [Parameter]
        public Batch Batch { get; set; }

        public void Reload()
        {
            InvokeAsync(StateHasChanged);
        }

        public void OnPropertyChanged(PropertyChangedEventArgs args)
        {
        }

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

        protected override async Task OnInitializedAsync()
        {
            await Task.Run( () => Load());
        }
        protected void Load()
        {
            if (Batch == null)
                Batch = new Batch() { };
        }

        protected async Task Form0Submit(Batch batch)
        {
            try
            {
                if (Batch.BatchId == 0)
                {
                    var result = await bapi.SimpleSave<Batch>(batch, "batches");
                    DialogService.Close(result);
                }
                else
                {
                    var result = await bapi.SimpleUpdate<Batch>(batch, "batches", batch.BatchId.ToString());
                    DialogService.Close(result);
                }


            }
            catch (Exception)
            {
                NotificationService.Notify(NotificationSeverity.Error, $"Error", $"Unable to create new Batch!");
            }
        }

        protected async Task Button2Click(MouseEventArgs args)
        {
            await Task.Run(() => DialogService.Close(null));
        }
    }
}
