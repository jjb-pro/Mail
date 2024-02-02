using Mail.Model;

namespace Mail.Services;

public class AccountsService : DataService<Account>
{
    protected override string FileName => "accounts.xml";
}