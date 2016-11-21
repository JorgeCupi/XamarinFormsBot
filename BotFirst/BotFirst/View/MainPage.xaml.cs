using BotFirst.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace BotFirst.View
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            MainPageViewModel vm = new MainPageViewModel();
            this.BindingContext = vm;
        }
    }
}
