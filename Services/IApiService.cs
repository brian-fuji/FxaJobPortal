using System.Collections.Generic;
using System.Threading.Tasks;
using FxaPortal.Models;
using RestClient.Net.Abstractions;

namespace FxaPortal.Services
{
    public interface IApiService
    {
        Task<Response<T>> SimplePagedGet<T>(int page, int size, string methodCall);

        Task<Response<T>> SimpleSave<T>(T item, string call);

        Task<Response<T>> SimpleUpdate<T>(T item, string call, string id);

    }
}
