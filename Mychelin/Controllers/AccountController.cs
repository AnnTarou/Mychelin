using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mychelin.Data;
using Mychelin.Models;


namespace Mychelin.Controllers
{
    public class AccountController : Controller
    {
        // DBコンテキストを受け取るためのプロパティ
        private readonly MychelinContext _context;

        // セッションを扱うためのプロパティ
        private readonly IHttpContextAccessor _httpContextAccessor;

        // コンストラクター
        public AccountController(MychelinContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;

        }

        // ログイン画面にアクセスがあったとき
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // ログインボタンが押されたとき
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                try
                {
                    // データベースからユーザーを検索
                    // 同じメールアドレスはない前提
                    // nullがあると値があると例外発生？？
                    var user = await _context.Person.SingleOrDefaultAsync(u => u.Mail == model.Email);

                    // ユーザーが見つからない、またはパスワードが一致しない場合はエラーメッセージを表示
                    if (user == null || user.Password != model.Password)
                    {
                        ModelState.AddModelError(string.Empty, "メールアドレスまたはパスワードが一致しません。");
                        return View(model);
                    }

                    // ログイン成功時にセッションにユーザー情報を保存
                    _httpContextAccessor.HttpContext.Session.SetInt32("PersonId", user.PersonId);

                    // パスワードが一致した場合はホームページにリダイレクト
                    return RedirectToAction("Index", "Shoplists");
                }
                catch (Exception ex)
                {
                    return NotFound();
                }        
            }
        }
    }
}