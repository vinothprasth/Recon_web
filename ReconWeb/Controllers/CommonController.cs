using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NuGet.Common;
using System.Net.Http;
using Newtonsoft.Json;
using System.Security.Policy;

namespace Recon.Controllers
{
    public class CommonController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<CommonController> _logger;
        private readonly HttpClient _httpClient;

        public CommonController(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            ILogger<CommonController> logger)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        //public string GetApiResult(string serializedString, string apiMethodName)
        //{
        //    string post_data = "";
        //    try
        //    {

        //        using (var client = new HttpClient())
        //        {
        //            client.BaseAddress = new Uri("https://localhost:44348/api/UserManagement/");
        //            client.DefaultRequestHeaders.Accept.Clear();
        //            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //            HttpContent content = new StringContent(serializedString, UTF8Encoding.UTF8, "application/json");
        //            content.Headers.Add("user_code", "12345");
        //            var response = client.PostAsync("Loginvalidation", content).Result;
        //            if (response.StatusCode.ToString() == "Unauthorized")
        //            {
        //                Console.WriteLine("Inside If");
        //            }
        //            else
        //            {
        //                Stream data = response.Content.ReadAsStreamAsync().Result;
        //                StreamReader reader = new StreamReader(data);
        //                post_data = reader.ReadToEnd();
        //            }

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex.Message == "Object reference not set to an instance of an object.")
        //        {
        //            Console.WriteLine("Inside If");
        //        }
        //        else
        //        {
        //            string control = this.ControllerContext.RouteData.Values["controller"].ToString(); 
        //            //LogHelper.WriteLog(ex.ToString(), control);
        //        }
        //    }
        //    return post_data;
        //}


        //Hema


        [HttpGet]
        public async Task GetToken()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var form = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", "user"),
                    new KeyValuePair<string, string>("password", "user"),
                });

                var tokenResponse = await client.PostAsync(_configuration["URL"] + "/token", form);

                if (tokenResponse.IsSuccessStatusCode)
                {
                    var token = await tokenResponse.Content.ReadFromJsonAsync<Token>();
                    string tokenValue = token.access_token;
                    HttpContext.Session.SetString("token_", tokenValue);
                }
            }
            catch (Exception ex)
            {
                string control = this.ControllerContext.RouteData.Values["controller"].ToString();
                //LogHelper.WriteLog(ex.ToString(), control);
                _logger.LogError(ex.ToString(), control);

            }
        }


        public string GetApiResult(string serializedString, string apiMethodName)
        {
            string post_data = "";
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:4195/api/UserManagement/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpContent content = new StringContent(serializedString, UTF8Encoding.UTF8, "application/json");
                    // content.Headers.Add("user_code", "12345"); // Remove if not needed
                    var response = client.PostAsync("Loginvalidation", content).Result;

                    // Check for a successful response (HTTP 200 OK)
                    if (response.IsSuccessStatusCode)
                    {
                        Stream data = response.Content.ReadAsStreamAsync().Result;
                        StreamReader reader = new StreamReader(data);
                        post_data = reader.ReadToEnd();
                    }
                    else
                    {
                        // Log the full response for debugging
                        Console.WriteLine($"API call failed with status code: {response.StatusCode}");
                        Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                    }
                }
            }
            catch (Exception ex)
            {
                string control = this.ControllerContext.RouteData.Values["controller"].ToString();
                // Log the exception for debugging
                Console.WriteLine($"Exception: {ex.Message}");
                // LogHelper.WriteLog(ex.ToString(), control);
            }
            return post_data;
        }



        public class Token
        {
            public string? access_token { get; set; }
        }

    }
}
