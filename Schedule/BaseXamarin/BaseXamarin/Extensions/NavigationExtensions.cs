using BaseXamarin.Services.Navigation;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace BaseXamarin.Extensions
{
    public static class NavigationExtensions
    {
        private static ConditionalWeakTable<Xamarin.Forms.Page, NavigationParameters> arguments = new ConditionalWeakTable<Xamarin.Forms.Page, NavigationParameters>();

        public static IDictionary<string, object> NavigationArgs(this Xamarin.Forms.Page page)
        {
            NavigationParameters parameters = null;
            arguments.TryGetValue(page, out parameters);
            return parameters;
        }

        public static void AddNavigationArgs(this Xamarin.Forms.Page page, NavigationParameters parameters)
        {
            arguments.Add(page, parameters);
        }

        public static void RemoveNavigationArgs(this Xamarin.Forms.Page page)
        {
            arguments.Remove(page);
        }
    }
}
