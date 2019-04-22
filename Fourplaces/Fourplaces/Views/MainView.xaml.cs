using System;
using System.Collections.Generic;
using Fourplaces.Models;
using Fourplaces.ViewModels;
using Storm.Mvvm.Forms;
using TD.Api.Dtos;
using Xamarin.Forms;

namespace Fourplaces.Views
{
    public partial class MainView : BaseContentPage
    {
        public MainView()
        {
            InitializeComponent();
            BindingContext = new MainViewModel();




        }

    }

}
