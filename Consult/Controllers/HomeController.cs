using Consult.Models;
using FireSharp.Interfaces;
using FireSharp.Response;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace Consult.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        IFirebaseConfig config = new FireSharp.Config.FirebaseConfig
        {
            //AuthSecret = "nj4aEjsxCd7BtHO2uij3o9nrLM4LznPEmx8Wr0vj",
            BasePath = "https://consult673-default-rtdb.asia-southeast1.firebasedatabase.app"
        };
        IFirebaseClient client;

        public IActionResult Index()
        {
            var token = TempData["userEmail"] as string;

            //adding user details in TempData
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Users");
            dynamic? data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Team>();

            if (data != null)
            {
                foreach (var item in data)
                {
                    list.Add(JsonConvert.DeserializeObject<Team>(((JProperty)item).Value.ToString()));
                }
            }

            return View(list);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}