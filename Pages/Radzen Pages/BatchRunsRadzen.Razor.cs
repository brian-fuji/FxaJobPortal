using Blazorise;
using FxaPortal.Models;
using FxaPortal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FxaPortal.Pages
{
    public class BatchRunsRadzenBase : ComponentBase
    {
        protected IEnumerable<BatchRun> batchRunList;
        protected int batchRunCount;
        protected string refreshIcon = string.Empty;
        protected LoadDataArgs lastArgs;
        protected bool showFileError = false;
        protected FileEdit fileUploadControl;
        protected List<IFileEntry> uploadFiles = new List<IFileEntry>();
        protected bool jobButtonDisabled = true;
        protected RadzenGrid<BatchRun> batchRunGrid;
        private bool firstRun = true;
        protected RadzenUpload upload;

        [Inject] BatchApiService bapi { get; set; }
        [Inject] FileUploadService fUpload { get; set; }
        [Inject] HttpClient client { get; set; }
        [Inject] NotificationService notifyService { get; set; }

        protected async Task LoadBatchRunData(LoadDataArgs args)
        {
            if (firstRun)
            {
                args.OrderBy = "RunDate desc";
                firstRun = false;
            }
            lastArgs = args;
            var result = await bapi.GetResults<BatchRun>(filter: args.Filter, top: args.Top, skip: args.Skip, orderby: args.OrderBy, expand: "Batch,BatchRunStatus", count: true, endMethod: "BatchRun");
            batchRunList = result.Value.AsODataEnumerable();
            batchRunCount = result.Count;

            StateHasChanged();
        }

        protected async Task RefreshData()
        {
            refreshIcon = "refresh";

            await LoadBatchRunData(lastArgs);

            refreshIcon = string.Empty;
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

        protected void RowRender(RowRenderEventArgs<BatchRun> args)
        {
            args.Expandable = args.Data.BatchRunStatus.Count > 0;
        }

        protected void OnFileChanged(FileChangedEventArgs e)
        {
            try
            {
                if (e.Files == null || e.Files.Count() == 0)
                {
                    var x = uploadFiles;
                    uploadFiles.Clear();
                }
                else
                {
                    uploadFiles.Clear();
                    foreach (var f in e.Files)
                    {
                        uploadFiles.Add(f);
                    }
                }
                jobButtonDisabled = uploadFiles.Count() == 0;
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

                        notifyService.Notify(NotificationSeverity.Success, "File Upload complete");

                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
        }
    }
}
