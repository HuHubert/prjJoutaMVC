//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace IIIProject_travel
{
    using System;
    using System.Collections.Generic;
    
    public partial class tMember
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tMember()
        {
            this.tActivity = new HashSet<tActivity>();
        }
    
        public int f會員編號 { get; set; }
        public string f會員帳號 { get; set; }
        public string f會員密碼 { get; set; }
        public string f會員名稱 { get; set; }
        public Nullable<double> f會員評分 { get; set; }
        public string f會員電子郵件 { get; set; }
        public string f會員稱號 { get; set; }
        public string f會員大頭貼 { get; set; }
        public string f會員手機 { get; set; }
        public string f會員電話 { get; set; }
        public string f會員生日 { get; set; }
        public string f會員暱稱 { get; set; }
        public string f會員英文姓名 { get; set; }
        public string f會員性別 { get; set; }
        public string f會員興趣 { get; set; }
        public string f會員已占用時間 { get; set; }
        public string f會員自我介紹 { get; set; }
        public string f會員參加的活動編號 { get; set; }
        public string f會員發起的活動編號 { get; set; }
        public string f驗證碼 { get; set; }
        public bool isAdmin { get; set; }
        public string f會員收藏的活動編號 { get; set; }
        public Nullable<int> f會員評分人數 { get; set; }
        public Nullable<int> f會員總分 { get; set; }
        public string f重置驗證碼 { get; set; }
        public string f會員擁有的優惠券 { get; set; }
        public string f會員黑名單 { get; set; }
        public Nullable<int> f瀏覽人數 { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tActivity> tActivity { get; set; }
    }
}
