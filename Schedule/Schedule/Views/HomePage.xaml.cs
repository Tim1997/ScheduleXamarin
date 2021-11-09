using Schedule.Models;
using Schedule.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Schedule.Views
{
    public partial class HomePage : ContentPage
    {
        private int _lastSelectedIndex;
        public HomePage()
        {
            InitializeComponent();
        }

        private void CoverFlowView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("SelectedIndex"))
            {
                var flowView = (PanCardView.CoverFlowView)sender;
                var currentIndex = flowView.SelectedIndex;

                if (currentIndex != _lastSelectedIndex)
                {
                    _lastSelectedIndex = currentIndex;
                }
            }
        }
    }
}