using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceSiteForTheElderly.Models.ViewModels
{
    public class CommonViewModels
    {
    }

    public class SessionModel
    {
        /// <summary>
        /// 顧客情報
        /// </summary>
        public MCustomers customerUserInfo { get; set; }
        /// <summary>
        /// カートの中身
        /// </summary>
        public List<CartModel> cartModelInfo { get; set; }
        /// <summary>
        /// 送り先情報
        /// </summary>
        public MShippingAddress shippingAddressInfo { get; set; }
        /// <summary>
        /// 注文時のランダムID
        /// </summary>
        public string randomId { get; set; } = "";
        /// <summary>
        /// お問い合わせ時の本文
        /// </summary>
        public string message { get; set; } = "";
        /// <summary>
        /// お問い合わせ時のカテゴリid
        /// </summary>
        public int? categoryId { get; set; } = null;
    }

    public class LoginModel
    {
        public string Tel { get; set; }
        public string Password { get; set; }
    }

    public class SignUpModel
    {
        public string Name { get; set; }

        public string Furigana { get; set; }

        public string Tel { get; set; }

        public string Mail { get; set; }

        public string Postcode { get; set; }

        public string Address { get; set; }

        public string Password { get; set; }

        public bool IsNotEmpty()
        {
            return this.GetType().GetProperties().Aggregate(true, (acc, x) => acc && !string.IsNullOrEmpty(x.GetValue(this).ToString()));
        }
    }


    public class CartModel
    {
        public int GoodsId { get; set; }
        public string Variety { get; set; }
        public int Quantity { get; set; }

        public override string ToString()
        {
            return $"(id={this.GoodsId}, variety={this.Variety} num={this.Quantity})";
        }
    }

    public class ShippingAddressModel
    {
        public string Name { get; set; }
        public string Furigana { get; set; }
        public string Tel { get; set; }
        public string Postcode { get; set; }
        public string Address { get; set; }
    }

    public class MessageModel
    {
        public string Message { get; set; }
        public int CategoryId { get; set; }
    }

}