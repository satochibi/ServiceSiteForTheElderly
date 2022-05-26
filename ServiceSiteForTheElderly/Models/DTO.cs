using System;

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
    public int Price { get; set; }
}


public class MJsonWithStatus
{
    public string status { get; set; }
}