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
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FxaPortal.Pages
{
    public class SmsTransactionsBase : ComponentBase
    {
        protected IList<SmsTransaction> smsTransactionList;
        protected SmsTransaction selectedSmsTransaction;
        protected int totalSmsTransactions;
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


        protected void OnSelectedRowStyling(SmsTransaction smstransaction, DataGridRowStyling styling)
        {
            styling.Background = Background.Secondary;
        }

        protected async Task OnReadData(DataGridReadDataEventArgs<SmsTransaction> e)
        {
            var response = await bapi.SimplePagedGet<SmsTransactionResponse>(e.Page, e.PageSize, "smstransaction");

            if (response == null)
            {
                snackbarReadFail.Show();
                return;
            }
            smsTransactionList = response.Body.Data;

            totalSmsTransactions = response.Body.Paging.TotalRecordCount; // this is used to tell datagrid how many items are available so that pagination will work

            // always call StateHasChanged!
            StateHasChanged();
        }

        protected async Task OnRefreshData()
        {
            isLoading = true;
            var response = await bapi.SimplePagedGet<SmsTransactionResponse>(1, 30, "smstransaction");

            if (response != null)
                smsTransactionList = response.Body.Data;


            totalSmsTransactions = response.Body.Paging.TotalRecordCount; // this is used to tell datagrid how many items are available so that pagination will work

            isLoading = false;

            // always call StateHasChanged!
            StateHasChanged();
        }

        protected void OnRowClicked(DataGridRowMouseEventArgs<SmsTransaction> transaction)
        {
            currentMetaData = string.Empty;
        }

        protected void OnPageChanged(DataGridPageChangedEventArgs e)
        {
            var x = e;
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
    }
}
