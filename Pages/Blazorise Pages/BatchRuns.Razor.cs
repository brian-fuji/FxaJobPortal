using Blazorise;
using Blazorise.DataGrid;
using Blazorise.Snackbar;
using FxaPortal.Models;
using FxaPortal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FxaPortal.Pages
{
    public class BatchRunsBase : ComponentBase
    {
        protected IList<BatchRun> batchRunList;
        protected BatchRun selectedBatchRun;
        protected int totalBatchRuns;
        protected int currentPage;
        protected bool showFileError = false;
        protected List<IFileEntry> uploadFiles = new List<IFileEntry>();
        protected Snackbar snackBarUploadComplete;
        protected FileEdit fileUploadControl;
        protected List<int> expandedItems = new List<int>();
        protected IconName currentIconName;
        protected int currentPageSize;
        protected bool jobButtonDisabled = true;
        protected bool isLoading = false;

        [Inject] BatchApiService bapi { get; set; }
        [Inject] FileUploadService fUpload { get; set; }
        [Inject] HttpClient client { get; set; }
        [Inject] IJSRuntime JsRuntime { get; set; }

        protected void OnSelectedRowStyling(BatchRun batchRun, DataGridRowStyling styling)
        {
            styling.Background = Background.Secondary;
        }

        protected async Task OnReadData(DataGridReadDataEventArgs<BatchRun> e)
        {
            currentPageSize = e.PageSize;
            currentPage = e.Page;

            var qryStuff = JsonConvert.SerializeObject(e.Columns);

            var response = await bapi.SimplePagedGet<BatchRunResponse>(e.Page, e.PageSize, "batchrun");

            if (response != null)
                batchRunList = response.Body.Data;

            totalBatchRuns = response.Body.Paging.TotalRecordCount; // this is used to tell datagrid how many items are available so that pagination will work

            // always call StateHasChanged!
            StateHasChanged();
        }

        protected async Task OnRefreshData()
        {
            isLoading = true;
            var response = await bapi.SimplePagedGet<BatchRunResponse>(currentPage, currentPageSize, "batchrun");

            if (response != null)
                batchRunList = response.Body.Data;

            var x = batchRunList;

            totalBatchRuns = response.Body.Paging.TotalRecordCount; // this is used to tell datagrid how many items are available so that pagination will work

            isLoading = false;

            // always call StateHasChanged!
            StateHasChanged();
        }

        protected void OnRowClicked(DataGridRowMouseEventArgs<BatchRun> batchRun)
        {

        }

        protected void OnPageChanged(DataGridPageChangedEventArgs e)
        {
            currentPage = e.Page;
        }

        protected void OnFileChanged(FileChangedEventArgs e)
        {
            try
            {
                foreach (var f in e.Files)
                {
                    uploadFiles.Add(f);
                }
                jobButtonDisabled = (e.Files.Count() == 0);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            showFileError = false;
        }

        protected void OnCreateClicked()
        {
            if (uploadFiles == null || uploadFiles.Count == 0)
            {
                showFileError = true;
            }
            else
            {
                showFileError = false;
                UploadFiles();
            }

            fileUploadControl.NotifyChange(new FileEntry[] { });
        }

        private async void UploadFiles()
        {
            try
            {
                foreach (var file in uploadFiles)
                {
                    // A stream is going to be the destination stream we're writing to.                
                    using (var stream = new MemoryStream())
                    {
                        // Here we're telling the FileEdit where to write the upload result
                        await file.WriteToStreamAsync(stream);

                        var content = new MultipartFormDataContent {
                            { new ByteArrayContent(stream.GetBuffer()), "\"upload\"", file.Name }
                        };

                        await fUpload.UploadAsync(client, content, "upload");

                        snackBarUploadComplete.Show();
                        
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
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

        protected string SetStyle(BatchRun br, string value)
        {
            return "white-space:nowrap;max-width:80px";
        }
    }
}
