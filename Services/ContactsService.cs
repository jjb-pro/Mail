using Mail.Model;

namespace Mail.Services;

public class ContactsService : DataService<Contact>
{
    protected override string FileName => "contacts.xml";
}