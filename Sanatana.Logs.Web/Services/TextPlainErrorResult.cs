using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net.Http;
using System.Threading;
using System.Net;

namespace Sanatana.Logs.Web.Services
{
    internal class TextPlainErrorResult : IHttpActionResult
    {
        //properties
        public HttpRequestMessage Request { get; set; }

        public string Content { get; set; }


        //methods
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(Content),
                RequestMessage = Request,
            };

            Uri referrer = Request.Headers.Referrer;
            if (referrer != null)
                response.Headers.Add("Access-Control-Allow-Origin", referrer.GetLeftPart(UriPartial.Authority));

            return Task.FromResult(response);
        }
    }
}
