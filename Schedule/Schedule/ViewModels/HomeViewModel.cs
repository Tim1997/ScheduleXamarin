using BaseXamarin.Services.Json;
using Newtonsoft.Json;
using Schedule.Helpers;
using Schedule.Models;
using Schedule.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Plugin.Calendar.Models;
using XF.Material.Forms.UI.Dialogs;

namespace Schedule.ViewModels
{
    [QueryProperty(nameof(ParameterUser), nameof(ParameterUser))]
    public class HomeViewModel : BaseXamarin.ViewModels.BaseViewModel
    {
        #region Properties
        private ObservableCollection<User> users;
        private User selectedUser;
        private DateTime _monthYear = DateTime.Today;
        private DateTime? _selectedDate = DateTime.Today;
        private CultureInfo _culture = CultureInfo.InvariantCulture;
        private IEnumerable<CalenderEvent> GenerateEvents(int count, string name)
        {
            return Enumerable.Range(1, count).Select(x => new CalenderEvent
            {
                Name = $"{name} event{x}",
                Description = $"This is {name} event{x}'s description!",
                Starting = new DateTime(2000, 1, 1, (x * 2) % 24, (x * 3) % 60, 0)
            });
        }
        private EventCollection events;
        private string parameterUser;
        private Color Payment = Color.Green, UnPayment = Color.CadetBlue;

        public DateTime MonthYear
        {
            get => _monthYear;
            set => SetProperty(ref _monthYear, value);
        }
        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set => SetProperty(ref _selectedDate, value);
        }
        public CultureInfo Culture
        {
            get => _culture;
            set => SetProperty(ref _culture, value);
        }
        public ObservableCollection<User> Users { get => users; set => SetProperty(ref users, value); }
        public User SelectedUser
        {
            get => selectedUser;
            set
            {
                SetProperty(ref selectedUser, value);
                if (selectedUser != null)
                {
                    selectedUser.Background = "background.png";


                    new Thread(async () =>
                    await ExecuteAddEventAsync(new DateTime(MonthYear.Year, MonthYear.Month, 1))).Start();
                }
                else
                {
                    Events?.Clear();
                }
            }
        }
        public string ParameterUser
        {
            get => parameterUser;
            set
            {
                parameterUser = Uri.UnescapeDataString(value ?? string.Empty);
                SetProperty(ref parameterUser, value);

                if (!string.IsNullOrEmpty(parameterUser))
                    BackNavigation(JsonConvert.DeserializeObject<User>(parameterUser));
            }
        }
        #endregion

        #region Command 
        public ICommand CopyCommand => new Command(async () =>
        {
            var result = $"{selectedUser?.Name}\nBuổi: ";
            foreach (var eve in Events)
            {
                result += eve.Key.Day + ", ";
            }
            result = result.Substring(0, result.Length - 2);

            result += "\nTổng tiền: " + (Events.Count * int.Parse(SelectedUser.Money)).ToMoney() + "VND";
            await Clipboard.SetTextAsync(result);
            await MaterialDialog.Instance.SnackbarAsync(message: "Copy to clipboard",
                                            msDuration: MaterialSnackbar.DurationShort);
        });
        public ICommand PageAppearingCommand => new Command(async () =>
        {
            await LoadItemAsync();
        });
        public ICommand RemoveCommand => new Command(async () =>
        {
            var isOk = await App.Current.MainPage.DisplayAlert("Remove", "Are you sure you want to delete?", "Ok", "Cancel");

            if (isOk && SelectedUser != null)
            {
                var pUser = BaseConstant.PATH_CLASSROOM + SelectedUser.Id;
                var x = await StorageService.DeleteFolder(pUser);

                Users.Remove(SelectedUser);
            }
        });
        public ICommand EditCommand => new Command(async () =>
        {
            var json = JsonConvert.SerializeObject(SelectedUser);
            await Shell.Current.GoToAsync($"{nameof(ClassroomPage)}?{nameof(ClassroomViewModel.Parameter)}={json}");
        });
        public ICommand AddCommand => new Command(async () =>
        {
            await Shell.Current.GoToAsync(nameof(ClassroomPage));
        });
        public ICommand MonthExtentionCommand => new Command(async () => await ExecuteAddEventAsync(new DateTime(MonthYear.Year, MonthYear.Month, 1)));
        public ICommand PaymentCommand => new Command(async () => await ExecutePaymentCommandAsync());
        public ICommand DayTappedCommand => new Command<DateTime>(async (date) => await DayTapped(date));
        public ICommand SwipeLeftCommand => new Command(() => { MonthYear = MonthYear.AddMonths(2); });
        public ICommand SwipeRightCommand => new Command(() => { MonthYear = MonthYear.AddMonths(-2); });
        public ICommand SwipeUpCommand => new Command(() => { MonthYear = DateTime.Today; });
        public ICommand EventSelectedCommand => new Command(async (item) => await ExecuteEventSelectedCommand(item));
        public ICommand SwipeCardCommand => new Command(() =>
        {
            if (Users.Count <= 1) return;

            foreach (var item in Users)
                item.Background = null;
        });
        public EventCollection Events { get => events; set => SetProperty(ref events, value); }
        #endregion

