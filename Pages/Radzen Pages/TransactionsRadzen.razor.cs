using Blazorise;
using Blazorise.DataGrid;
using Blazorise.Snackbar;
using FxaPortal.Helpers;
using FxaPortal.Models;
using FxaPortal.Services;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Radzen;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FxaPortal.Pages
{
    public class TransactionsRadzenBase : ComponentBase
    {
        [Inject] BatchApiService bapi { get; set; }

        [Inject] NotificationService notifyService { get; set; }
        protected int count;
        protected IEnumerable<Transaction> transactionList;

        protected Transaction selectedTransaction;
        protected string currentMetaData;
        protected LoadDataArgs lastArgs;
        protected string refreshIcon = string.Empty;
        protected bool firstLoad = true;

        // We are leaving out the metadata column for the load as these can be quite big
        // The metadata can be loaded separately to be viewed.
        protected async Task LoadData(LoadDataArgs args)
        {
            if (firstLoad)
            {
                args.OrderBy = "TransactionDate desc";
                firstLoad = false;
            }

            lastArgs = args;
            var result = await bapi.GetResults<Transaction>(filter: args.Filter, 
                top: args.Top, 
                skip: args.Skip, 
                orderby: args.OrderBy, 
                count: true, 
                select: "DocumentId,BatchRunNUmber,BatchAbbr,TransactionType,IssueDate,PolicyNumber,CoverType,NoticeType,Status,TransactionDate",
                endMethod: "transaction");
            transactionList = result.Value.AsODataEnumerable();
            count = result.Count;

            StateHasChanged();
        }

        protected async Task RefreshData()
        {
            refreshIcon = "refresh";

            await LoadData(lastArgs);

            refreshIcon = string.Empty;
        }

        protected void OnRowSelected(Transaction tr)
        {
            selectedTransaction = tr;
        }

        protected async Task ViewMetadataClicked()
        {
            if (selectedTransaction != null)
            {
                var response = await bapi.SimpleGet<string>($"transaction/metadata/{selectedTransaction.DocumentId}");

                if (response != null && response.Body != null)
                {
                    if (response.Body.StartsWith("<"))
                    {
                        try
                        {
                            currentMetaData = StaticHelpers.PrintXML(response.Body);
                        }
                        catch (Exception)
                        {
                            currentMetaData = response.Body;
                            notifyService.Notify(NotificationSeverity.Warning, "The Metadata is not valid XML");
                        }
                    }
                    else if (response.Body.StartsWith("[") | response.Body.StartsWith("{"))
                    {
                        try
                        {
                            currentMetaData = JToken.Parse(response.Body).ToString();
                        }
                        catch (Exception)
                        {
                            currentMetaData = response.Body;
                            notifyService.Notify(NotificationSeverity.Warning, "The Metadata is not valid JSON");
                        }
                    }
                    else
                        currentMetaData = response.Body;
                }
                else
                {
                    notifyService.Notify(NotificationSeverity.Warning, "There was a problem reading the Metadata");
                }
            }
        }

    }
}
