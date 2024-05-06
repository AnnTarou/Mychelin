using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using Mychelin.Data;
using Mychelin.Models;
using Mychelin.Filter;

namespace Mychelin.Controllers
{
    // セッションチェックすフィルターをクラスに適用
    [SessionCheckFilter]
    public class ShoplistsController : Controller
    {
        // DBコンテキストを受け取るためのプロパティ
        private readonly MychelinContext _context;

        // アプリの環境情報を取得するためのプロパティ
        // ファイルのアップロードに使用
        private readonly IHostEnvironment _hostEnvironment;

        // 店舗新規登録画面「行った」の項目リスト
        public enum Status
        {
            行った,
            気になる
        }

        // 店舗新規登録画面「カテゴリ」の項目リスト
        public enum Category
        {
            ランチ,
            ディナー,
            スイーツ,
            Bar,
            おもたせ,
            その他
        }

        // 店舗新規登録画面「価格帯」の項目リスト
        public enum Class
        {
            安価,
            お手頃,
            高級,
            不明
        }
        // コンストラクター
        public ShoplistsController(MychelinContext context, IHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;

        }

        // GET: Shoplists
        public async Task<IActionResult> Index()
        {
            // セッションからユーザーIDを取得
            var userId = HttpContext.Session.GetInt32("PersonId");
            
           // ユーザーIDに関連するShoplistのみを取得
            var shoplists = _context.Shoplist
                .Where(s => s.PersonId == userId.Value)
                .ToList();

            // ログインしているユーザーのShoplistをビューに渡す
            return View(shoplists);
        }

        // GET: Shoplists/Details
        public async Task<IActionResult> Details(int? id)
        {
            var userId = HttpContext.Session.GetInt32("PersonId");

            if (id == null)
            {
                return NotFound();
            }
            
            // データベースからShoplistとPersonオブジェクトを取得＆選択されたidデータを取得
            var shoplist = await _context.Shoplist
                .Include(s => s.person)
                .FirstOrDefaultAsync(m => m.ShoplistId == id);

            // Shoplistが存在しない、またはShoplistのPersonIdがセッションのユーザーIDと一致しない場合はNotFoundを返す
            if (shoplist == null || shoplist.PersonId != userId.Value)
            {
                return NotFound();
            }

            return View(shoplist);
        }

        // GET: Shoplists/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Shoplists/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Shoplist shoplist)
        {
            // セッションからユーザーIDを取得
            var userId = HttpContext.Session.GetInt32("PersonId");

            // モデルの状態が有効でない場合はビューを返す 
            // 値が不正でないのにfalseになる原因究明が必要
            /*
            if (!ModelState.IsValid)
            {
                return View(shoplist);
            }*/

            /// 画像ファイルのアップロード処理
            // 一意のファイル名を生成
            string uniqueFileName = null;

            // ファイルが選択されているとき
            if (shoplist.ImageFile != null)
            {
                // wwwroot/imagesフォルダへのパスを取得
                string uploadsFolder = Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot/images");

                // ファイル名をGuid(Globally Unique IDentifier)のメソッドを使用して一意にする
                uniqueFileName = Guid.NewGuid().ToString() + "_" + shoplist.ImageFile.FileName;

                // ファイルの保存先のパスを生成
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // ファイルをwwwroot/imagesフォルダに保存
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    // IFormFile型のファイルを指定されたパスにコピー
                    await shoplist.ImageFile.CopyToAsync(fileStream);
                }

                // ImagePathを指定
                shoplist.ImagePath = "/images/" + uniqueFileName;

            }
            // もしファイルが選択されていない場合は、デフォルトの画像を指定
            else
            {
                shoplist.ImagePath = "/images/mychelinlist4.jpg";
            }

            // セッションからユーザーIDを取得し、それをshoplist.PersonIdに設定
            shoplist.PersonId = (int)userId;

            // コンテキストに登録内容を追加
            _context.Add(shoplist);

            // データベースに保存
            await _context.SaveChangesAsync();

            // インデックスページにリダイレクト(GET: Shoplists)
            return RedirectToAction(nameof(Index));
        }

        // GET: Shoplists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var userId = HttpContext.Session.GetInt32("PersonId");

