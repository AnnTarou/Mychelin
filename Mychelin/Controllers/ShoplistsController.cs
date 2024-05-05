using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using Mychelin.Data;
using Mychelin.Models;

namespace Mychelin.Controllers
{
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
            var mychelinContext = _context.Shoplist.Include(s => s.person);
            return View(await mychelinContext.ToListAsync());
        }

        // GET: Shoplists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoplist = await _context.Shoplist
                .Include(s => s.person)
                .FirstOrDefaultAsync(m => m.ShoplistId == id);
            if (shoplist == null)
            {
                return NotFound();
            }

            return View(shoplist);
        }

        // GET: Shoplists/Create
        public IActionResult Create()
        {
            ViewData["PersonId"] = new SelectList(_context.Person, "PersonId", "Mail");
            return View();
        }

        // POST: Shoplists/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Shoplist shoplist)
        {
            /*if (ModelState.IsValid)
            {
                ViewData["PersonId"] = new SelectList(_context.Person, "PersonId", "Mail", shoplist.PersonId);
                return View(shoplist);
            }*/

            // 画像ファイルのアップロード処理
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
            shoplist.PersonId = (int)HttpContext.Session.GetInt32("PersonId");

            // コンテキストに登録内容を追加
            _context.Add(shoplist);

            // データベースに保存
            await _context.SaveChangesAsync();

            // インデックスページにリダイレクト
            return RedirectToAction(nameof(Index));
        }

        // GET: Shoplists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoplist = await _context.Shoplist.FindAsync(id);
            if (shoplist == null)
            {
                return NotFound();
            }
            ViewData["PersonId"] = new SelectList(_context.Person, "PersonId", "Mail", shoplist.PersonId);
            return View(shoplist);
        }

        // POST: Shoplists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ShoplistId,ShoplistName,Status,Category,Class,Star,Coment,Url,ImagePath,UpdatedDate,PersonId")] Shoplist shoplist)
        {
            if (id != shoplist.ShoplistId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shoplist);
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["PersonId"] = new SelectList(_context.Person, "PersonId", "Mail", shoplist.PersonId);
            return View(shoplist);
        }

        // GET: Shoplists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shoplist = await _context.Shoplist
                .Include(s => s.person)
                .FirstOrDefaultAsync(m => m.ShoplistId == id);
            if (shoplist == null)
            {
                return NotFound();
            }

            return View(shoplist);
        }

        // POST: Shoplists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
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

        private bool ShoplistExists(int id)
        {
            return _context.Shoplist.Any(e => e.ShoplistId == id);
        }
    }
}
