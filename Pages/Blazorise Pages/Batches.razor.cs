using Blazorise;
using Blazorise.DataGrid;
using Blazorise.Snackbar;
using FxaPortal.Models;
using FxaPortal.Services;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace FxaPortal.Pages
{
    public class BatchesBase : ComponentBase
    {
        protected IList<Batch> batchList;
        protected Batch selectedBatch;
        protected int totalBatches;
        [Inject] BatchApiService bapi { get; set; }

        protected Snackbar snackbar;
        protected Snackbar snackbarFail;
        protected Snackbar snackbarInvalidAbbr;

        protected object currentId;

        #region Data Functions

        protected override async Task OnInitializedAsync()
        {
            var args = new DataGridReadDataEventArgs<Batch>(1, 30, null);
            await OnReadData(args);

            selectedBatch = new Batch();        
        }

        protected async Task OnReadData(DataGridReadDataEventArgs<Batch> e)
        {
            var response = await bapi.SimplePagedGet<BatchResponse>(e.Page, e.PageSize, "batches");

            if (response != null)
                batchList = response.Body.Data;

            totalBatches = response.Body.Paging.TotalRecordCount; // this is used to tell datagrid how many items are available so that pagination will work

            // always call StateHasChanged!
            StateHasChanged();
        }

        protected async Task OnRowInserting(CancellableRowChange<Batch, Dictionary<string, object>> e)
        {
            try
            {
                if (selectedBatch != null)
                    Console.WriteLine(selectedBatch.BatchAbbr);

                var batch = e.Item;
                batch.BatchBusName = (string)e.Values["BatchBusName"];
                batch.BatchAbbr = ((string)e.Values["BatchAbbr"]).ToUpper();

                if (batchList.Count(x => x.BatchAbbr == batch.BatchAbbr) != 0)
                {
                    snackbarInvalidAbbr.Show();
                    return;
                }

                var response = await bapi.SimpleSave<Batch>(e.Item, "batches");

                if (response != null && response.IsSuccess)
                {
                    batchList.Add(response.Body);
                    snackbar.Show();
                    StateHasChanged();
                }
                else
                    snackbarFail.Show();

            }
            catch (Exception ex)
            {
                Console.WriteLine("***************************" + ex.Message);
            }
        }

        #endregion

        protected void OnRowClicked(DataGridRowMouseEventArgs<Batch> e)
        {
            selectedBatch = e.Item;
        }

        protected void OnSelected(Batch b)
        {
            selectedBatch = b;
        }

        protected void OnSelectedRowChanged(Batch b)
        {
            var x = b;
        }

        protected void OnRowStyling(Batch batch, DataGridRowStyling styling)
        {
            // To Do - Need to have an Active Flag on the Batch
            //if (batch.Status != 1)
            //    styling.Style = "color: red;";
        }

        protected void OnSelectedRowStyling(Batch schedule, DataGridRowStyling styling)
        {
            styling.Background = Background.Secondary;
        }

        protected async void OnRowUpdating(CancellableRowChange<Batch, Dictionary<string, object>> e)
        {
            var batch = new Batch(selectedBatch.BatchId, e.Values);

            var response = await bapi.SimpleUpdate<Batch>(batch, "batches", selectedBatch.BatchId.ToString());

            if (response != null && response.IsSuccess)
            {
                var origBatch = batchList.FirstOrDefault(b => b.BatchId == batch.BatchId);
                origBatch.UpdateFromBatch(batch);                
                StateHasChanged();
                snackbar.Show();
            }
            else
            {
                snackbarFail.Show();
            }
        }

        protected void OnValidateAbbr(ValidatorEventArgs args)
        {
            var abbr = Convert.ToString(args.Value).ToUpper();

            if (string.IsNullOrEmpty(abbr))
            {
                args.Status = ValidationStatus.Error;
                args.ErrorText = "This is a required field";
                return;
            }
                
            var batch = batchList.FirstOrDefault(b => b.BatchAbbr == abbr);

            var x = selectedBatch;

            if (batch == null)
                args.Status = ValidationStatus.Success;
            else
            {
                if (selectedBatch != null)
                {
                    if (selectedBatch.BatchId == batch.BatchId)
                        args.Status = ValidationStatus.Success;
                    else
                        args.Status = ValidationStatus.Error;
                }
            }
        }
    }
}
