using Mail.Model;
using Mail.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Mail.ViewModels;

public class ContactsViewModel : BindableBase
{
    // all accounts
    private ObservableCollection<Contact> contacts = [];
    public ObservableCollection<Contact> Contacts
    {
        get => contacts;
        set => SetProperty(ref contacts, value);
    }

    // selected item
    private Contact selectedItem = new();
    public Contact SelectedItem
    {
        get => selectedItem;
        set
        {
            if (null != value)
            {
                Name = selectedItem.Name;
                EmailAddress = selectedItem.EmailAddress;

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
            SetProperty(ref name, value);
            ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
        }
    }

    private string emailAddress = string.Empty;
    public string EmailAddress
    {
        get => emailAddress;
        set => SetProperty(ref emailAddress, value);
    }

    // commands
    public ICommand AddCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }

    private readonly DialogService _dialogService;
    private readonly ContactsService _contactsService;

    public ContactsViewModel(DialogService dialogService, ContactsService contactsService)
    {
        _dialogService = dialogService;
        _contactsService = contactsService;

        contacts = new(_contactsService.Values);

        AddCommand = new RelayCommand(AddContact);
        SaveCommand = new RelayCommand(SaveContact, CanSaveContact);
        DeleteCommand = new RelayCommand(DeleteContact);
    }

    private void AddContact()
    {
        var newContact = new Contact();

        contacts.Add(newContact);
        SelectedItem = newContact;
    }

    private async void SaveContact()
    {
        // remove the old element
        contacts.Remove(SelectedItem);

        // and create new
        var newContact = new Contact(name, emailAddress);

        contacts.Add(newContact);

        await UpdateData();
    }

    private bool CanSaveContact() => null != selectedItem && controlsAreEnabled && !string.IsNullOrWhiteSpace(name);

    private async void DeleteContact()
    {
        contacts.Remove(selectedItem);
        await UpdateData();
    }

    private async Task UpdateData()
    {
        try
        {
            await _contactsService.OverwriteAndSaveAsync([.. contacts]);
        }
        catch (Exception ex)
        {
            await _dialogService.ShowAccountSaveFailureDialog(ex.Message);
        }
    }

    //private void SearchItems()
    //{
    //    items.Clear();

    //    foreach (var student in allItems)
    //    {
    //        if (string.IsNullOrWhiteSpace(searchQuery) || student.GetParameterAsString(selectedFilter).ToLower().Contains(searchQuery.ToLower()))
    //            items.Add(student);
    //    }
    //}

    //// Handle selection
    //public void ListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
    //{
    //    var listView = (ListView)sender;
    //    ObservableCollection<Student> newSelectedItems = new();

    //    foreach (object item in listView.SelectedItems)
    //        newSelectedItems.Add((Student)item);

    //    selectedItems = newSelectedItems;

    //    ((RelayCommand)DeleteCommand).RaiseCanExecuteChanged();
    //}

    //// Handle add
    //public ICommand AddCommand { get; set; }

    //private int additionalItemCount = 0;
    //private void AddItem()
    //{
    //    var newItem = new Student
    //    {
    //        Name = new Name($"Student{additionalItemCount}", "Test")
    //    };

    //    allItems.Add(newItem);
    //    Items.Add(newItem);
    //    additionalItemCount++;
    //}

    //// Handle add to export
    //public ICommand AddToExportCommand { get; set; }

    //private void AddToExport()
    //{
    //    foreach (Student student in selectedItems)
    //        _dataService.StudentsToExport.Add(student);
    //}

    //// Handle edit
    //public ICommand EditCommand { get; set; }

    //public void EditItem()
    //{
    //    _navigationService.Navigate(typeof(AddEditPage), new Microsoft.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo(), selectedItem);
    //}

    //public void ListViewDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    //{
    //    EditItem();
    //}

    //// Handle delete
    //public ICommand DeleteCommand { get; set; }

    //public void DeleteItem()
    //{
    //    foreach (Student student in selectedItems)
    //    {
    //        allItems.Remove(student);
    //        items.Remove(student);
    //    }
    //}

    //private bool CanEdit() => null != selectedItem;

    //private bool ItemsAreSelected() => selectedItems.Count > 0;
}