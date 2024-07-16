using Amazon.Lambda.Core;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Serilog;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using webHookForCrm.Model;
namespace webHookForCrm.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebHookController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string token = "EAAXNXc6GmwQBOzDYttQ7kNJd8oHoqYoqVxsLUpTZBz8DOsR8I0yRxNgIDd1ENmmx27Es9sTtNBQvUpeKJqIwQbduRiK9LVi9nu7MHYlg1cSOMCgdG1GZC0sED5LczB510BYZA1lgPhZANcXNFxsxRoH4oJTIJxzurPt20QbabErbAqQm3RcvjhH76SsSnQzZCZAqmL3ukeu8rY2sSMS4cj";
        private readonly string mytoken ;

        public WebHookController(HttpClient httpClient,IConfiguration configuration)
        {
            _httpClient = httpClient;
           this.mytoken = configuration["MyVerificationToken"];
        }

        [HttpGet("webhook")]
        public IActionResult Get([FromQuery] string hubMode, [FromQuery] string hubChallenge, [FromQuery] string hubVerifyToken)
        {
            if (!string.IsNullOrEmpty(hubMode) && !string.IsNullOrEmpty(hubVerifyToken))
            {
                Log.Information("Iam here");
                Log.Information(mytoken);
                if (hubMode == "subscribe" && hubVerifyToken == mytoken)
                {
                    try
                    {
                        Log.Information("inside the subscribed");
                        CookieOptions cookieOptions = new CookieOptions
                        {
                            HttpOnly = true, // Prevent client-side JavaScript access
                            Secure = true,  // Only send over HTTPS connections (if applicable)
                            Expires = DateTimeOffset.UtcNow.AddMinutes(15) // Set appropriate expiration time
                        };
                        Response.Cookies.Append("expectedChallenge", hubChallenge, cookieOptions);
                        Log.Information(hubVerifyToken);
                        return Ok();
                    }catch(Exception ex)
                    {
                        Log.Error(ex.ToString()+"error is occured");
                        return StatusCode(500);
                    }
                }
                else
                {
                    Log.Information("forbidden");
                    return Forbid();
                }
            }
            return BadRequest();
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Post(WebhookRequest body)
        {
            Log.Information("hi");
            //var bodyParam = body;
            //System.Console.WriteLine(bodyParam.ToString(Newtonsoft.Json.Formatting.Indented));

            if (body.Object != null)
            {
                var entry = body.Entry?[0]?.Changes?[0]?.Value;
                if (entry != null && entry.Messages != null && entry.Messages[0] != null)
                {
                    var phoneNoId = entry.Metadata?.Phone_number_id?.ToString();
                    var from = entry.Messages[0]?.From?.ToString();
                    var msgBody = entry.Messages[0]?.Text?.Body?.ToString();

                    if (!string.IsNullOrEmpty(phoneNoId) && !string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(msgBody))
                    {
                        var requestBody = new
                        {
                            messaging_product = "whatsapp",
                            to = from,
                            text = new { body = "Hi.. I'm Bibin" }
                        };

                        var json = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var response = await _httpClient.PostAsync($"https://graph.facebook.com/v19.0/{phoneNoId}/messages?access_token={token}", content);

                        return Ok();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            return BadRequest();
        }

        [HttpGet]
        public IActionResult Index()
        {
            Log.Information("Its me bibin");
            
            return Ok("hello this is web hook set up");
        }
    }
}