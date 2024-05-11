using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mychelin.Data;
using Mychelin.Models;

namespace Mychelin.Controllers
{
    public class PeopleController : Controller
    {
        // DBコンテキストを受け取るためのプロパティ
        private readonly MychelinContext _context;

        // コンストラクター
        public PeopleController(MychelinContext context)
        {
            _context = context;
        }

        // Create以外のメソッドにセッションチェックの[SessionCheckFilter]属性を付与

        // GET: People/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: People/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Person person)
        {
            // ModelState.IsValidが通常はtrueのときの処理
            // 現在false（原因不明）となるためこの実装
            if (!ModelState.IsValid)
            {
                // パスワードハッシュ化クラスのインスタンス生成
                var hasher = new PasswordHasher<IdentityUser>();

                // ハッシュ化されたパスワードの生成
                var hashedPassword = hasher.HashPassword(null, person.Password);

                // ハッシュ化されたパスワードをパスワードに設定
                person.Password = hashedPassword;

                // ソルトは現在の実装では使用しないため仮にメールアドレスを入力
                person.Salt = person.Mail;

                // コンテキストに入力されたpersonを登録
                _context.Add(person);

                // データベースの更新
                await _context.SaveChangesAsync();

                // DB更新成功したらログインページへリダイレクト
                return RedirectToAction("Login", "Account");
            }
            else
            {
                return View(person);
            }            
        }

        // GET: People/Details/5
        // セッションチェックすフィルター適用
        [SessionCheckFilter]
        public async Task<IActionResult> Details()
        {
            // セッションからユーザーIDを取得
            var user = (Person)HttpContext.Items["User"];
            var userId = user.PersonId;

            // パスにidが含まれないときNotFoundページを表示   
            if (userId == null)
            {
                return NotFound();
            }

            // データベースからGETしたidに一致するものをpersonへ代入
            var person = await _context.Person
                .FirstOrDefaultAsync(m => m.PersonId == userId);

            // personがないときNotFoundページを表示
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // GET: People/Edit
        // セッションチェックすフィルター適用
        [SessionCheckFilter]
        public async Task<IActionResult> Edit()
        {
            // セッションからユーザーIDを取得
            var user = (Person)HttpContext.Items["User"];
            var userId = user.PersonId;

            // パスにidが含まれないときNotFoundページを表示   
            if (userId == null)
            {
                return NotFound();
            }

            // データベースからGETしたidに一致するものをpersonへ代入
            var person = await _context.Person
                .FirstOrDefaultAsync(m => m.PersonId == userId);

            // personがないときNotFoundページを表示
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // POST: People/Edit/5
        [HttpPost]
        [SessionCheckFilter]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Person person)
        {
            // セッションからユーザーIDを取得
            var user = (Person)HttpContext.Items["User"];
            var userId = user.PersonId;

            if (userId != person.PersonId)
            {
                return NotFound();
            }

            // あえてModelState.IsValid　falseのときにしている
            if (!ModelState.IsValid)
            {
                try
                {
                    // パスワードハッシュ化クラスのインスタンス生成
                    var hasher = new PasswordHasher<IdentityUser>();

                    // ハッシュ化されたパスワードの生成
                    var hashedPassword = hasher.HashPassword(null, person.Password);

                    // ハッシュ化されたパスワードをパスワードに設定
                    person.Password = hashedPassword;

                    // 現在の実装ではSalt利用していないためメールアドレスを入力
                    person.Salt = person.Mail;

                    // コンテキストに変更を登録
                    _context.Update(person);

                    // データベースの更新
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonExists(person.PersonId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                // DB更新成功したらアカウント詳細ページにリダイレクト
                return RedirectToAction("Details", "People");
            }
            return View(person);
        }

        // GET: People/Delete/5
        [SessionCheckFilter]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Person
                .FirstOrDefaultAsync(m => m.PersonId == id);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // POST: People/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [SessionCheckFilter]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var person = await _context.Person.FindAsync(id);

            // レコードがあったら
            if (person != null)
            {
                // ユーザー固有のShoplistのデータベースを取得
                var dbShoplists = await _context.Shoplist.Where(s => s.PersonId == id).ToListAsync();

                // 関連するShoplistsのレコードを削除
                _context.Shoplist.RemoveRange(dbShoplists);

                // DBからアカウント削除
                _context.Person.Remove(person);
            }

            // DB更新
            await _context.SaveChangesAsync();

            /// 削除が成功したらセッションとクッキーを削除してホームへリダイレクト

            // セッションをクリア
            HttpContext.Session.Clear();

            // クッキーを削除
            Response.Cookies.Delete("SessionId");

            // ホームへリダイレクト
            return RedirectToAction("Index", "Home");
        }

        private bool PersonExists(int id)
        {
            return _context.Person.Any(e => e.PersonId == id);
        }
    }
}
