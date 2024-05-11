using System.ComponentModel.DataAnnotations;

namespace Mychelin.Models
{
    // ログイン画面のビューモデル
    public class LoginViewModel
    {
        [Required(ErrorMessage = "必須項目です")]
        [EmailAddress(ErrorMessage = "無効なメールアドレスです")]
        [Display(Name = "メールアドレス")]
        public string Email { get; set; }

        [Required(ErrorMessage = "必須項目です")]
        [DataType(DataType.Password)]
        [Display(Name = "パスワード")]
        public string Password { get; set; }

        [Display(Name = "ログイン状態を保持する")]
        public bool RememberMe { get; set; }
    }
}
