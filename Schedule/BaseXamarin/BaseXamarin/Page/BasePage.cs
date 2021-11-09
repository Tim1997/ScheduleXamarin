using BaseXamarin.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BaseXamarin.Page
{
    public abstract class BasePage : ContentPage
    {
        private BaseViewModel ViewModel => BindingContext as BaseViewModel;

        protected BasePage()
        {
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (ViewModel == null) return;
            Title = ViewModel.Title;
            ViewModel.PropertyChanged += TitlePropertyChanged;

        }

        private void TitlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(ViewModel.Title)) return;

            Title = ViewModel.Title;
        }

        public async Task OnResume()
        {
            await ViewModel.ResumeASync();
        }

        public void OnAppLinkRequestReceived(Uri uri)
        {
            ViewModel.AppLinkRequestReceive(uri);
        }
    }
}
