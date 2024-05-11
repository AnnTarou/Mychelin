using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Mychelin.Models;
using Newtonsoft.Json;

using System.Diagnostics;

namespace Mychelin.Controllers
{
    public class HomeController : Controller
    {
        // エラーログの取得
        private readonly ILogger<HomeController> _logger;

        // コンストラクター
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Home/IndexのGetメソッド
        public IActionResult Index()
        {
            // セッションからセッションIDを取得
            var sessionId = HttpContext.Session.GetString("SessionId");

            // セッションIDが存在しない場合はCookieをチェック
            if (sessionId == null)
            {
                HttpContext.Request.Cookies.TryGetValue("SessionId", out sessionId);
            }

            Person user = null;

            // セッションIDが取得できた場合、Jsonを変換してユーザーを取得
            if (sessionId != null)
            {
                var userJson = HttpContext.Session.GetString(sessionId);
                if (userJson != null)
                {
                    user = JsonConvert.DeserializeObject<Person>(userJson);
                }
            }

            // もしセッションもしくはクッキーがあればマイページへ
            if (user != null)
            {
                return RedirectToAction("Index", "Shoplists");
            }
            // セッションもしくはクッキーがなければHomeを表示
            else
            {
                return View();
            }
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
