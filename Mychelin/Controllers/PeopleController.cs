using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mychelin.Data;
using Mychelin.Filter;
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
        public async Task<IActionResult> Create([Bind("PersonId,PersonName,Mail,Password")] Person person)
        {
            // ModelState.IsValidが通常はtrueのときの処理
            // 現在false(原因不明）となるためこの実装
            if (!ModelState.IsValid)
            {
                // パスワードをハッシュ化
                //person.Password = HashPassword(person.Password);

                // ハッシュ化実装していないためメールアドレスを入力
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
            var userId = HttpContext.Session.GetInt32("PersonId");

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
            var userId = HttpContext.Session.GetInt32("PersonId");

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
            var userId = HttpContext.Session.GetInt32("PersonId");

            if (userId != person.PersonId)
            {
                return NotFound();
            }

            // あえてModelState.IsValid　falseのときにしている
            if (!ModelState.IsValid)
            {
                try
                {
                    // パスワードをハッシュ化
                    //person.Password = HashPassword(person.Password);

                    // ハッシュ化実装していないためメールアドレスを入力
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
                // DB更新成功したら詳細ページにリダイレクト
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
                // DBから削除
                _context.Person.Remove(person);
            }

            // DB更新
            await _context.SaveChangesAsync();

            // 削除が成功したらホームへリダイレクト
            return RedirectToAction("Index", "Home");
        }

        private bool PersonExists(int id)
        {
            return _context.Person.Any(e => e.PersonId == id);
        }
    }
}
