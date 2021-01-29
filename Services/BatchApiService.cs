using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using FxaPortal.Models;
using Microsoft.Extensions.Configuration;
using Radzen;
using RestClient.Net;
using RestClient.Net.Abstractions;
using RestClient.Net.Abstractions.Extensions;

namespace FxaPortal.Services
{
    public partial class BatchApiService : IApiService
    {
        private IConfiguration config;
        private string baseApiUri;
        private Uri testUri;
        private HttpClient httpClient;

        public BatchApiService(IConfiguration configuration)
        {
            config = configuration;
            baseApiUri = config["apiURL"];
            testUri = new Uri(config["apiURL"]);
            this.httpClient = new HttpClient();
        }

        public async Task<Response<T>> SimplePagedGet<T>(int page, int size, string methodCall)
        {
            string call = $"{baseApiUri}{methodCall}";
            try
            {
                var client = new Client(new NewtonsoftSerializationAdapter(), baseUri: new Uri($"{call}?Page={page}&Size={size}"));
                client.ThrowExceptionOnFailure = false;
                var response = await client.GetAsync<T>();
                if (response.IsSuccess)
                    return response;
                else
                {
                    return default;
                }
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<Response<T>> SimplePagedGetWithColumns<T>(int page, int size, string methodCall)
        {
            string call = $"{baseApiUri}{methodCall}";
            try
            {
                var client = new Client(new NewtonsoftSerializationAdapter(), baseUri: new Uri($"{call}?Page={page}&Size={size}"));
                client.ThrowExceptionOnFailure = false;
                var response = await client.GetAsync<T>();
                if (response.IsSuccess)
                    return response;
                else
                {
                    return default;
                }
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<Response<T>> SimpleSave<T>(T item, string call)
        {
            try
            {
                var client = new Client(new NewtonsoftSerializationAdapter(), new Uri($"{baseApiUri}"));
                client.SetJsonContentTypeHeader();
                return await client.PostAsync<T, T>(item, call);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return default;
            }
        }

        public async Task<Response> SimpleDelete(string call, string id)
        {
            try
            {
                var client = new Client(new NewtonsoftSerializationAdapter(), new Uri($"{baseApiUri}{call}/{id}"));
                client.SetJsonContentTypeHeader();
                return await client.DeleteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return default;
            }
        }


        public async Task<Response<T>> SimpleUpdate<T>(T item, string call, string id)
        {
            try
            {
                var client = new Client(new NewtonsoftSerializationAdapter(), new Uri($"{baseApiUri}"));
                client.SetJsonContentTypeHeader();

                return await client.PutAsync<T, T>(item, $"{call}/{id}");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return default;
            }
        }
        public async Task<Response<T>> SimpleGet<T>(string apiCall)
        {
            string call = $"{baseApiUri}{apiCall}";
            try
            {
                var client = new Client(new NewtonsoftSerializationAdapter(), baseUri: new Uri($"{call}"));
                client.ThrowExceptionOnFailure = false;
                var response = await client.GetAsync<T>();
                if (response.IsSuccess)
                    return response;
                else
                {
                    return default;
                }
            }
            catch (Exception)
            {
                return default;
            }
        }

        partial void OnGetSchedules(HttpRequestMessage requestMessage);
        public async Task<ODataServiceResult<Schedule>> GetSchedules(string filter = default(string), int? top = default(int?), int? skip = default(int?), string orderby = default(string), string expand = default(string), string select = default(string), bool? count = default(bool?))
        {
            var uri = new Uri(testUri, $"/odata/Schedule");
            uri = uri.GetODataUri(filter: filter, top: top, skip: skip, orderby: orderby, expand: expand, select: select, count: count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetSchedules(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await response.ReadAsync<ODataServiceResult<Schedule>>();
        }

        partial void OnGetGeneric(HttpRequestMessage requestMessage);
        public async Task<ODataServiceResult<T>> GetResults<T>(string filter = default(string), int? top = default(int?), int? skip = default(int?), string orderby = default(string), string expand = default(string), string select = default(string), bool? count = default(bool?), string endMethod = default(string))
        {
            var uri = new Uri(testUri, $"/odata/{endMethod}");
            uri = uri.GetODataUri(filter: filter, top: top, skip: skip, orderby: orderby, expand: expand, select: select, count: count);

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetGeneric(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await response.ReadAsync<ODataServiceResult<T>>();
        }

        partial void OnGetGenericNoFilter(HttpRequestMessage requestMessage);
        public async Task<ODataServiceResult<T>> GetResults<T>(string endMethod)
        {
            var uri = new Uri(testUri, $"/odata/{endMethod}");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            OnGetGenericNoFilter(httpRequestMessage);

            var response = await httpClient.SendAsync(httpRequestMessage);

            return await response.ReadAsync<ODataServiceResult<T>>();
        }
    }
}
