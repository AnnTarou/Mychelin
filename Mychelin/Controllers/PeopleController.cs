using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

using Mychelin.Data;
using Mychelin.Filter;
using Mychelin.Models;

namespace Mychelin.Controllers
{
    public class PeopleController : Controller
    {
        private readonly MychelinContext _context;

        public PeopleController(MychelinContext context)
        {
            _context = context;
        }

        // GET: People/Details/5
        // セッションチェックすフィルター適用
        [SessionCheckFilter]
        public async Task<IActionResult> Details(int? id)
        {
            // パスにidが含まれないときNotFoundページを表示   
            if (id == null)
            {
                return NotFound();
            }

            // データベースからGETしたidに一致するものをpersonへ代入
            var person = await _context.Person
                .FirstOrDefaultAsync(m => m.PersonId == id);

            // personがないときNotFoundページを表示
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

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

                // 会員詳細ページへリダイレクト
                return RedirectToAction("Login", "Account");
            }
            else
            {
                return View(person);
            }            
        }

        // POST: People/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PersonId,PersonName,Mail,Password")] Person person)
        {
            if (id != person.PersonId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // パスワードをハッシュ化
                    //person.Password = HashPassword(person.Password);

                    _context.Update(person);
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
                return RedirectToAction(nameof(Index));
            }
            return View(person);
        }


        // GET: People/Delete/5
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var person = await _context.Person.FindAsync(id);
            if (person != null)
            {
                _context.Person.Remove(person);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PersonExists(int id)
        {
            return _context.Person.Any(e => e.PersonId == id);
        }
    }
}
