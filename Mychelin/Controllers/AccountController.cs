using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mychelin.Data;
using Mychelin.Models;
using Newtonsoft.Json;


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
            try
            {
                // データベースからユーザーを検索
                // 同じメールアドレスはない前提
                // nullがあると値があると例外発生？？
                var user = await _context.Person.SingleOrDefaultAsync(u => u.Mail == model.Email);

                // もしuserが見つからない場合エラーを出す
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "メールアドレスが一致しません。");
                    return View(model);
                }

                // パスワードハッシュ化クラスのインスタンス生成
                var hasher = new PasswordHasher<IdentityUser>();

                //　DBのパスワードと入力されたパスワードを検証
                var result = hasher.VerifyHashedPassword(null, user.Password, model.Password);

                // 検証結果がSuccessだったら
                if (result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded)
                {
                    // ログイン成功時にグイドで一意のセッションIDを生成
                    var sessionId = Guid.NewGuid().ToString();

                    // SessionIdをキーとして変換したsessionIdを設定
                    HttpContext.Session.SetString("SessionId", sessionId);

                    // sessionIdにユーザー情報をJson形式に変換して保存
                    HttpContext.Session.SetString(sessionId, JsonConvert.SerializeObject(user));

                    // "Remember me"がチェックされている場合、セッションIDをCookieにも保存
                    if (model.RememberMe)
                    {
                        // クッキーオプションを設定
                        var cookieOptions = new CookieOptions
                        {
                            //有効期限を1時間に設定
                            Expires = DateTime.Now.AddHours(1),
                            // GDPR対策
                            IsEssential = true
                        };

                        // クッキーにセッションIDを保存
                        Response.Cookies.Append("SessionId", sessionId, cookieOptions);
                    }

                    return RedirectToAction("Index", "Shoplists");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "パスワードが一致しません。");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        // パスワードを忘れた方はこちらにアクセスがGETアクセスがあったとき
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // ログアウトボタンが押されたとき
        [HttpPost]
        public IActionResult Logout()
        {
            // セッションをクリア
            HttpContext.Session.Clear();

            // クッキーを削除
            Response.Cookies.Delete("SessionId");

            // ホームにリダイレクト
            return RedirectToAction("Index", "Home");
        }
    }
}