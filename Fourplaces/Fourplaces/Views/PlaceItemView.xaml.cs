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
        //public PlaceItemView()
        //{
        //    InitializeComponent();
        //}

        //public PlaceItemView(PlaceItemSummary pis)
        public PlaceItemView()

        {
            InitializeComponent();



            /*mapT = new Map(
            MapSpan.FromCenterAndRadius(
                    new Position(37, -122), Distance.FromMiles(0.3)))
            {
                IsShowingUser = true,
                HeightRequest = 100,
                WidthRequest = 960,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            var stack = new StackLayout { Spacing = 0 };
            stack.Children.Add(mapT);
            Content = stack;*/

            //PlaceItemViewModel pivm= new PlaceItemViewModel(pis.Id);

            //BindingContext = new PlaceItemViewModel(pis.Id);
            BindingContext = new PlaceItemViewModel();

            //BindingContext = pivm;

            /*mapPlace = new Map(
            MapSpan.FromCenterAndRadius(
                    new Position(pis.Latitude, pis.Longitude), Distance.FromMiles(0.3)))
            {
                IsShowingUser = true,
                HeightRequest = 200,
                WidthRequest = 320,
                //VerticalOptions = LayoutOptions.FillAndExpand
            };*/


            /*mapPlace.MoveToRegion(
            MapSpan.FromCenterAndRadius(new Position(pis.Latitude, pis.Longitude), Distance.FromMiles(0.2f)));
            //MapSpan.FromCenterAndRadius(pivm.POS, Distance.FromMiles(1)));

            Console.WriteLine("Dev_PlaceItemViewModel" + pis.Id);*/


        }

    }
}
