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
    public class EmailTransactionsBase : ComponentBase
    {
        protected IList<EmailTransaction> emailTransactionList;
        protected EmailTransaction selectedEmailTransaction;
        protected int totalEmailTransactions;
        protected int currentPage;
        protected string metaData;
        protected Snackbar snackbar;
        protected Snackbar snackbarFail;
        protected Snackbar snackbarReadFail;
        protected Snackbar snackbarXmlWarn;
        protected Snackbar snackbarJsonWarn;
        protected string currentMetaData;
        protected string snackText;

        [Inject] BatchApiService bapi { get; set; }


        protected void OnSelectedRowStyling(EmailTransaction emailtransaction, DataGridRowStyling styling)
        {
            styling.Background = Background.Secondary;
        }

        protected async Task OnReadData(DataGridReadDataEventArgs<EmailTransaction> e)
        {
            var response = await bapi.SimplePagedGet<EmailTransactionResponse>(e.Page, e.PageSize, "emailtransaction");

            if (response == null)
            {
                snackbarReadFail.Show();
                return;
            }
            emailTransactionList = response.Body.Data;

            totalEmailTransactions = response.Body.Paging.TotalRecordCount; // this is used to tell datagrid how many items are available so that pagination will work

            // always call StateHasChanged!
            StateHasChanged();
        }

        protected void OnRowClicked(DataGridRowMouseEventArgs<EmailTransaction> transaction)
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
