namespace Mail.Model;

public class Contact
{
    public string Name { get; set; } = "Unnamed Contact";
    public string EmailAddress { get; set; } = string.Empty;

    public Contact() { }

    public Contact(string name, string emailAddress)
    {
        Name = name;
        EmailAddress = emailAddress;
    }
}