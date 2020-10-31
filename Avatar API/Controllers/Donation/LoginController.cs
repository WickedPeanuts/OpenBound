using Avatar_API.Data.Models;
using Avatar_API.Data.Services;
using Avatar_API.Filter;
using Avatar_API.Session;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OpenBound_Network_Object_Library.Session;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Avatar_API.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true), AuthorizeSessionAttribute]
    public class LoginController : Controller
    {
        private readonly UserService _userService;
        private readonly SessionHandler _sessionHandler;

        public LoginController(UserService userService,
                                  SessionHandler sessionHandler)
        {
            _sessionHandler = sessionHandler;
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginPostAsync(AuthenticateModel authenticateModel)
        {
            var x = _sessionHandler.GetSessionUser();
            var baseUrl = $"{Request.Scheme}://{Request.Host.Value}{Request.PathBase.Value}";
            var json = JsonConvert.SerializeObject(authenticateModel);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            using (var httpClient = new HttpClient())
            {
                using (var response = httpClient.PostAsync($"{baseUrl}/api/authorization/login", data))
                {
                    var apiResponse = response.Result;
                    var jsonString = await apiResponse.Content.ReadAsStringAsync();
                    var c = JsonConvert.DeserializeObject<SessionUser>(jsonString);
                    _sessionHandler.StartSession(c);
                    return RedirectToAction("Index", "Donation");
                }
            }
        }
    }
}
