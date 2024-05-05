using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Mychelin.Models
{
    public class Shoplist
    {
        // 主キー
        public int ShoplistId { get; set; }

        // お店の名前
        [Display(Name = "店舗名")]
        [Required(ErrorMessage = "必須項目です")]
        public string ShoplistName { get; set; }

        // 行ったことがあるかどうか
        [Display(Name = "行った？")]
        public string Status { get; set; }

        // カテゴリ
        [Display(Name = "カテゴリ")]
        public string Category { get; set; }

        // 価格帯
        [Display(Name = "価格帯")]
        public string Class { get; set; }

        // おすすめ度
        [Display(Name = "おすすめ度")]
        public int Star { get; set; }

        // 自由コメント
        [Display(Name = "コメント")]
        [DataType(DataType.MultilineText)]
        public string Coment { get; set; }

        // お店のURL
        [Display(Name = "店舗URL")]
        [DataType(DataType.Url)]
        public string Url { get; set; }

        // 画像イメージのプロパティ
        [Display(Name = "画像")]
        // データベースにはマッピングしない
        [NotMapped]
        public IFormFile ImageFile { get; set; }

        // 画像イメージのパス
        [Display(Name = "画像")]
        public string ImagePath { get; set; }

        //　更新日
        [Display(Name = "更新日")]
        public DateTime UpdatedDate { get; set; }

        // 外部キーの設定
        public int PersonId { get; set; }

        // 会員情報を保持するナビゲーションプロパティ
        public Person person { get; set; }
    }
}
