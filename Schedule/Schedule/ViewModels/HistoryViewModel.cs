using BaseXamarin.Extensions;
using BaseXamarin.Services.Json;
using BaseXamarin.Services.Navigation;
using Schedule.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Schedule.ViewModels
{
    public class HistoryViewModel : BaseXamarin.ViewModels.BaseViewModel
    {
        #region Properties
        private string[] Colors = { "#B96CBD", "#49A24D", "#FDA838", "#F75355", "#00C6AE", "#455399", "#2c35d4" };
        //private ObservableCollection<Agenda> GetAgenda()
        //{
        //    return new ObservableCollection<Agenda>
        //    {
        //        new Agenda { Topic = "All Things Xamarin", Duration = "07:30 UTC - 11:30 UTC", Color = "#B96CBD", Date = new DateTime(2020, 3, 23),
        //            Speakers = new ObservableCollection<Speaker>{ new Speaker { Name = "Maddy Leger", Time = "07:30" }, new Speaker { Name = "David Ortinau", Time = "08:30" }, new Speaker { Name = "Gerald Versluis", Time = "10:30" } } },

        //        new Agenda { Topic = "Visualize Your Data", Duration = "07:30 UTC - 11:30 UTC", Color = "#49A24D", Date = new DateTime(2020, 3, 24),
        //            Speakers = new ObservableCollection<Speaker>{ new Speaker { Name = "Maddy Leger", Time = "07:30" }, new Speaker { Name = "David Ortinau", Time = "08:30" }, new Speaker { Name = "Gerald Versluis", Time = "10:30" } } },

        //        new Agenda { Topic = "Testing Your Xamarin Apps", Duration = "07:30 UTC - 11:30 UTC", Color = "#FDA838", Date = new DateTime(2020, 3, 25),
        //            Speakers = new ObservableCollection<Speaker>{ new Speaker { Name = "Maddy Leger", Time = "07:30" }, new Speaker { Name = "David Ortinau", Time = "08:30" }, new Speaker { Name = "Gerald Versluis", Time = "10:30" } } },

        //        new Agenda { Topic = "Xamarin Productivity to the Max", Duration = "07:30 UTC - 11:30 UTC", Color = "#F75355", Date = new DateTime(2020, 3, 26),
        //            Speakers = new ObservableCollection<Speaker>{ new Speaker { Name = "Maddy Leger", Time = "07:30" }, new Speaker { Name = "David Ortinau", Time = "08:30" }, new Speaker { Name = "Gerald Versluis", Time = "10:30" } } },

        //        new Agenda { Topic = "All Things Xamarin.Forms Shell", Duration = "07:30 UTC - 11:30 UTC", Color = "#00C6AE", Date = new DateTime(2020, 3, 27),
        //            Speakers = new ObservableCollection<Speaker>{ new Speaker { Name = "Maddy Leger", Time = "07:30" }, new Speaker { Name = "David Ortinau", Time = "08:30" }, new Speaker { Name = "Gerald Versluis", Time = "10:30" } } },

        //        new Agenda { Topic = "Building Beautiful Apps", Duration = "07:30 UTC - 11:30 UTC", Color = "#455399", Date = new DateTime(2020, 3, 28),
        //            Speakers = new ObservableCollection<Speaker>{ new Speaker { Name = "Maddy Leger", Time = "07:30" }, new Speaker { Name = "David Ortinau", Time = "08:30" }, new Speaker { Name = "Gerald Versluis", Time = "10:30" } } }
        //    };
        //}
        private ObservableCollection<Agenda> agendas;
        private DateTime dateStart, dateEnd, dateMidle;
        private int _duration = 3;
        private Random rnd = new Random();

        public ObservableCollection<Agenda> Agendas { get => agendas; set => SetProperty(ref agendas, value); }
        public DateTime DateStart { get => dateStart; set => SetProperty(ref dateStart, value); }
        public DateTime DateEnd { get => dateEnd; set => SetProperty(ref dateEnd, value); }
        public DateTime DateMidle { get => dateMidle; set => SetProperty(ref dateMidle, value); }
        public Dictionary<User, List<DateTime>> CourseInWeek { get; set; }
        #endregion

        #region Command
        public ICommand PageAppearingCommand => new Command(async () =>
        {
            await LoadItemAsync();
        });
        #endregion

        #region Service
        private IJsonService<Course> CourseService => BaseXamarin.IoC.Container.Current.Resolve<IJsonService<Course>>();
        private IJsonService<User> UserService => BaseXamarin.IoC.Container.Current.Resolve<IJsonService<User>>();

        #endregion

        public HistoryViewModel() : base("History")
        {
            //////////////////////////
            Init();
            //////////////////////////
        }

        #region Init
        private void Init()
        {
            DateMidle = DateTime.Now;
            DateStart = DateMidle.AddDays(-_duration);
            DateEnd = DateMidle.AddDays(_duration);
            Agendas = new ObservableCollection<Agenda>();
            CourseInWeek = new Dictionary<User, List<DateTime>>();
        }
        #endregion

        #region Method
        private void AddAgenda()
        {
            Agendas?.Clear();
            if (CourseInWeek?.Count == 0) return;

            var dates = new List<DateTime>();
            for (var dt = DateStart; dt <= DateEnd; dt = dt.AddDays(1))
            {
                dates.Add(dt);
            }

            foreach (var day in dates)
            {
                var agenda = new Agenda();
                agenda.Speakers = new ObservableCollection<Speaker>();

                foreach (var course in CourseInWeek)
                {
                    var isExist = course.Value.Any(x => x.Day == day.Day && x.Month == day.Month);
                    if (isExist)
                    {
                        agenda.Speakers.Add(new Speaker
                        {
                            Name = course.Key.Name,
                            Time = "19:30",
                        });
                    }
                }

                agenda.Color = Color.FromRgb(rnd.Next(256), rnd.Next(256), rnd.Next(256)).ToHex();
                agenda.Date = day;
                agenda.Topic = "Teaching tutor";
                agenda.Description = "Online math";

                if (agenda.Speakers.Count != 0)
                    Agendas.Add(agenda);
            }
        }

        private async Task LoadItemAsync()
        {
            try
            {
                IsBusy = true;
                CourseInWeek?.Clear();

                var pClass = BaseConstant.PATH_CLASSROOM;
                var lUser = await StorageService.GetFolders(pClass);
                foreach (var id in lUser)
                {
                    var pY = pClass + id + $"/{DateMidle.Year}.json";
                    var pU = pClass + id + $"/user.json";

                    if (DateStart.Month == DateEnd.Month)
                    {
                        try
                        {
                            var courses = await CourseService.GetItemAsync(DateStart.Month.ToString(), pY);
                            var matchCourse = courses?.Time.FindAll(x => x.Day >= DateStart.Day && x.Day <= DateEnd.Day);

                            var u = await UserService.GetObjectAsync(pU);
                            CourseInWeek.Add(u, matchCourse);
                        }
                        catch (Exception) { }
                    }
                    else
                    {
                        try
                        {
                            var coursesPev = await CourseService.GetItemAsync(DateStart.Month.ToString(), pY);
                            var coursesNext = await CourseService.GetItemAsync(DateEnd.Month.ToString(), pY);
                            var matchCoursePev = coursesPev?.Time.FindAll(x => x.Day >= DateStart.Day);
                            var matchCourseNext = coursesNext?.Time.FindAll(x => x.Day <= DateEnd.Day);

                            var u = await UserService.GetObjectAsync(pU);

                            if (matchCoursePev == null) matchCoursePev = new List<DateTime>();
                            if (matchCourseNext == null) matchCourseNext = new List<DateTime>();

                            matchCoursePev.AddRange(matchCourseNext);
                            CourseInWeek.Add(u, matchCoursePev);
                        }
                        catch (Exception) { }
                    }
                }
            }
            catch (Exception) { }
            finally
            {
                AddAgenda();
                IsBusy = false;
            }
        }
        #endregion
    }
}
