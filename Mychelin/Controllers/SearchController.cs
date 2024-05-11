using Microsoft.AspNetCore.Mvc;
using Mychelin.Data;
using Mychelin.Models;

namespace Mychelin.Controllers
{
    // セッションチェックすフィルターをクラスに適用
    [SessionCheckFilter]
    public class SearchController : Controller
    {
        // DBコンテキストを受け取るためのプロパティ
        private readonly MychelinContext _context;

        // コンストラクター
        public SearchController(MychelinContext context)
        {
            _context = context;
        }
        
        // Find（検索）ページ/Getアクセス
        public IActionResult Find()
        {
            // セッションからユーザーIDを取得
            var user = (Person)HttpContext.Items["User"];
            var userId = user.PersonId;

            // ユーザーIDに関連するShoplistの中で全レコードを取得
            var shoplists = _context.Shoplist
                .Where(s => s.PersonId == userId)
                .ToList();

            // ログインしているユーザーのShoplistをビューに渡す
            return View(shoplists);
        }

        // Find（検索）ページ/Postメソッド
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Find(string FindStr)
        {
            // セッションからユーザーIDを取得
            var user = (Person)HttpContext.Items["User"];
            var userId = user.PersonId;

            // 検索文字列が空の場合は全レコードを取得し名前順に並べ替え
            if (string.IsNullOrEmpty(FindStr))
            {
                var shoplists = _context.Shoplist
                .Where(s => s.PersonId == userId)
                .OrderBy(s => s.ShoplistName).ToList();
                return View(shoplists);
            }
            // 検索文字列が空でない場合はShoplistの各カラムで検索文字列を含むレコードを取得
            else
            {
                // 純粋に検索文字のみを配列入れるために StringSplitOptions.RemoveEmptyEntriesを利用
                string[] keywords = FindStr.Split(new char[] { '　',' ', ',','、' }, StringSplitOptions.RemoveEmptyEntries);

                // Shoplistを検索してキーワードの文字いづれかが含まれるものを取得
                var shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId &
                    (keywords.Any(k => s.ShoplistName.Contains(k)) ||
                     keywords.Any(k => s.Status.Contains(k)) ||
                     keywords.Any(k => s.Category.Contains(k)) ||
                     keywords.Any(k => s.Class.Contains(k)) ||
                     keywords.Any(k => s.Coment.Contains(k)) ||
                     keywords.Any(k => s.UpdatedDate.ToString().Contains(k))))
                    .ToList();
               
                return View(shoplists);
            }
        }

        // Find（検索）ページ/クイック検索のPostメソッド
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FindCategory(string category)
        {
            // セッションからユーザーIDを取得
            var user = (Person)HttpContext.Items["User"];
            var userId = user.PersonId;

            // 抽出したエンティティを入れるリスト
            List<Shoplist>? shoplists;

            // 検索するキーワードのリスト
            List<string> keywords;

            switch (category)
            {
                case "おすすめ":

                    // 評価4以上のものを抽出
                    shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId && s.Star >= 4)
                    .ToList();

                    return View("Find", shoplists);

                case "デート":

                    // 検索するキーワードのリスト化
                    keywords = new List<string> { "デート", "彼氏", "彼女", "洒落", "しゃれ", "オシャレ","雰囲気" };

                    // キーワードにコメントの内容が含まれるものを抽出
                    shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId && keywords.Any(k => s.Coment.Contains(k)))
                    .ToList();

                    return View("Find", shoplists);

                case "子連れ":

                    // 検索するキーワードのリスト化
                    keywords = new List<string> {"子供","こども","子ども","キッズ","子連れ","子づれ" };

