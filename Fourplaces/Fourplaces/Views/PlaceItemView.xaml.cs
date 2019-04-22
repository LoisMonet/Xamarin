using System;
using System.Collections.Generic;
using Fourplaces.Models;
using Fourplaces.ViewModels;
using Storm.Mvvm.Forms;
using TD.Api.Dtos;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Fourplaces.Views
{
    public partial class PlaceItemView : BaseContentPage
    {

        public PlaceItemView()

        {
            InitializeComponent();

            BindingContext = new PlaceItemViewModel();

        }

    }
}
