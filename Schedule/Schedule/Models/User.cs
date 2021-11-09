using BaseXamarin.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Schedule.Models
{
    public class User : BaseModel
    {
        private string name;
        private string background;
        private string address;
        private string money;
        private string description;
        private string phone;
        private int incomeMoney;
        private int debtMoney;


        public string Background { get => background; set => SetProperty(ref background, value); }
        public string Name { get => name; set => SetProperty(ref name, value); }
        public string Address { get => address; set => SetProperty(ref address, value); }
        public string Description { get => description; set => SetProperty(ref description, value); }
        public string Phone { get => phone; set => SetProperty(ref phone, value); }
        public string Money { get => money; set => SetProperty(ref money, value); }
        public int IncomeMoney { get => incomeMoney; set => SetProperty(ref incomeMoney, value); }
        public int DebtMoney { get => debtMoney; set => SetProperty(ref debtMoney, value); }
    }
}
