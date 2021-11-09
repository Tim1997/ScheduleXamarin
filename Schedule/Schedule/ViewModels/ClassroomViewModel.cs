using BaseXamarin.Services.Json;
using BaseXamarin.Services.Navigation;
using Newtonsoft.Json;
using Schedule.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Schedule.ViewModels
{
    [QueryProperty(nameof(Parameter), nameof(Parameter))]
    public class ClassroomViewModel : BaseXamarin.ViewModels.BaseViewModel
    {
        #region Properties
        private User user;
        private string parameter;
        private string name;
        private string description;
        private string phone;
        private string address;
        private string money;
        private bool isErrorMoney, isErrorName, isErrorDescription;

        public string Parameter
        {
            get => parameter;
            set
            {
                parameter = Uri.UnescapeDataString(value ?? string.Empty);
                SetProperty(ref parameter, value);

                if (!string.IsNullOrEmpty(parameter))
                {
                    User = JsonConvert.DeserializeObject<User>(parameter);

                    Title = "Edit User";

                    Name = User.Name;
                    Address = User.Address;
                    Description = User.Description;
                    Phone = User.Phone;
                    Money = User.Money;
                }
            }
        }
        public string Name { get => name; set { SetProperty(ref name, value); IsErrorName = string.IsNullOrEmpty(name); } }
        public string Description { get => description; set { SetProperty(ref description, value); IsErrorDescription = string.IsNullOrEmpty(description); } }
        public string Phone { get => phone; set => SetProperty(ref phone, value); }
        public string Address { get => address; set => SetProperty(ref address, value); }
        public string Money { get => money; set { SetProperty(ref money, value); IsErrorMoney = string.IsNullOrEmpty(money); } }
        public User User { get => user; set => user = value; }
        public bool IsErrorMoney { get => isErrorMoney; set => SetProperty(ref isErrorMoney, value); }
        public bool IsErrorName { get => isErrorName; set => SetProperty(ref isErrorName, value); }
        public bool IsErrorDescription { get => isErrorDescription; set => SetProperty(ref isErrorDescription, value); }
        #endregion

        #region Service
        private IJsonService<User> UserService => BaseXamarin.IoC.Container.Current.Resolve<IJsonService<User>>();
        #endregion

        #region Command
        public ICommand OkCommand => new Command(async () =>
        {
            if (IsErrorMoney || IsErrorName || IsErrorDescription)
                return;

            if (User == null)
            {
                User = new User() { Id = Guid.NewGuid().ToString()};
            }

            User.Name = Name;
            User.Address = Address;
            User.Description = Description;
            User.Phone = Phone;
            User.Money = Money;

            var pathFile = BaseConstant.PATH_CLASSROOM + User.Id + $"/user.json";
            if (Title == "Add User")
            {
                await StorageService.CreateFolder(Path.GetDirectoryName(pathFile));
            }

            await UserService.CreateObjectAsync(pathFile, User);
            await Shell.Current.GoToAsync($"..?{nameof(HomeViewModel.ParameterUser)}={ JsonConvert.SerializeObject(User)}");
        });

        public ICommand CancelCommand => new Command(async () =>
        {
            await Shell.Current.GoToAsync($"..");
        });
        #endregion

        public ClassroomViewModel() : base("User")
        {
            Init();
        }

        #region Init
        private void Init()
        {
            Title = "Add User";
        }
        #endregion
    }
}
