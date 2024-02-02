using Mail.Services;
using Mail.ViewModels;
using Mail.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;

namespace Mail
{
    public partial class App : Application
    {
        private Window? _window;

        public IServiceProvider? Container { get; private set; }

        public App() => InitializeComponent();

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            Container = RegisterServices();

            _window = new MainWindow(Container.GetService<NavigationService>()!);
            _window.Activate();
        }

        private static ServiceProvider RegisterServices()
        {
            var services = new ServiceCollection();

            // add services
            services.AddSingleton<NavigationService>();
            services.AddSingleton<DialogService>();

            // create the email server cache service and add
            EmailServerCacheService emailServerCacheService = new();
            if (!emailServerCacheService.TryLoad())
                Console.WriteLine($"{DateTime.Now}: [WARNING] Could not load emailServerCache.xml.");

            services.AddSingleton(emailServerCacheService);

            // accounts service
            ContactsService contactsService = new();
            if (!contactsService.TryLoad())
                Console.WriteLine($"{DateTime.Now}: [WARNING] Could not load contacts.xml.");

            services.AddSingleton(contactsService);

            // accounts service
            AccountsService accountsService = new();
            if (!accountsService.TryLoad())
                Console.WriteLine($"{DateTime.Now}: [WARNING] Could not load accounts.xml.");

            services.AddSingleton(accountsService);

            services.AddTransient<SendViewModel>();
            services.AddTransient<ContactsViewModel>();
            services.AddTransient<AccountsViewModel>();

            return services.BuildServiceProvider();
        }
    }
}