﻿using BaseXamarin.Services.Dialog;
using BaseXamarin.Services.Navigation;
using BaseXamarin.Services.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BaseXamarin.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected readonly IStorageService StorageService;
        protected readonly INavigationService NavigationService;
        protected readonly IDialogService DialogService;

        string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; OnPropertyChanged(); }
        }

        bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; OnPropertyChanged(); }
        }

        public BaseViewModel(string title)
        {
            Title = title;
            NavigationService = IoC.Container.Current.Resolve<INavigationService>();
            DialogService = IoC.Container.Current.Resolve<IDialogService>();
            StorageService = IoC.Container.Current.Resolve<IStorageService>();
        }

        public virtual Task LoadAsync(NavigationParameters navigationData) => Task.FromResult(false);

        public virtual Task OnNavigate(NavigationParameters navigationData) => Task.FromResult(false);

        public virtual Task ResumeASync() => Task.FromResult(false);

        public virtual void AppLinkRequestReceive(Uri uri) => Task.FromResult(false);

    }
}
