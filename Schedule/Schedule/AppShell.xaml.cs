using BaseXamarin.IoC;
using BaseXamarin.Services.Json;
using BaseXamarin.Services.Navigation;
using BaseXamarin.Services.Storage;
using Schedule.Models;
using Schedule.ViewModels;
using Schedule.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Schedule
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            BuildDependencies();
        }

        private void BuildDependencies()
        {
            //NavigationPage.SetHasNavigationBar(this, false);
            //Container.Current.RegisterForNavigation<HomePage, HomeViewModel>();
            //Container.Current.RegisterForNavigation<HistoryPage, HistoryViewModel>();

            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            Routing.RegisterRoute(nameof(HistoryPage), typeof(HistoryPage));
            Routing.RegisterRoute(nameof(ClassroomPage), typeof(ClassroomPage));

            //Register services for use
            Container.Current.Register<IJsonService<User>, JsonService<User>>(LifeTime.Singleton);
            Container.Current.Register<IJsonService<Course>, JsonService<Course>>(LifeTime.Singleton);

            //Configure Container
            Container.Current.Setup();
        }

        async void InitNavigation()
        {
            var navigationService = Container.Current.Resolve<INavigationService>();

            //await navigationService.InitializeAsync<AppShellViewModel>(null, true);
            //Basic Startup
            await navigationService.NavigateToAsync<HomeViewModel>();
        }
    }
}
