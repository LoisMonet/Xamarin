using System;
using Fourplaces.Models;
using Fourplaces.Views;
using Storm.Mvvm;
using TD.Api.Dtos;
using Xamarin.Forms;

namespace Fourplaces.ViewModels
{
    public class CreatePlaceViewModel : ViewModelBase
    {
        /*private String nom;
        private String description;
        private String image;
        private String latitude;
        private String longitude;*/



        public CreatePlaceViewModel()
        {
          

            NOM = "PAS MOI";
            DESCRIPTION = "vous etes ou";
            IMAGE = "Image";
            LATITUDE = "0.7";
            LONGITUDE = "0.7";

        }
        public String NOM { get; set; }

        public String DESCRIPTION { get; set; }

        public String IMAGE { get; set; }

        public String LATITUDE { get; set; }

        public String LONGITUDE { get; set; }


        public Command SENDWPICTURE
        {
            get
            {
                return new Command(() => SendPlaceWCamera());
            }
        }

        public Command SENDWGALLERY
        {
            get
            {
                return new Command(() => SendPlaceWGallery());
            }
        }

        public async void SendPlaceWCamera()
        {

            Console.WriteLine("Dev_Send:" + NOM + "|" + DESCRIPTION + "|" + IMAGE + "|" + LATITUDE + "|" + LONGITUDE);

            if (SingletonLoginResult.LR != null)
            {
                Console.WriteLine("Dev_CPAccessToken:" + SingletonLoginResult.LR.AccessToken);
                bool send=await SingletonRestService.RS.SendPlaceDataAsync(NOM, DESCRIPTION, LATITUDE, LONGITUDE, SingletonLoginResult.LR, true);
                if (send)
                {
                    await NavigationService.PushAsync(new MainView());
                }

            }
            else
            {
                Console.WriteLine("Dev_CPPasEncoreConnecte:");
            }

        }

        public async void SendPlaceWGallery()
        {

            Console.WriteLine("Dev_Send:" + NOM + "|" + DESCRIPTION + "|" + IMAGE + "|" + LATITUDE + "|" + LONGITUDE);

            if (SingletonLoginResult.LR != null)
            {
                Console.WriteLine("Dev_CPAccessToken:" + SingletonLoginResult.LR.AccessToken);
                bool send = await SingletonRestService.RS.SendPlaceDataAsync(NOM, DESCRIPTION, LATITUDE, LONGITUDE, SingletonLoginResult.LR, false);
                if (send)
                {
                    await NavigationService.PushAsync(new MainView());
                }

            }
            else
            {
                Console.WriteLine("Dev_CPPasEncoreConnecte:");
            }

        }
    }
}

