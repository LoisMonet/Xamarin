using System;
using System.Collections.Generic;
using Fourplaces.ViewModels;
using Storm.Mvvm.Forms;
using TD.Api.Dtos;
using Xamarin.Forms;

namespace Fourplaces.Views
{
    public partial class CreatePlace : BaseContentPage
    {
        public CreatePlace()
        {
            InitializeComponent();
            BindingContext = new CreatePlaceViewModel();

        }
    }
}
