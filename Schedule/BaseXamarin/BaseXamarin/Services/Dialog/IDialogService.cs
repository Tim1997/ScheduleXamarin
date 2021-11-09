using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BaseXamarin.Services.Dialog
{
    public interface IDialogService
    {
        Task<string> ActionSheetAsync(string title, string cancel, string destruction, params string[] buttons);
        Task AlertAsync(string title, string message, string cancel);
        Task<bool> AlertAsync(string title, string message, string accept, string cancel);
    }
}
