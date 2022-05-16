public class MBooks
{
    public int id { get; set; }

    public string title { get; set; }   // 会員番号 12文字(年月日番号　○○○○年○○月○○日○○○○) 顧客番号 12文字

    public string description { get; set; }

}


public class MCategores
{
    //public int Id { get; set; }
    //public bool IsContact { get; set; }

    public string Name { get; set; }

    public string Link { get; set; }
}


public class MCustomers
{
    public int Id { get; set; }

    public string Name { get; set; }   // 会員番号 12文字(年月日番号　○○○○年○○月○○日○○○○) 顧客番号 12文字

    public string Furigana { get; set; }

    public string Tel { get; set; }

    public string Mail { get; set; }

    public string Postcode { get; set; }

    public string Address { get; set; }
    public string Password { get; set; }

}


public class MJsonWithStatus
{
    public string status { get; set; }
}