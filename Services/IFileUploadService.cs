using Blazorise;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FxaPortal.Services
{
    public interface IFileUploadService
    {
        Task UploadAsync(HttpClient client, MultipartFormDataContent content, string methodCall);
    }
}
