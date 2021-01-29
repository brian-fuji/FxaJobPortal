using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace FxaPortal.Services
{
    public class FileUploadService : IFileUploadService
    {
        private IConfiguration config;
        private string baseApiUri;
        public FileUploadService(IConfiguration configuration)
        {
            config = configuration;
            baseApiUri = config["apiURL"];
        }

        public async Task UploadAsync(HttpClient client, MultipartFormDataContent content, string methodCall)
        {
            string call = $"{baseApiUri}{methodCall}";

            await client.PostAsync(call, content);
        }
    }
}