        #region Service
        private IJsonService<User> UserService => BaseXamarin.IoC.Container.Current.Resolve<IJsonService<User>>();
        private IJsonService<Course> CourseService => BaseXamarin.IoC.Container.Current.Resolve<IJsonService<Course>>();
        #endregion

        public HomeViewModel() : base("Home")
        {
            //////////////////////////////////
            Init();
            InitClass();
            ///////////////////////////////////
        }

        #region init
        private void Init()
        {
            Users = new ObservableCollection<User>();
            Culture = CultureInfo.CreateSpecificCulture("en-GB");

            #region Event Calender
            Events = new EventCollection();
            SelectedDate = DateTime.Today;

            //Task.Delay(5000).ContinueWith(_ =>
            //{
            //    //// indexer - update later
            //    //Events[DateTime.Now] = new ObservableCollection<CalenderEvent>(GenerateEvents(10, "Cool"));

            //    //// add later
            //    //Events.Add(DateTime.Now.AddDays(3), new List<CalenderEvent>(GenerateEvents(5, "Cool")));

            //    //// indexer later
            //    //Events[DateTime.Now.AddDays(10)] = new List<CalenderEvent>(GenerateEvents(10, "Boring"));

            //    //// add later
            //    //Events.Add(DateTime.Now.AddDays(15), new List<CalenderEvent>(GenerateEvents(10, "Cool")));


            //    //Task.Delay(3000).ContinueWith(t =>
            //    //{
            //    //    MonthYear = MonthYear.AddMonths(-2);

            //    //    // get observable collection later
            //    //    var todayEvents = Events[DateTime.Now] as ObservableCollection<CalenderEvent>;

            //    //    // insert/add items to observable collection
            //    //    todayEvents.Insert(0, new CalenderEvent { Name = "Cool event insert", Description = "This is Cool event's description!", Starting = new DateTime() });
            //    //    todayEvents.Add(new CalenderEvent { Name = "Cool event add", Description = "This is Cool event's description!", Starting = new DateTime() });

            //    //}, TaskScheduler.FromCurrentSynchronizationContext());
            //}, TaskScheduler.FromCurrentSynchronizationContext());
            #endregion
        }

        private void InitClass()
        {
            #region Class
            //Classrooms.Add(new Classroom
            //{
            //    Id = Guid.NewGuid().ToString(),
            //    Name = "Tiêu",
            //    Address = "Cầu giấy, Hà nội",
            //    Description = "Toán 12, B, KTQD, Khá",
            //    Money = 150000.ToMoney(),
            //    Phone = "098765432",
            //    Sessions = new Dictionary<DateTime, Course>
            //    {
            //        {new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), new Course
            //        {
            //            Time = new List<DateTime>()
            //            {
            //                new DateTime(DateTime.Now.Year, DateTime.Now.Month, 3),
            //                new DateTime(DateTime.Now.Year, DateTime.Now.Month, 4),
            //                new DateTime(DateTime.Now.Year, DateTime.Now.Month, 5)
            //            }, IsPayment = false
            //        }},

            //        {new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1), new Course
            //        {
            //            Time = new List<DateTime>()
            //            {
            //                new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1),
            //                new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 2),
            //                new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 9)
            //            }, IsPayment = false
            //        }},
            //    },
            //    TotalMoney = (5 * 150000).ToMoney(),
            //});
            //Classrooms.Add(new Classroom
            //{
            //    Id = Guid.NewGuid().ToString(),
            //    Name = "Khoa",
            //    Address = "Hai bà trưng, Hà nội",
            //    Description = "Toán 6, Chuyên, Giỏi",
            //    Money = 120000.ToMoney(),
            //    Phone = "012345678",
            //    Sessions = new Dictionary<DateTime, Course>
            //    {
            //        {new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), new Course
            //        {
            //            Time = new List<DateTime>()
            //            {
            //                new DateTime(DateTime.Now.Year, DateTime.Now.Month, 13),
            //                new DateTime(DateTime.Now.Year, DateTime.Now.Month, 14),
            //                new DateTime(DateTime.Now.Year, DateTime.Now.Month, 15)
            //            }, IsPayment = false
            //        }},
            //    },
            //    TotalMoney = (5 * 120000).ToMoney(),
            //});
            #endregion
        }
        #endregion

