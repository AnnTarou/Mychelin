using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Mychelin.Filter
{
    // セッションチェックするためのクラス、属性を付与することでフィルターが適用される
    public class SessionCheckFilter : ActionFilterAttribute
    {
        // アクションメソッドが実行される前に実行されるメソッド
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // セッションからユーザーIDを取得
            var userId = context.HttpContext.Session.GetInt32("PersonId");

            // セッションが存在しない場合はAccount/Loginにリダイレクト
            if (!userId.HasValue)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
            }
        }
    }
}