            // idがnullの場合はNotFoundを返す
            if (id == null)
            {
                return NotFound();
            }

            // データベースのShoplistから指定されたidのデータを取得
            var shoplist = await _context.Shoplist.FindAsync(id);

            // shoplistがnullの場合はNotFoundを返す
            if (shoplist == null)
            {
                return NotFound();
            }

            // セッションのユーザーIDとShoplistのPersonIdが一致しない場合はNotFoundを返す
            if (shoplist.PersonId != userId.Value)
            {
                return NotFound();
            }

            return View(shoplist);

        }

        // POST: Shoplists/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Shoplist shoplist)
        {
            // セッションからユーザーIDを取得
            var userId = HttpContext.Session.GetInt32("PersonId");

            // 選択されたショップリストと一致するデータが存在しない場合はNotFoundを返す
            if (id != shoplist.ShoplistId)
            {
                return NotFound();
            }

            // モデルの状態が有効でない場合はビューを返す 
            // 値が不正でないのにfalseになる原因究明が必要
            /*if (!ModelState.IsValid)
            {
                return View(shoplist);
            }*/

            // モデルの状態が有効なとき、DB更新を試みる
            // ここのif分が期待通り動かないのでModelState.IsValidをあえてfalseにしている
            if (!ModelState.IsValid)
            {
                try
                {
                    /// 画像ファイルのアップロード処理
                    // 一意のファイル名を生成
                    string uniqueFileName = null;

                    // ファイルが選択されているとき
                    if (shoplist.ImageFile != null)
                    {
                        // wwwroot/imagesフォルダへのパスを取得
                        string uploadsFolder = Path.Combine(_hostEnvironment.ContentRootPath, "wwwroot/images");

                        // ファイル名をGuid(Globally Unique IDentifier)のメソッドを使用して一意にする
                        uniqueFileName = Guid.NewGuid().ToString() + "_" + shoplist.ImageFile.FileName;

                        // ファイルの保存先のパスを生成
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        // ファイルをwwwroot/imagesフォルダに保存
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            // IFormFile型のファイルを指定されたパスにコピー
                            await shoplist.ImageFile.CopyToAsync(fileStream);
                        }

                        // ImagePathを指定
                        shoplist.ImagePath = "/images/" + uniqueFileName;

                    }
                    // もしファイルが選択されていない場合は、デフォルトの画像を指定
                    else
                    {
                        shoplist.ImagePath = "/images/mychelinlist4.jpg";
                    }

                    // セッションからユーザーIDを取得し、それをshoplist.PersonIdに設定
                    shoplist.PersonId = (int)userId;

                    // コンテキストに内容をアップデート                    
                    _context.Update(shoplist);

                    // データベースに保存
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShoplistExists(shoplist.ShoplistId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                // 編集した店舗の詳細ページにリダイレクト
                return RedirectToAction(nameof(Details), new { id = shoplist.ShoplistId });
            }
            else
            {
                return View(shoplist);
            }
        }

        // GET: Shoplists/Delete
        [SessionCheckFilter]
        public async Task<IActionResult> Delete(int? id)
        {
            var userId = HttpContext.Session.GetInt32("PersonId");

            // idがnullの場合はNotFoundを返す
            if (id == null)
            {
                return NotFound();
            }

            // データベースのShoplistから指定されたidのデータを取得
            var shoplist = await _context.Shoplist.FindAsync(id);

            // shoplistがnullの場合はNotFoundを返す
            if (shoplist == null)
            {
                return NotFound();
            }

            // セッションのユーザーIDとShoplistのPersonIdが一致しない場合はNotFoundを返す
            if (shoplist.PersonId != userId.Value)
            {
                return NotFound();
            }

            return View(shoplist);
        }

        // POST: Shoplists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [SessionCheckFilter]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shoplist = await _context.Shoplist.FindAsync(id);
            if (shoplist != null)
            {
                _context.Shoplist.Remove(shoplist);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Shoplistが存在するかどうかを確認するメソッド
        private bool ShoplistExists(int id)
        {
            return _context.Shoplist.Any(e => e.ShoplistId == id);
        }

    }
}
