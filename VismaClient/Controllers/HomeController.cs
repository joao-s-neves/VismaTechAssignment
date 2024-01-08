using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using VismaClient.Models;
using static VismaClient.Controllers.AccountController;

namespace VismaClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration pConfig;

        public HomeController(IConfiguration configuration)
        {
            pConfig = configuration;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(string username, string password)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            // Authenticate user using Auth0
            var isAuthenticated = await AuthenticateUserAsync(username, hashedPassword);

            if (isAuthenticated)
            {
                // If authentication is successful, make a call to the API for the timestamp
                var accessToken = await GetAccessToken();

                using (HttpClient client = new HttpClient())
                {
                    // Replace the URL with your API endpoint
                    string apiUrl = "https://localhost:yourapiport/api/home/gettimestamp";

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string result = await response.Content.ReadAsStringAsync();
                        ViewBag.Timestamp = result;
                        return View("Index");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Error retrieving timestamp from the API.";
                    }
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid username or password.";
            }

            // If authentication fails or API call fails, return to the login page
            return View("Index");
        }

        private async Task<bool> AuthenticateUserAsync(string username, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                var tokenEndpoint = $"https://{pConfig["Auth0:Domain"]}/oauth/token";

                var content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"grant_type", "password"},
                    {"username", username},
                    {"password", password},
                    {"client_id", pConfig["Auth0:ClientId"]},
                    {"client_secret", pConfig["Auth0:ClientSecret"]},
                    {"audience", "https://dev-wyby4agothizhjr8.us.auth0.com/api/v2/"}, // Replace with your API identifier
                    {"scope", "openid profile"}
                });

                var response = await client.PostAsync(tokenEndpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    //var result = await response.Content.ReadAsAsync<Auth0TokenResponse>();
                    var result = JsonSerializer.DeserializeAsync<Auth0TokenResponse>(await response.Content.ReadAsStreamAsync());

                    // You may want to store or use the access token as needed
                    return true;
                }

                return false;
            }
        }

        private async Task<string> GetAccessToken()
        {
            // Retrieve the access token from the authentication cookie
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            return accessToken;
        }

        public class Auth0TokenResponse
        {
            public string AccessToken { get; set; }
            public string TokenType { get; set; }
            public int ExpiresIn { get; set; }
            // Other properties as needed
        }
    }
}