                    // キーワードにコメントの内容が含まれるものを抽出
                    shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId && keywords.Any(k => s.Coment.Contains(k)))
                    .ToList();
                    return View("Find", shoplists);

                case "コスパ":

                    // コメントにコスパが含まれるまたはクラスが安価のものを抽出
                    shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId &&
                    (s.Coment.Contains("コスパ") ||
                    s.Class == "安価")).ToList();

                    return View("Find", shoplists);

                case "女子":

                    // 検索するキーワードのリスト化
                    keywords = new List<string> { "女子会", "女", "友達", "ともだち", "友だち", "おしゃれ","オシャレ","洒落","かわいい","可愛い","映え" };

                    // キーワードにコメントの内容が含まれるものを抽出
                    shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId && keywords.Any(k => s.Coment.Contains(k)))
                    .ToList();

                    return View("Find", shoplists);

                default:

                    return View("Find");
            }
        }

        // Lunchページ/Getアクセス
        public IActionResult Lunch()
        {
            // セッションからユーザーIDを取得
            var user = (Person)HttpContext.Items["User"];
            var userId = user.PersonId;

            // ユーザーIDに関連するShoplistの中でランチカテゴリのレコードのみを取得
            var shoplists = _context.Shoplist
                .Where(s => s.PersonId == userId && s.Category == "ランチ")
                .ToList();

            // ログインしているユーザーのShoplistをビューに渡す
            return View(shoplists);
        }

        // Lunchページ/Postメソッド
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Lunch(string FindStr)
        {
            // セッションからユーザーIDを取得
            var user = (Person)HttpContext.Items["User"];
            var userId = user.PersonId;

            // 検索文字列が空の場合は全Lunchカテゴリのレコードを取得し名前順に並べ替え
            if (string.IsNullOrEmpty(FindStr))
            {
                var shoplists = _context.Shoplist
                .Where(s => s.PersonId == userId && s.Category == "ランチ")
                .OrderBy(s => s.ShoplistName).ToList();
                return View(shoplists);
            }
            // 検索文字列が空でない場合はShoplistの各カラムで検索文字列を含むレコードを取得
            else
            {
                // 純粋に検索文字のみを配列入れるために StringSplitOptions.RemoveEmptyEntriesを利用
                string[] keywords = FindStr.Split(new char[] { '　', ' ', ',', '、' }, StringSplitOptions.RemoveEmptyEntries);

                // Shoplistを検索してキーワードの文字いづれかが含まれるものを取得
                var shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId && s.Category == "ランチ" &&
                    (keywords.Any(k => s.ShoplistName.Contains(k)) ||
                     keywords.Any(k => s.Status.Contains(k)) ||
                     keywords.Any(k => s.Category.Contains(k)) ||
                     keywords.Any(k => s.Class.Contains(k)) ||
                     keywords.Any(k => s.Coment.Contains(k)) ||
                     keywords.Any(k => s.UpdatedDate.ToString().Contains(k))))
                    .ToList();

                return View(shoplists);
            }

        }

        // Lunchページ/クイック検索のPostメソッド
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LunchFindCategory(string category)
        {
            // セッションからユーザーIDを取得
            var user = (Person)HttpContext.Items["User"];
            var userId = user.PersonId;

            // 抽出したエンティティを入れるリスト
            List<Shoplist>? shoplists;

            // 検索するキーワードのリスト
            List<string> keywords;

            switch (category)
            {
                case "おすすめ":

                    // カテゴリがランチでかつ評価4以上のものを抽出
                    shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId && s.Category == "ランチ" && s.Star >= 4)
                    .ToList();

                    return View("Lunch", shoplists);

                case "デート":

                    // 検索するキーワードのリスト化
                    keywords = new List<string> { "デート", "彼氏", "彼女", "洒落", "しゃれ", "オシャレ", "雰囲気" };

                    // カテゴリがランチかつキーワードにコメントの内容が含まれるものを抽出
                    shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId && s.Category == "ランチ" && keywords.Any(k => s.Coment.Contains(k)))
                    .ToList();

                    return View("Lunch", shoplists);

                case "子連れ":

                    // 検索するキーワードのリスト化
                    keywords = new List<string> { "子供", "こども", "子ども", "キッズ", "子連れ", "子づれ" };

                    // カテゴリがランチかつキーワードにコメントの内容が含まれるものを抽出
                    shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId && s.Category == "ランチ" && keywords.Any(k => s.Coment.Contains(k)))
                    .ToList();
                    return View("Lunch", shoplists);

                case "コスパ":

                    // カテゴリがランチかつコメントにコスパが含まれるまたはクラスが安価のものを抽出
                    shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId && s.Category == "ランチ" &&
                    (s.Coment.Contains("コスパ") ||
                    s.Class == "安価")).ToList();

                    return View("Lunch", shoplists);

                case "女子":

                    // 検索するキーワードのリスト化
                    keywords = new List<string> { "女子会", "女", "友達", "ともだち", "友だち", "おしゃれ", "オシャレ", "洒落", "かわいい", "可愛い", "映え" };

                    // カテゴリがランチかつキーワードにコメントの内容が含まれるものを抽出
                    shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId && s.Category == "ランチ" && 
                    keywords.Any(k => s.Coment.Contains(k)))
                    .ToList();

                    return View("Lunch", shoplists);

                default:

                    return View("Lunch");
            }
        }

        // Dinnerページ/Getアクセス
        public IActionResult Dinner()
        {
            // セッションからユーザーIDを取得
            var user = (Person)HttpContext.Items["User"];
            var userId = user.PersonId;

            // ユーザーIDに関連するShoplistの中でディナーカテゴリのレコードのみを取得し新しい順に並べ替え
            var shoplists = _context.Shoplist
                .Where(s => s.PersonId == userId && s.Category == "ディナー")
                .OrderByDescending(s => s.UpdatedDate).ToList();

            // ログインしているユーザーのShoplistをビューに渡す
            return View(shoplists);
        }

        // Dinnerページ/Postメソッド
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dinner(string FindStr)
        {
            // セッションからユーザーIDを取得
            var user = (Person)HttpContext.Items["User"];
            var userId = user.PersonId;

            // 検索文字列が空の場合は全Lunchカテゴリのレコードを取得し名前順に並べ替え
            if (string.IsNullOrEmpty(FindStr))
            {
                var shoplists = _context.Shoplist
                .Where(s => s.PersonId == userId && s.Category == "ディナー")
                .OrderBy(s => s.ShoplistName).ToList();
                return View(shoplists);
            }
            // 検索文字列が空でない場合はShoplistの各カラムで検索文字列を含むレコードを取得
            else
            {
                // 純粋に検索文字のみを配列入れるために StringSplitOptions.RemoveEmptyEntriesを利用
                string[] keywords = FindStr.Split(new char[] { '　', ' ', ',', '、' }, StringSplitOptions.RemoveEmptyEntries);

                // Shoplistを検索してキーワードの文字いづれかが含まれるものを取得
                var shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId && s.Category == "ディナー" &&
                    (keywords.Any(k => s.ShoplistName.Contains(k)) ||
                     keywords.Any(k => s.Status.Contains(k)) ||
                     keywords.Any(k => s.Category.Contains(k)) ||
                     keywords.Any(k => s.Class.Contains(k)) ||
                     keywords.Any(k => s.Coment.Contains(k)) ||
                     keywords.Any(k => s.UpdatedDate.ToString().Contains(k))))
                    .ToList();

                return View(shoplists);
            }
        }

        // Dinnerページ/クイック検索のPostメソッド
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DinnerFindCategory(string category)
        {
            // セッションからユーザーIDを取得
            var user = (Person)HttpContext.Items["User"];
            var userId = user.PersonId;

            // 抽出したエンティティを入れるリスト
            List<Shoplist>? shoplists;

            // 検索するキーワードのリスト
            List<string> keywords;

            switch (category)
            {
                case "おすすめ":

                    // カテゴリがディナーでかつ評価4以上のものを抽出
                    shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId && s.Category == "ディナー" && s.Star >= 4)
                    .ToList();

                    return View("Dinner", shoplists);

                case "デート":

                    // 検索するキーワードのリスト化
                    keywords = new List<string> { "デート", "彼氏", "彼女", "洒落", "しゃれ", "オシャレ", "雰囲気" };

                    // カテゴリがディナーかつキーワードにコメントの内容が含まれるものを抽出
                    shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId && s.Category == "ディナー" && keywords.Any(k => s.Coment.Contains(k)))
                    .ToList();

                    return View("Dinner", shoplists);

                case "子連れ":

                    // 検索するキーワードのリスト化
                    keywords = new List<string> { "子供", "こども", "子ども", "キッズ", "子連れ", "子づれ" };

                    // カテゴリがディナーかつキーワードにコメントの内容が含まれるものを抽出
                    shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId && s.Category == "ディナー" && keywords.Any(k => s.Coment.Contains(k)))
                    .ToList();
                    return View("Dinner", shoplists);

                case "コスパ":

                    // カテゴリがディナーかつコメントにコスパが含まれるまたはクラスが安価のものを抽出
                    shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId && s.Category == "ディナー" &&
                    (s.Coment.Contains("コスパ") ||
                    s.Class == "安価")).ToList();

                    return View("Dinner", shoplists);

                case "女子":

                    // 検索するキーワードのリスト化
                    keywords = new List<string> { "女子会", "女", "友達", "ともだち", "友だち", "おしゃれ", "オシャレ", "洒落", "かわいい", "可愛い", "映え" };

                    // カテゴリがディナーかつキーワードにコメントの内容が含まれるものを抽出
                    shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId && s.Category == "ディナー" &&
                    keywords.Any(k => s.Coment.Contains(k)))
                    .ToList();

                    return View("Dinner", shoplists);

                default:

                    return View("Dinner");
            }
        }

        // Sweetページ/Getアクセス
        public IActionResult Sweet()
        {
            // セッションからユーザーIDを取得
            var user = (Person)HttpContext.Items["User"];
            var userId = user.PersonId;

            // ユーザーIDに関連するShoplistの中でスイーツカテゴリのレコードのみを取得し新しい順に並べ替え
            var shoplists = _context.Shoplist
                .Where(s => s.PersonId == userId && s.Category == "スイーツ")
                .OrderByDescending(s => s.UpdatedDate).ToList().ToList();

            // ログインしているユーザーのShoplistをビューに渡す
            return View(shoplists);
        }

        // Sweetページ/Postメソッド
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sweet(string FindStr)
        {
            // セッションからユーザーIDを取得
            var user = (Person)HttpContext.Items["User"];
            var userId = user.PersonId;

            // 検索文字列が空の場合は全Lunchカテゴリのレコードを取得し名前順に並べ替え
            if (string.IsNullOrEmpty(FindStr))
            {
                var shoplists = _context.Shoplist
                .Where(s => s.PersonId == userId && s.Category == "スイーツ")
                .OrderBy(s => s.ShoplistName).ToList();
                return View(shoplists);
            }
            // 検索文字列が空でない場合はShoplistの各カラムで検索文字列を含むレコードを取得
            else
            {
                // 純粋に検索文字のみを配列入れるために StringSplitOptions.RemoveEmptyEntriesを利用
                string[] keywords = FindStr.Split(new char[] { '　', ' ', ',', '、' }, StringSplitOptions.RemoveEmptyEntries);

                // Shoplistを検索してキーワードの文字いづれかが含まれるものを取得
                var shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId && s.Category == "スイーツ" &&
                    (keywords.Any(k => s.ShoplistName.Contains(k)) ||
                     keywords.Any(k => s.Status.Contains(k)) ||
                     keywords.Any(k => s.Category.Contains(k)) ||
                     keywords.Any(k => s.Class.Contains(k)) ||
                     keywords.Any(k => s.Coment.Contains(k)) ||
                     keywords.Any(k => s.UpdatedDate.ToString().Contains(k))))
                    .ToList();

                return View(shoplists);
            }
        }

        // Sweetページ/クイック検索のPostメソッド
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SweetFindCategory(string category)
        {
            // セッションからユーザーIDを取得
            var user = (Person)HttpContext.Items["User"];
            var userId = user.PersonId;

            // 抽出したエンティティを入れるリスト
            List<Shoplist>? shoplists;

            // 検索するキーワードのリスト
            List<string> keywords;

            switch (category)
            {
                case "おすすめ":

                    // カテゴリがスイーツかつ評価4以上のものを抽出
                    shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId && s.Category == "スイーツ" && s.Star >= 4)
                    .ToList();

                    return View("Sweet", shoplists);

                case "デート":

                    // 検索するキーワードのリスト化
                    keywords = new List<string> { "デート", "彼氏", "彼女", "洒落", "しゃれ", "オシャレ", "雰囲気" };

                    // カテゴリがスイーツかつキーワードにコメントの内容が含まれるものを抽出
                    shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId && s.Category == "スイーツ" && keywords.Any(k => s.Coment.Contains(k)))
                    .ToList();

                    return View("Sweet", shoplists);

                case "子連れ":

                    // 検索するキーワードのリスト化
                    keywords = new List<string> { "子供", "こども", "子ども", "キッズ", "子連れ", "子づれ" };

                    // カテゴリがスイーツかつキーワードにコメントの内容が含まれるものを抽出
                    shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId && s.Category == "スイーツ" && keywords.Any(k => s.Coment.Contains(k)))
                    .ToList();
                    return View("Sweet", shoplists);

                case "コスパ":

                    // カテゴリがスイーツかつコメントにコスパが含まれるまたはクラスが安価のものを抽出
                    shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId && s.Category == "スイーツ" &&
                    (s.Coment.Contains("コスパ") ||
                    s.Class == "安価")).ToList();

                    return View("Sweet", shoplists);

                case "女子":

                    // 検索するキーワードのリスト化
                    keywords = new List<string> { "女子会", "女", "友達", "ともだち", "友だち", "おしゃれ", "オシャレ", "洒落", "かわいい", "可愛い", "映え" };

                    // カテゴリがスイーツかつキーワードにコメントの内容が含まれるものを抽出
                    shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId && s.Category == "スイーツ" &&
                    keywords.Any(k => s.Coment.Contains(k)))
                    .ToList();

                    return View("Sweet", shoplists);

                default:

                    return View("Sweet");
            }
        }

        // Barページ/Getアクセス
        public IActionResult Bar()
        {// セッションからユーザーIDを取得
            var user = (Person)HttpContext.Items["User"];
            var userId = user.PersonId;

            // ユーザーIDに関連するShoplistの中でBarカテゴリのレコードのみを取得し新しい順に並べ替え
            var shoplists = _context.Shoplist
                .Where(s => s.PersonId == userId && s.Category == "Bar")
                .OrderByDescending(s => s.UpdatedDate).ToList().ToList();

            // ログインしているユーザーのShoplistをビューに渡す
            return View(shoplists);
        }

        //  Barページ/Postメソッド
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Bar(string FindStr)
        {
            // セッションからユーザーIDを取得
            var user = (Person)HttpContext.Items["User"];
            var userId = user.PersonId;

            // 検索文字列が空の場合は全Lunchカテゴリのレコードを取得し名前順に並べ替え
            if (string.IsNullOrEmpty(FindStr))
            {
                var shoplists = _context.Shoplist
                .Where(s => s.PersonId == userId && s.Category == "Bar")
                .OrderBy(s => s.ShoplistName).ToList();
                return View(shoplists);
            }
            // 検索文字列が空でない場合はShoplistの各カラムで検索文字列を含むレコードを取得
            else
            {
                // 純粋に検索文字のみを配列入れるために StringSplitOptions.RemoveEmptyEntriesを利用
                string[] keywords = FindStr.Split(new char[] { '　', ' ', ',', '、' }, StringSplitOptions.RemoveEmptyEntries);

                // Shoplistを検索してキーワードの文字いづれかが含まれるものを取得
                var shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId && s.Category == "Bar" &&
                    (keywords.Any(k => s.ShoplistName.Contains(k)) ||
                     keywords.Any(k => s.Status.Contains(k)) ||
                     keywords.Any(k => s.Category.Contains(k)) ||
                     keywords.Any(k => s.Class.Contains(k)) ||
                     keywords.Any(k => s.Coment.Contains(k)) ||
                     keywords.Any(k => s.UpdatedDate.ToString().Contains(k))))
                    .ToList();

                return View(shoplists);
            }
        }

        //  Barページ/クイック検索のPostメソッド
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BarFindCategory(string category)
        {
            // セッションからユーザーIDを取得
            var user = (Person)HttpContext.Items["User"];
            var userId = user.PersonId;

            // 抽出したエンティティを入れるリスト
            List<Shoplist>? shoplists;

            // 検索するキーワードのリスト
            List<string> keywords;

            switch (category)
            {
                case "おすすめ":

                    // カテゴリがBarでかつ評価4以上のものを抽出
                    shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId && s.Category == "Bar" && s.Star >= 4)
                    .ToList();

                    return View("Bar", shoplists);

                case "デート":

                    // 検索するキーワードのリスト化
                    keywords = new List<string> { "デート", "彼氏", "彼女", "洒落", "しゃれ", "オシャレ", "雰囲気" };

                    // カテゴリがBarかつキーワードにコメントの内容が含まれるものを抽出
                    shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId && s.Category == "Bar" && keywords.Any(k => s.Coment.Contains(k)))
                    .ToList();

                    return View("Bar", shoplists);

                case "子連れ":

                    // 検索するキーワードのリスト化
                    keywords = new List<string> { "子供", "こども", "子ども", "キッズ", "子連れ", "子づれ" };

                    // カテゴリがBarかつキーワードにコメントの内容が含まれるものを抽出
                    shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId && s.Category == "Bar" && keywords.Any(k => s.Coment.Contains(k)))
                    .ToList();
                    return View("Bar", shoplists);

                case "コスパ":

                    // カテゴリがBarかつコメントにコスパが含まれるまたはクラスが安価のものを抽出
                    shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId && s.Category == "Bar" &&
                    (s.Coment.Contains("コスパ") ||
                    s.Class == "安価")).ToList();

                    return View("Bar", shoplists);

                case "女子":

                    // 検索するキーワードのリスト化
                    keywords = new List<string> { "女子会", "女", "友達", "ともだち", "友だち", "おしゃれ", "オシャレ", "洒落", "かわいい", "可愛い", "映え" };

                    // カテゴリがBarかつキーワードにコメントの内容が含まれるものを抽出
                    shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId && s.Category == "Bar" &&
                    keywords.Any(k => s.Coment.Contains(k)))
                    .ToList();

                    return View("Bar", shoplists);

                default:

                    return View("Bar");
            }
        }

        // Favoriteページ/Getアクセス
        public IActionResult Favorite()
        {
            // セッションからユーザーIDを取得
            var user = (Person)HttpContext.Items["User"];
            var userId = user.PersonId;

            // ユーザーIDに関連するShoplistの中で評価が４以上のレコードを取得し新しい順に並べ替え
            var shoplists = _context.Shoplist
                .Where(s => s.PersonId == userId && s.Star >= 4)
                .OrderByDescending(s => s.UpdatedDate).ToList().ToList();

            // ログインしているユーザーのShoplistをビューに渡す
            return View(shoplists);
        }

        //  Favoriteページ/Postメソッド
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Favorite(string FindStr)
        {
            // セッションからユーザーIDを取得
            var user = (Person)HttpContext.Items["User"];
            var userId = user.PersonId;

            // 検索文字列が空の場合は全Lunchカテゴリのレコードを取得し名前順に並べ替え
            if (string.IsNullOrEmpty(FindStr))
            {
                var shoplists = _context.Shoplist
                .Where(s => s.PersonId == userId && s.Star >= 4)
                .OrderBy(s => s.ShoplistName).ToList();
                return View(shoplists);
            }
            // 検索文字列が空でない場合はShoplistの各カラムで検索文字列を含むレコードを取得
            else
            {
                // 純粋に検索文字のみを配列入れるために StringSplitOptions.RemoveEmptyEntriesを利用
                string[] keywords = FindStr.Split(new char[] { '　', ' ', ',', '、' }, StringSplitOptions.RemoveEmptyEntries);

                // Shoplistを検索してキーワードの文字いづれかが含まれるものを取得
                var shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId && s.Star >= 4 &&
                    (keywords.Any(k => s.ShoplistName.Contains(k)) ||
                     keywords.Any(k => s.Status.Contains(k)) ||
                     keywords.Any(k => s.Category.Contains(k)) ||
                     keywords.Any(k => s.Class.Contains(k)) ||
                     keywords.Any(k => s.Coment.Contains(k)) ||
                     keywords.Any(k => s.UpdatedDate.ToString().Contains(k))))
                    .ToList();

                return View(shoplists);
            }
        }

        //  Favoriteページ/クイック検索のPostメソッド
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FavoriteFindCategory(string category)
        {
            // セッションからユーザーIDを取得
            var user = (Person)HttpContext.Items["User"];
            var userId = user.PersonId;

            // 抽出したエンティティを入れるリスト
            List<Shoplist>? shoplists;

            // 検索するキーワードのリスト
            List<string> keywords;

            switch (category)
            {
                case "デート":

                    // 検索するキーワードのリスト化
                    keywords = new List<string> { "デート", "彼氏", "彼女", "洒落", "しゃれ", "オシャレ", "雰囲気" };

                    // Star4以上かつキーワードにコメントの内容が含まれるものを抽出
                    shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId && s.Star >= 4 && keywords.Any(k => s.Coment.Contains(k)))
                    .ToList();

                    return View("Favorite", shoplists);

                case "子連れ":

                    // 検索するキーワードのリスト化
                    keywords = new List<string> { "子供", "こども", "子ども", "キッズ", "子連れ", "子づれ" };

                    // Star4以上かつキーワードにコメントの内容が含まれるものを抽出
                    shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId && s.Star >= 4 && keywords.Any(k => s.Coment.Contains(k)))
                    .ToList();
                    return View("Favorite", shoplists);

                case "コスパ":

                    // Star4以上かつコメントにコスパが含まれるまたはクラスが安価のものを抽出
                    shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId && s.Star >= 4 &&
                    (s.Coment.Contains("コスパ") ||
                    s.Class == "安価")).ToList();

                    return View("Favorite", shoplists);

                case "女子":

                    // 検索するキーワードのリスト化
                    keywords = new List<string> { "女子会", "女", "友達", "ともだち", "友だち", "おしゃれ", "オシャレ", "洒落", "かわいい", "可愛い", "映え" };

                    // Star4以上かつキーワードにコメントの内容が含まれるものを抽出
                    shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId && s.Star >= 4 &&
                    keywords.Any(k => s.Coment.Contains(k)))
                    .ToList();

                    return View("Favorite", shoplists);

                default:

                    return View("Favorite");
            }
        }

        // Wantページ/Getアクセス
        public IActionResult Want()
        {
            // セッションからユーザーIDを取得
            var user = (Person)HttpContext.Items["User"];
            var userId = user.PersonId;

            // ユーザーIDに関連するShoplistの中でステータスが"気になる"のレコードを取得し新しい順に並べ替え
            var shoplists = _context.Shoplist
                .Where(s => s.PersonId == userId && s.Status == "気になる")
                .OrderByDescending(s => s.UpdatedDate).ToList().ToList();

            // ログインしているユーザーのShoplistをビューに渡す
            return View(shoplists);
        }

        //  Wantページ/Postメソッド
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Want(string FindStr)
        {
            // セッションからユーザーIDを取得
            var user = (Person)HttpContext.Items["User"];
            var userId = user.PersonId;

            // 検索文字列が空の場合は全Lunchカテゴリのレコードを取得し名前順に並べ替え
            if (string.IsNullOrEmpty(FindStr))
            {
                var shoplists = _context.Shoplist
                .Where(s => s.PersonId == userId && s.Status == "気になる")
                .OrderBy(s => s.ShoplistName).ToList();
                return View(shoplists);
            }
            // 検索文字列が空でない場合はShoplistの各カラムで検索文字列を含むレコードを取得
            else
            {
                // 純粋に検索文字のみを配列入れるために StringSplitOptions.RemoveEmptyEntriesを利用
                string[] keywords = FindStr.Split(new char[] { '　', ' ', ',', '、' }, StringSplitOptions.RemoveEmptyEntries);

                // Shoplistを検索してキーワードの文字いづれかが含まれるものを取得
                var shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId && s.Status == "気になる" &&
                    (keywords.Any(k => s.ShoplistName.Contains(k)) ||
                     keywords.Any(k => s.Status.Contains(k)) ||
                     keywords.Any(k => s.Category.Contains(k)) ||
                     keywords.Any(k => s.Class.Contains(k)) ||
                     keywords.Any(k => s.Coment.Contains(k)) ||
                     keywords.Any(k => s.UpdatedDate.ToString().Contains(k))))
                    .ToList();

                return View(shoplists);
            }
        }
        //  Wantページ/クイック検索のPostメソッド
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult WantFindCategory(string category)
        {
            // セッションからユーザーIDを取得
            var user = (Person)HttpContext.Items["User"];
            var userId = user.PersonId;

            // 抽出したエンティティを入れるリスト
            List<Shoplist>? shoplists;

            // 検索するキーワードのリスト
            List<string> keywords;

            switch (category)
            {
                case "デート":

                    // 検索するキーワードのリスト化
                    keywords = new List<string> { "デート", "彼氏", "彼女", "洒落", "しゃれ", "オシャレ", "雰囲気" };

                    // Statusが気になるかつキーワードにコメントの内容が含まれるものを抽出
                    shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId && s.Status == "気になる" && keywords.Any(k => s.Coment.Contains(k)))
                    .ToList();

                    return View("Want", shoplists);

                case "子連れ":

                    // 検索するキーワードのリスト化
                    keywords = new List<string> { "子供", "こども", "子ども", "キッズ", "子連れ", "子づれ" };

                    // Statusが気になるかつキーワードにコメントの内容が含まれるものを抽出
                    shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId && s.Status == "気になる" && keywords.Any(k => s.Coment.Contains(k)))
                    .ToList();
                    return View("Want", shoplists);

                case "コスパ":

                    // Statusが気になるかつコメントにコスパが含まれるまたはクラスが安価のものを抽出
                    shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId && s.Status == "気になる" &&
                    (s.Coment.Contains("コスパ") ||
                    s.Class == "安価")).ToList();

                    return View("Want", shoplists);

                case "女子":

                    // 検索するキーワードのリスト化
                    keywords = new List<string> { "女子会", "女", "友達", "ともだち", "友だち", "おしゃれ", "オシャレ", "洒落", "かわいい", "可愛い", "映え" };

                    // Statusが気になるかつキーワードにコメントの内容が含まれるものを抽出
                    shoplists = _context.Shoplist
                    .Where(s => s.PersonId == userId && s.Status == "気になる" &&
                    keywords.Any(k => s.Coment.Contains(k)))
                    .ToList();

                    return View("Want", shoplists);

                default:

                    return View("Want");
            }
        }
    }
}
