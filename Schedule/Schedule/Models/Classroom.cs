using BaseXamarin.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Schedule.Models
{
    public class Course : BaseModel
    {
        public List<DateTime> Time { get; set; }
        public bool IsPayment { get; set; }
    }

    public class Speaker
    {
        public string Name { get; set; }
        public string Time { get; set; }
    }

    public class Agenda
    {
        public string Topic { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public ObservableCollection<Speaker> Speakers { get; set; }
        public string Color { get; set; }
    }
}
