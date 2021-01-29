using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FxaPortal.Models
{
    public class BaseResponse
    {
        public Paging Paging { get; set; }      
        public HttpResponseMessage ResponseMessage { get; protected set; }

        public bool IsSuccess { get
            {
                return ResponseMessage.StatusCode == HttpStatusCode.OK;
            } 
        }

        public BaseResponse()
        {

        }
    }
}
