using Newtonsoft.Json;
using Serilog;
using System.Text;

namespace dotnet_rpg.Extensions
{
    public static class LogHelper
    {
        public static string RequestPayload = "";

       

        public static async void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext)
        {
            var request = httpContext.Request;

            dynamic reqBody = JsonConvert.DeserializeObject<dynamic>(RequestPayload);
            if(reqBody?.ContainsKey("bvn")==true)
                reqBody.bvn = "******";

            RequestPayload = JsonConvert.SerializeObject(reqBody);

            diagnosticContext.Set("RequestBody", RequestPayload);
            diagnosticContext.Set("ClientIp", httpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString());
            diagnosticContext.Set("CorrelationId", httpContext.Request.Headers["x-correlation-id"].ToString());
            diagnosticContext.Set("ClientId", httpContext.Request.Headers["client_id"].ToString());
            diagnosticContext.Set("ProductId", httpContext.Request.Headers["product_id"].ToString());
            string responseBodyPayload = await ReadResponseBody(httpContext.Response);
            diagnosticContext.Set("ResponseBody", responseBodyPayload);

            var responseBody = JsonConvert.DeserializeObject<ResponseBody>(responseBodyPayload);
            if (responseBody != null)
            {
                diagnosticContext.Set("ResponseCode", responseBody.Success);
                diagnosticContext.Set("Description", responseBody.Message);
            }



            // Set all the common properties available for every request
            diagnosticContext.Set("Host", request.Host);
            diagnosticContext.Set("Protocol", request.Protocol);
            diagnosticContext.Set("Scheme", request.Scheme);

            // Only set it if available. You're not sending sensitive data in a querystring right?!
            if (request.QueryString.HasValue)
            {
                diagnosticContext.Set("QueryString", request.QueryString.Value);
            }

            // Set the content-type of the Response at this point
            diagnosticContext.Set("ContentType", httpContext.Response.ContentType);

            // Retrieve the IEndpointFeature selected for the request
            var endpoint = httpContext.GetEndpoint();
            if (endpoint is object) // endpoint != null
            {
                diagnosticContext.Set("EndpointName", endpoint.DisplayName);
            }
        }

        private static async Task<string> ReadResponseBody(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            string responseBody = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            return $"{responseBody}";

        }
    }

    public class ResponseBody
    {
        public dynamic? Data{ get; set; }

        public bool Success { get; set; }

        public dynamic? Message { get; set; }
    }
}
