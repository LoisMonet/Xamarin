using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Common.Api.Dtos;
using Fourplaces.Models;
using Fourplaces.Models.Exceptions;
using Fourplaces.Views;
using MonkeyCache.SQLite;
using Newtonsoft.Json;
using Plugin.Geolocator;
using Storm.Mvvm;
using TD.Api.Dtos;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Fourplaces.ViewModels
{
    public class MainViewModel : ViewModelBase
    {

        public List<PlaceItemSummary> lpis;


        public Position posCurr;

        private Plugin.Geolocator.Abstractions.Position positionUser = null;

        private PlaceItemSummary pisS;
        private string exception;
        private bool _isVisible;

        public PlaceItemSummary PISS
        {

            get
            {
                return pisS;
            }

            set
            {

                SetProperty(ref pisS, value);
                ItemTapped();
            }

        }

        public Boolean IsVisible
        {
            get
            {
                return _isVisible;
            }

            set
            {
                SetProperty(ref _isVisible, value);
            }
        }


        public Command ADDPLACE
        {
            get
            {
                return new Command(() => AddPlace());
            }

        }

        public Command CONNEXION
        {
            get
            {
                return new Command(() => Connexion());
            }

        }

        public Command INSCRIPTION 
        {
            get
            {
                return new Command(() => Inscription());
            }
        }

        public Command MONCOMPTE
        {
            get
            {
                return new Command(() => Compte());
            }

        }


        public List<PlaceItemSummary> LPIS
        {

            get
            {

                return lpis;
            }

            set => SetProperty(ref lpis, value);

        }

        public String EXCEPTION
        {
            get
            {
                return exception;
            }

            set
            {
                SetProperty(ref exception, value);
            }
        }



        public MainViewModel()
        {

            Barrel.ApplicationId = "FOURPLACESID";

            if (Barrel.Current.Exists("LoginResult"))
            {
                SingletonLoginResult.LR = SingletonRestService.RS.CacheData<LoginResult>("LoginResult");



            }



        }

        public async Task RefreshTokenCache()
        {
            await SingletonRestService.RS.RefreshAsync();
        }


        public async Task FindData()
        {
            List<PlaceItemSummary> listprov;

            try
            {
      
                listprov = await SingletonRestService.RS.RefreshDataAsync();


            }
            catch (NoConnectE e) //no connected
            {

                String url = e.urlSave;
                listprov= SingletonRestService.RS.CacheData<List<PlaceItemSummary>>(url);

                EXCEPTION = e.ExceptionMess;
            }

            try
            {
                posCurr = await GetLocationAsync();

                LPIS = sortDistLPIS(listprov);

            }
            catch (Exception e) //no geo
            {

                EXCEPTION = e.Message;
                LPIS = listprov;
            }




        }

      
        async Task<Position> GetLocationAsync() 
        {
           
            try
            {
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 100;

                var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(0.5f)); //SEE AGAIN
                positionUser = position;
               
                return new Position(positionUser.Latitude, positionUser.Longitude);

            }
            catch (Exception)
            {
              
                throw new Exception("impossible d'obtenir une localisation gps");
            }
        }


        const int EARTH_RADIUS_KM = 6371;

        Distance DistanceBetweenPoints(Position p1, Position p2)
        {
            double latitude1 = DegreesToRadians(p1.Latitude);
            double latitude2 = DegreesToRadians(p2.Latitude);
            double longitude1 = DegreesToRadians(p1.Longitude);
            double longitude2 = DegreesToRadians(p2.Longitude);

            double distance = Math.Sin((latitude2 - latitude1) / 2.0);
            distance *= distance;

            double intermediate = Math.Sin((longitude2 - longitude1) / 2.0);
            intermediate *= intermediate;

            distance = distance + Math.Cos(latitude1) * Math.Cos(latitude2) * intermediate;
            distance = 2 * EARTH_RADIUS_KM * Math.Atan2(Math.Sqrt(distance), Math.Sqrt(1 - distance));

            return Distance.FromKilometers(distance);
        }

        double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }


        public List<PlaceItemSummary> sortDistLPIS(List<PlaceItemSummary> listprov)
        {

         
            listprov.Sort(delegate (PlaceItemSummary pis1, PlaceItemSummary pis2) {
                Position posPlace1 = new Position(pis1.Latitude, pis1.Longitude);
                Position posPlace2 = new Position(pis2.Latitude, pis2.Longitude);
                Distance d1 = DistanceBetweenPoints(posCurr, posPlace1);
                Distance d2 = DistanceBetweenPoints(posCurr, posPlace2);
                return d1.Kilometers.CompareTo(d2.Kilometers);
            });



            foreach (PlaceItemSummary pis in listprov)
            {
                Position posPlace = new Position(pis.Latitude, pis.Longitude);
                Distance d = DistanceBetweenPoints(posCurr, posPlace);
            }

            return listprov;
        }


        private void AddPlace()
        {

            NavigationService.PushAsync(new CreatePlace());
       
        }

        async private void ItemTapped()
        {
            if (PISS != null)
            {
          
                await NavigationService.PushAsync<PlaceItemView>(new Dictionary<string,object>() { { "PIS", PISS } });

            }

        }

        async private void Connexion()
        {
            await NavigationService.PushAsync(new Connexion());
        }

        async private void Inscription()
        {
            await NavigationService.PushAsync(new Inscription());
        }

        async private void Compte()
        {
            await NavigationService.PushAsync<MonCompte>();
        }

        public override Task OnResume()
        {
            EXCEPTION = "";
            LPIS = null;

            Task t = FindData();

            

            if (SingletonLoginResult.LR != null)
            {
                IsVisible = true;
            }
            else
            {
                IsVisible = false;
            }
            return base.OnResume();
        }



    }

}

