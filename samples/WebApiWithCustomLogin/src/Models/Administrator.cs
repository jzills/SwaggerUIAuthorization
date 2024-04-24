namespace Sample.Models;

public class Administrator
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Title { get; set; }
    public ContactInfo? ContactInfo { get; set; }
}

public class ContactInfo
{
    public string? PhoneNumber { get; set; }
    public string? FaxNumber { get; set; }
}