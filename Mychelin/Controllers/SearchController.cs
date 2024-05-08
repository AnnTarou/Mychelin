using Microsoft.AspNetCore.Mvc;
using Mychelin.Data;
using Mychelin.Filter;

namespace Mychelin.Controllers
{
    // セッションチェックすフィルターをクラスに適用
    [SessionCheckFilter]
    public class SearchController : Controller
    {
        // DBコンテキストを受け取るためのプロパティ
        private readonly MychelinContext _context;

        // コンストラクター
        public SearchController(MychelinContext context, IHostEnvironment hostEnvironment)
        {
            _context = context;
        }

        // Find（検索）ページ/Getアクセス
        public IActionResult Find()
        {
            // セッションからユーザーIDを取得
            var userId = HttpContext.Session.GetInt32("PersonId");

            // ユーザーIDに関連するShoplistの中で全レコードを取得
            var shoplists = _context.Shoplist
                .Where(s => s.PersonId == userId.Value)
                .ToList();

            // ログインしているユーザーのShoplistをビューに渡す
            return View(shoplists);
        }

        // Find（検索）ページ/Postメソッド
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Find(string Findstr)
        {
            return View();
        }

        // Lunchページ/Getアクセス
        public IActionResult Lunch()
        {
            // セッションからユーザーIDを取得
            var userId = HttpContext.Session.GetInt32("PersonId");

            // ユーザーIDに関連するShoplistの中でランチカテゴリのレコードのみを取得
            var shoplists = _context.Shoplist
                .Where(s => s.PersonId == userId.Value && s.Category == "ランチ")
                .ToList();

            // ログインしているユーザーのShoplistをビューに渡す
            return View(shoplists);
        }

        // Lunchページ/Postメソッド
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Lunch(string Findstr)
        {
            return View();

        }
        // Dinnerページ/Getアクセス
        public IActionResult Dinner()
        {
            // セッションからユーザーIDを取得
            var userId = HttpContext.Session.GetInt32("PersonId");

            // ユーザーIDに関連するShoplistの中でディナーカテゴリのレコードのみを取得
            var shoplists = _context.Shoplist
                .Where(s => s.PersonId == userId.Value && s.Category == "ディナー")
                .ToList();

            // ログインしているユーザーのShoplistをビューに渡す
            return View(shoplists);
        }

        // Dinnerページ/Postメソッド
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dinner(string Findstr)
        {
            return View();

        }

        // Sweetページ/Getアクセス
        public IActionResult Sweet()
        {
            // セッションからユーザーIDを取得
            var userId = HttpContext.Session.GetInt32("PersonId");

            // ユーザーIDに関連するShoplistの中でスイーツカテゴリのレコードのみを取得
            var shoplists = _context.Shoplist
                .Where(s => s.PersonId == userId.Value && s.Category == "スイーツ")
                .ToList();

            // ログインしているユーザーのShoplistをビューに渡す
            return View(shoplists);
        }

        // Sweetページ/Postメソッド
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sweet(string Findstr)
        {
            return View();

        }

        // Barページ/Getアクセス
        public IActionResult Bar()
        {
            // セッションからユーザーIDを取得
            var userId = HttpContext.Session.GetInt32("PersonId");

            // ユーザーIDに関連するShoplistの中でBarカテゴリのレコードのみを取得
            var shoplists = _context.Shoplist
                .Where(s => s.PersonId == userId.Value && s.Category == "Bar")
                .ToList();

            // ログインしているユーザーのShoplistをビューに渡す
            return View(shoplists);
        }

        //  Barページ/Postメソッド
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Bar(string Findstr)
        {
            return View();

        }

        // Favoriteページ/Getアクセス
        public IActionResult Favorite()
        {
            // セッションからユーザーIDを取得
            var userId = HttpContext.Session.GetInt32("PersonId");

            // ユーザーIDに関連するShoplistの中で評価が４以上のレコードを取得
            var shoplists = _context.Shoplist
                .Where(s => s.PersonId == userId.Value && s.Star >= 4)
                .ToList();

            // ログインしているユーザーのShoplistをビューに渡す
            return View(shoplists);
        }

        //  Favoriteページ/Postメソッド
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Favorite(string Findstr)
        {
            return View();
        }

        // Wantページ/Getアクセス
        public IActionResult Want()
        {
            // セッションからユーザーIDを取得
            var userId = HttpContext.Session.GetInt32("PersonId");

            // ユーザーIDに関連するShoplistの中でステータスが"気になる"のレコードを取得
            var shoplists = _context.Shoplist
                .Where(s => s.PersonId == userId.Value && s.Status == "気になる")
                .ToList();

            // ログインしているユーザーのShoplistをビューに渡す
            return View(shoplists);
        }

        //  Favoriteページ/Postメソッド
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Want(string Findstr)
        {
            return View();
        }
    }
}
