using System;
using System.Collections.Generic;

public class MCategores
{
    public int Id { get; set; }

    public bool IsContact { get; set; }

    public string Name { get; set; }

    public string Link { get; set; }
}


public class MCustomers
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Furigana { get; set; }

    public string Tel { get; set; }

    public string Mail { get; set; }

    public string Postcode { get; set; }

    public string Address { get; set; }
    public string Password { get; set; }

}


public class MGoods
{
    public int Id { get; set; }
    public int? OrderOfPublication { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Picture { get; set; }
    public int ShopId { get; set; }
    public string Publisher { get; set; }
    public string Author { get; set; }
    public DateTime PublicationStartDate { get; set; }
    public DateTime PublicationEndDate { get; set; }
    public List<MPrice> Price { get; set; }
    public string ShopName { get; set; }
    public string ShopGenre { get; set; }
}


public class MShops
{
    public int Id { get; set; }
    public string DisplayName { get; set; }
    public int? ShippingCost { get; set; }
    public int CategoryId { get; set; }
    public int CompanyId { get; set; }
    public string Picture { get; set; }
    public string Genre { get; set; }
}

public class MPrice
{
    public string Variety { get; set; }
    public int Price { get; set; }
}


public class MGoodsOfCart
{
    public int GoodsId { get; set; }
    public int Quantity { get; set; }
    public string Variety { get; set; }
    public int Price { get; set; }
    public string Name { get; set; }
    public string Picture { get; set; }
    public int ShopId { get; set; }
    public DateTime PublicationStartDate { get; set; }
    public DateTime PublicationEndDate { get; set; }
    public string ShopName { get; set; }
    public string ShopGenre { get; set; }
    public int ShippingCost { get; set; }
}


public class MShippingAddress
{ 
    public string Name { get; set; }
    public string Furigana { get; set; }
    public string Tel { get; set; }
    public string Postcode { get; set; }
    public string Address { get; set; }
}


public class MJsonWithStatus
{
    public string status { get; set; }
}