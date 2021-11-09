using BaseXamarin.Services.Dialog;
using BaseXamarin.Services.Navigation;
using BaseXamarin.Services.Storage;
using BaseXamarin.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseXamarin.IoC
{
    public class Container
    {

        private static IServiceCollection Services { get; set; }
        private IServiceProvider ServiceProvider { get; set; }

        internal Dictionary<Type, Type> Mappings;

        static readonly Lazy<Container> LazyContainer = new Lazy<Container>(() => new Container());
        public static Container Current => LazyContainer.Value;

        public Container()
        {
            Services = new ServiceCollection();

            Services.AddSingleton<INavigationService, NavigationService>();
            Services.AddSingleton<IDialogService, DialogService>();
            Services.AddSingleton<IStorageService, StorageService>();

            Mappings = new Dictionary<Type, Type>();

        }

        public void Register<TInterface, TImplementation>(LifeTime lifeTime)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            switch (lifeTime)
            {
                case LifeTime.Scoped:
                    Services.AddScoped<TInterface, TImplementation>();
                    break;
                case LifeTime.Singleton:
                    Services.AddSingleton<TInterface, TImplementation>();
                    break;
                case LifeTime.Transient:
                    Services.AddTransient<TInterface, TImplementation>();
                    break;
            }
        }

        public void Register<T>(LifeTime lifeTime) where T : class
        {
            switch (lifeTime)
            {
                case LifeTime.Scoped:
                    Services.AddScoped<T>();
                    break;
                case LifeTime.Singleton:
                    Services.AddSingleton<T>();
                    break;
                case LifeTime.Transient:
                    Services.AddTransient<T>();
                    break;
            }
        }


        public T Resolve<T>() => ServiceProvider.GetService<T>();
        public object Resolve(Type type) => ServiceProvider.GetService(type);


        public void RegisterForNavigation<TView, TViewModel>()
            where TView : Xamarin.Forms.Page
            where TViewModel : BaseViewModel
        {
            Mappings.Add(typeof(TViewModel), typeof(TView));

            Services.AddTransient<TViewModel>();
        }


        public void Setup()
        {
            ServiceProvider = null;
            ServiceProvider = Services.BuildServiceProvider();
        }

    }

    public enum LifeTime
    {
        Scoped,
        Singleton,
        Transient
    }
}
