using BaseXamarin.Page;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseXamarin.Application
{
    public class BaseApplication : Xamarin.Forms.Application
    {
        public new static BaseApplication Current => (BaseApplication)Xamarin.Forms.Application.Current;

        protected override async void OnResume()
        {
            if (MainPage is BasePage page)
            {
                await page.OnResume();
                return;
            }
        }

        protected override void OnAppLinkRequestReceived(Uri uri)
        {
            if (MainPage is BasePage page)
            {
                page.OnAppLinkRequestReceived(uri);
            }

            base.OnAppLinkRequestReceived(uri);
        }

    }
}
