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

            Console.WriteLine("MainView");



        }
        /*async private void MainListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var Selected = e.Item as PlaceItemSummary;

            Console.WriteLine("Dev_MVID:" + Selected.Id);

            Console.WriteLine("Dev_MVLATLONG:" + Selected.Latitude+"|"+Selected.Longitude);

            await Navigation.PushAsync(new PlaceItemView(Selected));



        }*/




    
    }

}
