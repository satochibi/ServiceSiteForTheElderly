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
        public MCustomers customerUserInfo { get; set; }
        public List<CartModel> cartModelInfo { get; set; }
        public MShippingAddress shippingAddressInfo { get; set; }
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

}