using Mail.Helpers;
using Mail.Model;
using Mail.Services;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Input;

namespace Mail.ViewModels;

public class SendViewModel : BindableBase
{
    // Observable Properties
    private ObservableCollection<Account> _accounts;
    public ObservableCollection<Account> Accounts
    {
        get => _accounts;
        set => SetProperty(ref _accounts, value);
    }

    private Account? _selectedAccount;
    public Account? SelectedAccount
    {
        get => _selectedAccount;
        set
        {
            SetProperty(ref _selectedAccount, value);
            ((RelayCommand)SendCommand).RaiseCanExecuteChanged();
        }
    }

    private string _messageBody = string.Empty;
    public string MessageBody
    {
        get => _messageBody;
        set => SetProperty(ref _messageBody, value);
    }

    private bool _isSending = false;
    public bool IsSending
    {
        get => _isSending;
        set
        {
            SetProperty(ref _isSending, value);
            ((RelayCommand)SendCommand).RaiseCanExecuteChanged();
        }
    }

    private string _actualRecipient = string.Empty;
    public string ActualRecipient
    {
        get => _actualRecipient;
        set
        {
            SetProperty(ref _actualRecipient, value);
            SetSuggestedRecipients();
        }
    }

    public Priority[] Priorities = Enum.GetValues(typeof(Priority)).Cast<Priority>().ToArray();

    private Priority _selectedPriority = Priority.Normal;
    public Priority SelectedPriority
    {
        get => _selectedPriority;
        set => SetProperty(ref _selectedPriority, value);
    }

    private string _subject = string.Empty;
    public string Subject
    {
        get => _subject;
        set => SetProperty(ref _subject, value);
    }

    private ObservableCollection<string> _recipients = [];
    public ObservableCollection<string> Recipients
    {
        get => _recipients;
        set
        {
            SetProperty(ref _recipients, value);
            ((RelayCommand)SendCommand).RaiseCanExecuteChanged();
        }
    }

    private Contact[]? _suggestedRecipients;
    public Contact[]? SuggesetsRecipients
    {
        get => _suggestedRecipients;
        set => SetProperty(ref _suggestedRecipients, value);
    }

    private bool _toolTipWasOpen = false;
    private bool _isToolTipOpen = false;
    public bool IsToolTipOpen
    {
        get => _isToolTipOpen;
        set => SetProperty(ref _isToolTipOpen, value);
    }

    public ICommand SendCommand { get; }
    public ICommand CancelCommand { get; }

    private readonly DialogService _dialogService;
    private readonly ContactsService _contactsService;
    private readonly AccountsService _accountsService;

    public SendViewModel(DialogService dialogService, ContactsService contactsService, AccountsService accountsService)
    {
        _dialogService = dialogService;
        _contactsService = contactsService;
        _accountsService = accountsService;

        _accounts = new(_accountsService.Values);

        SendCommand = new RelayCommand(SendMail, CanSendMail);
        CancelCommand = new RelayCommand(CancelMailSending);
    }

    private CancellationTokenSource? _cancellationTokenSource;

    private async void SendMail()
    {
        IsSending = true;

        _cancellationTokenSource = new();

        try
        {
            await MailHelper.SendMail(SelectedAccount!, [.. Recipients], Subject, SelectedPriority, MessageBody, _cancellationTokenSource.Token);
            await _dialogService.ShowEmailSendDialog("Success", "Your email has been sended.");
        }
        catch (Exception ex)
        {
            await _dialogService.ShowEmailSendDialog("Error", ex.Message);
        }
        finally
        {
            IsSending = false;
        }
    }

    private void CancelMailSending() => _cancellationTokenSource?.Cancel();

    private bool CanSendMail() => null != SelectedAccount && !IsSending && Recipients.Count > 0;

    private void SetSuggestedRecipients()
    {
        List<Contact> suitableItems = [];
        var splitText = _actualRecipient.ToLower().Split(" ");
        foreach (var recipient in _contactsService.Values)
        {
            var found = splitText.All((key) => recipient.Name.Contains(key, StringComparison.CurrentCultureIgnoreCase));
            if (found)
            {
                suitableItems.Add(recipient);
            }
        }

        if (MailHelper.IsValidEmailAddress(_actualRecipient))
            suitableItems.Add(new($"Use new contact", _actualRecipient));

        SuggesetsRecipients = [.. suitableItems];
    }

    public void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (args.SelectedItem is Contact newRecipient && !Recipients.Contains(newRecipient.EmailAddress))
        {
            Recipients.Add(newRecipient.EmailAddress);
            ((RelayCommand)SendCommand).RaiseCanExecuteChanged();

            if (Recipients.Count == 1 && !_toolTipWasOpen)
            {
                _toolTipWasOpen = true;
                IsToolTipOpen = true;
            }
        }

        sender.Text = string.Empty;
        sender.IsSuggestionListOpen = false;
    }

    public void ContentGridView_ItemClick(object sender, ItemClickEventArgs e)
    {
        Recipients.Remove((string)e.ClickedItem);
        ((RelayCommand)SendCommand).RaiseCanExecuteChanged();
    }
}