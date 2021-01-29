using Blazorise;
using Blazorise.DataGrid;
using Blazorise.Snackbar;
using FxaPortal.Helpers;
using FxaPortal.Models;
using FxaPortal.Services;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FxaPortal.Pages
{
    public class TransactionsBase : ComponentBase
    {
        protected IList<Transaction> transactionList;
        protected Transaction selectedTransaction;
        protected int totalTransactions;
        protected int currentPage;
        protected string metaData;
        protected Snackbar snackbar;
        protected Snackbar snackbarFail;
        protected Snackbar snackbarReadFail;
        protected Snackbar snackbarXmlWarn;
        protected Snackbar snackbarJsonWarn;
        protected string currentMetaData;
        protected string snackText;
        protected bool isLoading;

        [Inject] BatchApiService bapi { get; set; }


        protected void OnSelectedRowStyling(Transaction transaction, DataGridRowStyling styling)
        {
            styling.Background = Background.Secondary;
        }

        protected async Task OnReadData(DataGridReadDataEventArgs<Transaction> e)
        {
            var response = await bapi.SimplePagedGet<TransactionResponse>(e.Page, e.PageSize, "transactions/getlite");

            if (response == null)
            {
                snackbarReadFail.Show();
                return;
            }
            transactionList = response.Body.Data;

            totalTransactions = response.Body.Paging.TotalRecordCount; // this is used to tell datagrid how many items are available so that pagination will work

            // always call StateHasChanged!
            StateHasChanged();
        }

        protected async Task OnRefreshData()
        {
            isLoading = true;

            var response = await bapi.SimplePagedGet<TransactionResponse>(1, 30, "transactions/getlite");

            if (response == null)
            {
                snackbarReadFail.Show();
                return;
            }
            transactionList = response.Body.Data;

            totalTransactions = response.Body.Paging.TotalRecordCount; // this is used to tell datagrid how many items are available so that pagination will work

            // always call StateHasChanged!
            StateHasChanged();

            isLoading = false;
        }

        protected void OnRowClicked(DataGridRowMouseEventArgs<Transaction> transaction)
        {
            currentMetaData = string.Empty;
        }

        protected void OnPageChanged(DataGridPageChangedEventArgs e)
        {
            var x = e;
        }

        protected async Task OnViewMetadataClicked()
        {
            if (selectedTransaction != null)
            {
                var response = await bapi.SimpleGet<string>($"transactions/metadata/{selectedTransaction.DocumentId}");

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
                            snackbarXmlWarn.Show();
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
                            snackbarJsonWarn.Show();
                        }
                    } else
                        currentMetaData = response.Body;
                }
                else
                {
                    snackbarReadFail.Show();
                }
            }
        }
    }
}
