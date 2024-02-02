using Mail.Model;
using Mail.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Mail.ViewModels;

public class AccountsViewModel : BindableBase
{
    // all accounts
    private ObservableCollection<Account> accounts = [];
    public ObservableCollection<Account> Accounts
    {
        get => accounts;
        set => SetProperty(ref accounts, value);
    }

    // selected item
    private Account selectedItem = new();
    public Account SelectedItem
    {
        get => selectedItem;
        set
        {
            if (null != value)
            {
                Name = selectedItem.Name;
                EmailAddress = selectedItem.EmailAddress;
                Password = selectedItem.Password;
                Smtp = selectedItem.Smtp;
                Port = selectedItem.Port.ToString();
                EnableSSL = selectedItem.EnableSSL;

                SetProperty(ref selectedItem, value);
                ControlsAreEnabled = true;
            }
            else
            {
                ControlsAreEnabled = false;
            }

            ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
        }
    }

    // controls enabled
    private bool controlsAreEnabled = false;
    public bool ControlsAreEnabled
    {
        get => controlsAreEnabled;
        set => SetProperty(ref controlsAreEnabled, value);
    }

    // actual selected account
    private string name = string.Empty;

    public string Name
    {
        get => name;
        set
        {
            SetProperty(ref name, value, nameof(Name));
            ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
        }
    }

    private string emailAddress = string.Empty;
    public string EmailAddress
    {
        get => emailAddress;
        set
        {
            if (value.Contains('@') && _emailServerCacheService.TryGet(value.Split('@')[1], out var emailServerCache))
            {
                Smtp = emailServerCache!.SmtpServer;
                Port = "0";// emailServerCache!.Port.ToString();
            }

            SetProperty(ref emailAddress, value);
        }
    }

    private string password = string.Empty;
    public string Password
    {
        get => password;
        set => SetProperty(ref password, value);
    }

    private string smtp = string.Empty;
    public string Smtp
    {
        get => smtp;
        set
        {
            IsGmailInfoBarOpen = value == "smtp.gmail.com";
            SetProperty(ref smtp, value);
        }
    }

    private string port = string.Empty;
    public string Port
    {
        get => port;
        set
        {
            if (uint.TryParse(value, out _))
                SetProperty(ref port, value);
        }
    }

    private bool enableSSL = true;
    public bool EnableSSL
    {
        get => enableSSL;
        set => SetProperty(ref enableSSL, value);
    }

    // gmail infobar
    private bool isGmailInfoBarOpen = false;
    public bool IsGmailInfoBarOpen
    {
        get => isGmailInfoBarOpen;
        set => SetProperty(ref isGmailInfoBarOpen, value);
    }

    // commands
    public ICommand AddCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }

    private readonly DialogService _dialogService;
    private readonly AccountsService _accountsService;
    private readonly EmailServerCacheService _emailServerCacheService;

    public AccountsViewModel(DialogService dialogService, AccountsService accountsService, EmailServerCacheService emailServerCacheService)
    {
        _dialogService = dialogService;
        _accountsService = accountsService;
        _emailServerCacheService = emailServerCacheService;

        accounts = new ObservableCollection<Account>(_accountsService.Values);

        AddCommand = new RelayCommand(AddAccount);
        SaveCommand = new RelayCommand(SaveAccount, CanSaveAccount);
        DeleteCommand = new RelayCommand(DeleteAccount);
    }

    private void AddAccount()
    {
        var newAccount = new Account();

        accounts.Add(newAccount);
        SelectedItem = newAccount;
    }

    private async void SaveAccount()
    {
        // remove the old element
        accounts.Remove(SelectedItem);

        // and create new
        var newAccount = new Account()
        {
            Name = name,
            EmailAddress = emailAddress,
            Password = password,
            Smtp = smtp,
            Port = uint.Parse(port),
            EnableSSL = enableSSL
        };

        accounts.Add(newAccount);

        await UpdateData();
    }

    private bool CanSaveAccount() => null != selectedItem && controlsAreEnabled && !string.IsNullOrWhiteSpace(name);

    private async void DeleteAccount()
    {
        accounts.Remove(selectedItem);
        await UpdateData();
    }

    private async Task UpdateData()
    {
        try
        {
            await _accountsService.OverwriteAndSaveAsync([.. accounts]);
        }
        catch (Exception ex)
        {
            await _dialogService.ShowAccountSaveFailureDialog(ex.Message);
        }
    }
}