using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [ApiController]
    [Route("/")]
    public class SqsController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> ReceiveMessage()
        {
            // This is the content that _should_ be returned
            byte[] bytes = Encoding.UTF8.GetBytes("<?xml version=\"1.0\"?><ReceiveMessageResponse xmlns=\"http://queue.amazonaws.com/doc/2012-11-05/\"><ReceiveMessageResult/><ResponseMetadata><RequestId>1bd0633b-28ac-586d-a0da-3a90aaa242e4</RequestId></ResponseMetadata></ReceiveMessageResponse>");

            // Only set the information for the headers
            Response.Headers.ContentLength = bytes.Length;

            // Send the header bytes
            await Response.Body.FlushAsync();

            // Never send the content bytes
            await Task.Delay(Timeout.Infinite);

            // Unreachable
            return new EmptyResult();
        }
    }
}
