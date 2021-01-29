using Blazorise;
using Blazorise.DataGrid;
using Blazorise.Snackbar;
using FxaPortal.Models;
using FxaPortal.Services;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Radzen;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FxaPortal.Pages
{
    public class SmsTransactionRadzenBase : ComponentBase
    {
        [Inject] BatchApiService bapi { get; set; }


        protected int count;
        protected IEnumerable<SmsTransaction> smsList;
        protected string refreshIcon = string.Empty;
        protected LoadDataArgs lastArgs;
        protected bool firstLoad = true;

        protected async Task LoadData(LoadDataArgs args)
        {
            if (firstLoad)
            {
                args.OrderBy = "TransactionDate desc";
                firstLoad = false;
            }

            lastArgs = args;
            var result = await bapi.GetResults<SmsTransaction>(filter: args.Filter, top: args.Top, skip: args.Skip, orderby: args.OrderBy, count: true, endMethod: "smstransaction");
            smsList = result.Value.AsODataEnumerable();
            count = result.Count;

            StateHasChanged();
        }

        protected async Task RefreshData()
        {
            refreshIcon = "refresh";

            await LoadData(lastArgs);

            refreshIcon = string.Empty;
        }

    }
}