        #region Method
        private async Task DayTapped(DateTime d)
        {
            var date = new DateTime(d.Year, d.Month, d.Day);
            try
            {
                IsBusy = true;

                var pYear = BaseConstant.PATH_CLASSROOM + SelectedUser.Id + $"/{date.Year}.json";
                var pUser = BaseConstant.PATH_CLASSROOM + SelectedUser.Id + $"/user.json";

                var isExistFileYear = await StorageService.IsFileExistAsync(pYear);
                if (!isExistFileYear)
                {
                    await CourseService.CreateItemAsync(pYear);
                }

                var isExistDay = Events.Any(x => x.Key == date);
                if (isExistDay)
                {
                    // update course
                    var mCourse = await CourseService.GetItemAsync(date.Month.ToString(), pYear);
                    mCourse.Time.Remove(date);
                    await CourseService.UpdateItemAsync(mCourse, pYear);
                    Events.Remove(date);

                    // update user
                    SelectedUser.DebtMoney -= int.Parse(SelectedUser.Money);
                    if (SelectedUser.DebtMoney <= 0) SelectedUser.DebtMoney = 0;

                    await UserService.CreateObjectAsync(pUser, SelectedUser);
                }
                else
                {
                    // update course
                    var mCourse = await CourseService.GetItemAsync(date.Month.ToString(), pYear);
                    if (mCourse == null)
                    {
                        mCourse = new Course
                        {
                            Id = date.Month.ToString(),
                            IsPayment = false,
                            Time = new List<DateTime> { date },
                        };
                        await CourseService.AddItemAsync(mCourse, pYear);
                    }
                    else
                    {
                        mCourse.Time.Add(date);
                        await CourseService.UpdateItemAsync(mCourse, pYear);
                    }

                    var color = mCourse.IsPayment ? Payment : UnPayment;
                    Events.Add(date, new DayEventCollection<CalenderEvent>(color, color));

                    // update user
                    SelectedUser.DebtMoney += int.Parse(SelectedUser.Money);
                    await UserService.CreateObjectAsync(pUser, SelectedUser);
                }

            }
            catch (Exception) { }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ExecuteEventSelectedCommand(object item)
        {
            if (item is CalenderEvent eventModel)
            {
                var title = $"Selected: {eventModel.Name}";
                var message = $"Starts: {eventModel.Starting:HH:mm}{Environment.NewLine}Details: {eventModel.Description}";
                await App.Current.MainPage.DisplayAlert(title, message, "Ok");
            }
        }

        private async Task ExecuteAddEventAsync(DateTime time)
        {
            Device.BeginInvokeOnMainThread(() => Events?.Clear());

            var pathFile = $"{BaseConstant.PATH_CLASSROOM}{SelectedUser.Id}/{time.Year}.json";

            if (await StorageService.IsFileExistAsync(pathFile))
            {
                var courseMonth = await CourseService.GetItemAsync(time.Month.ToString(), pathFile);
                if (courseMonth == null) return;

                var color = courseMonth.IsPayment ? Payment : UnPayment;
                if (events != null)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        foreach (var e in courseMonth.Time)
                        {
                            Events[e] = new DayEventCollection<CalenderEvent>(color, color) { };
                        }
                    });
                }
            }
        }

        private async Task ExecutePaymentCommandAsync()
        {
            var curPay = new DateTime(MonthYear.Year, MonthYear.Month, 1);
            var pYear = $"{BaseConstant.PATH_CLASSROOM}{SelectedUser.Id}/{curPay.Year}.json";
            var pUser = BaseConstant.PATH_CLASSROOM + SelectedUser.Id + "/user.json";

            if (await StorageService.IsFileExistAsync(pYear))
            {
                var courseMonth = await CourseService.GetItemAsync(curPay.Month.ToString(), pYear);
                if (courseMonth == null) return;

                if (!courseMonth.IsPayment)
                {
                    var total = courseMonth.Time.Count * int.Parse(SelectedUser.Money);

                    SelectedUser.IncomeMoney += total;
                    SelectedUser.DebtMoney -= total;
                    courseMonth.IsPayment = true;

                    await CourseService.UpdateItemAsync(courseMonth, pYear);
                    await UserService.CreateObjectAsync(pUser, SelectedUser);

                    Events?.Clear();
                    foreach (var t in courseMonth.Time)
                    {
                        Events[t] = new DayEventCollection<CalenderEvent>(Payment, Payment) { };
                    }
                }
            }
        }

        private void BackNavigation(User us)
        {
            //if (us != null)
            //{
            //    var item = Users.FirstOrDefault(x => x.Id == us.Id);
            //    if (item != null)
            //    {
            //        item.Money = us.Money;
            //        item.Phone = us.Phone;
            //        item.Description = us.Description;
            //        item.Name = us.Name;
            //        item.Address = us.Address;
            //    }
            //    else Users.Add(us);
            //}
        }

        private async Task LoadItemAsync()
        {
            try
            {
                IsBusy = true;

                Users?.Clear();
                var pClass = BaseConstant.PATH_CLASSROOM;
                var lUser = await StorageService.GetFolders(pClass);
                //await StorageService.DeleteFolder(pClass + lUser[0]);
                foreach (var id in lUser)
                {
                    var p = pClass + id + "/user.json";
                    var u = await UserService.GetObjectAsync(p);
                    u.Background = null;
                    Users.Add(u);
                }
            }
            catch (Exception) { }
            finally
            {
                IsBusy = false;
            }
        }
        #endregion
    }
}
