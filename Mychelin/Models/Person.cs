using System.ComponentModel.DataAnnotations;

namespace Mychelin.Models
{
    public class Person
    {
        public int PersonId { get; set; }

        [Display(Name = "ニックネーム")]
        [Required(ErrorMessage = "必須項目です")]
        public string PersonName { get; set; }

        [Display(Name = "メールアドレス")]
        [Required(ErrorMessage = "必須項目です")]
        [DataType(DataType.EmailAddress)]
        public string Mail { get; set; }

        [Display(Name = "パスワード")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // Shoplistをリストで保持
        public List<Shoplist> Shoplists { get; set; }
    }
}
