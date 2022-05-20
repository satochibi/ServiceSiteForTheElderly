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


public class MJsonWithStatus
{
    public string status { get; set; }
}