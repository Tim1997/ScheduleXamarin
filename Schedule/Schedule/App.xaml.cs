using BaseXamarin.IoC;
using BaseXamarin.Services.Navigation;
using Schedule.ViewModels;
using Schedule.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Schedule
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            XF.Material.Forms.Material.Init(this);
            MainPage = new AppShell();
        }
    }
}
