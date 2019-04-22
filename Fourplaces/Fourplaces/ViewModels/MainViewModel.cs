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

        //private int id=1;

        public PlaceItemSummary PISS
        {

            get
            {
                Console.WriteLine("GETPISS:" + (pisS != null));
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

                Console.WriteLine("GETLPIS:"+(lpis!=null));
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
            Console.WriteLine("MainViewModelConstruct");

            Barrel.ApplicationId = "FOURPLACESID";

            if (Barrel.Current.Exists("LoginResult"))
            {
                Console.WriteLine("Dev_AlreadyConnected");
                SingletonLoginResult.LR = SingletonRestService.RS.CacheData<LoginResult>("LoginResult");



            }




            //Task t= FindData();

            //if (SingletonLoginResult.LR != null)
            //{
            //    Console.WriteLine("Dev_CPAccessToken:" + SingletonLoginResult.LR.AccessToken);
            //}
            //else
            //{
            //    Console.WriteLine("Dev_MVMPasEncoreConnecte");
            //}



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
                Console.WriteLine("Dev_FD");
                //lpis = await rs.RefreshDataAsync();
                listprov = await SingletonRestService.RS.RefreshDataAsync();


            }
            //catch(AuthenticationException ae) //no connected
            catch (NoConnectE e) //no connected
            {

                String url = e.urlSave;
                listprov= SingletonRestService.RS.CacheData<List<PlaceItemSummary>>(url);

                EXCEPTION = e.ExceptionMess;
            }

            try
            {
                posCurr = await GetLocationAsync();

                Console.WriteLine("Dev_SORTGO:" + posCurr.Latitude + "|" + posCurr.Longitude);
                LPIS = sortDistLPIS(listprov);
                //Dictionary<int, double> dico=new Dictionary<int, double>();
                //List<double,PlaceItemSummary> l = new List<double>();
                /*foreach (PlaceItemSummary pis in LPIS)
                {
                    Position posPlace = new Position(pis.Latitude, pis.Longitude);
                    Distance d = DistanceBetweenPoints(posCurr, posPlace);
                    Console.WriteLine("Dev_Dist:" + pis.Title + "|" + d.Kilometers);
                    //l.Add(d.Kilometers);
                    dico.Add(pis.Id, d.Kilometers);


                }

                //var list= dico.Keys.ToList();

                var items=from pair in dico
                          orderby pair.Value ascending
                          select pair;

                foreach (KeyValuePair<int, double> pair in items)
                {
                    Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
                }*/





                /*foreach(double d in l)
                {
                    Console.WriteLine("Dev_DistSort:"+d);
                }*/

                //OnPropertyChanged("LPIS");
            }
            //catch (AuthenticationException ae) //no geo
            catch (Exception e) //no geo
            {

                EXCEPTION = e.Message;
                LPIS = listprov;
            }




        }

        //find a mean to get location before to sort when you have permission asked else bad sort maybe 
        //using TimeSpan.FromSeconds(20000)
        async Task<Position> GetLocationAsync() 
        {
            //textLocation.Text = "Getting Location";
            Console.WriteLine("DevLoc_Getting Location");
            try
            {
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 100;

                var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(5)); //SEE AGAIN
                positionUser = position;
                //positionUser = null;


                //if (positionUser != null)
                //{
                Console.WriteLine(string.Format("DevLoc_Lat: {0}  Long: {1}", positionUser.Latitude, positionUser.Longitude));

                return new Position(positionUser.Latitude, positionUser.Longitude);


                    //Console.WriteLine("Dev_Dist:" + d.Meters);
                //}
                //else
                //{

                //    Console.WriteLine("DevLoc_Lat:null");
                //    return new Position();
                //}
                //textLocation.Text = string.Format("Lat: {0}  Long: {1}", position.Latitude, position.Longitude);


            }
            catch (Exception ex)
            {
                //textLocation.Text = "Unable to get location: " + ex.ToString();
                Console.WriteLine("DevLoc_Unable to get location: " + ex.ToString());

                //throw new AuthenticationException("impossible d'obtenir une localisation gps");
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

            Console.WriteLine("DevLoc_SIZELIST:" + listprov.Count);
            /*foreach (PlaceItemSummary pis in listprov)
            {
                Position posPlace = new Position(pis.Latitude, pis.Longitude);
                Distance d = DistanceBetweenPoints(posCurr, posPlace);
                Console.WriteLine("Dev_DistNoSort:"+pis.Id+"|" + pis.Title + "|" + d.Kilometers);
            }*/

            //LPIS = lpis;

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
                Console.WriteLine("Dev_DistSort:" + pis.Title + "|" + pis.ImageId+ "|" + d.Kilometers);
            }

            return listprov;
        }


        private void AddPlace()
        {
            /*Dictionary<string, object> Test=new Dictionary<string, object>();
            Test.Add("page1", new MainView());

            Initialize(Test);
            NavigationService.PushAsync(new MainView());*/
            NavigationService.PushAsync(new CreatePlace());
            Console.WriteLine("Dev_AddPlace:");




        }

        async private void ItemTapped()
        {
            if (PISS != null)
            {
                Console.WriteLine("Dev_TestItemT");
                //var Selected = e.Item as PlaceItemSummary;

                Console.WriteLine("Dev_MVID:" + PISS.Id);

                Console.WriteLine("Dev_MVLATLONG:" + PISS.Latitude + "|" + PISS.Longitude);

                //await NavigationService.PushAsync(new PlaceItemView(PISS));
                await NavigationService.PushAsync<PlaceItemView>(new Dictionary<string,object>() { { "PIS", PISS } });



                /*Dictionary<String, object> dico = new Dictionary<string, object>(); SEE WITH ANTHONY
                dico.Add("pis",PISS);
                dico.Add("pis",lr);

                await NavigationService.PushAsync<PlaceItemView>(dico);*/
            }

            //Dictionary<String, PlaceItemSummary> dico = new Dictionary<string, PlaceItemSummary>();
            //dico.Add("pis",PISS);
            //



        }

        async private void Connexion()
        {
            await NavigationService.PushAsync(new Connexion());
        }

        async private void Inscription()
        {
            Console.WriteLine("Dev_Inscription");
            await NavigationService.PushAsync(new Inscription());
        }

        async private void Compte()
        {
            Console.WriteLine("Dev_MC");
            //await NavigationService.PushAsync(new MonCompte());
            await NavigationService.PushAsync<MonCompte>();
        }

        public override Task OnResume()
        {
            EXCEPTION = "";
            LPIS = null;
            Console.WriteLine("MainViewModel");

            Task t = FindData();

            

            if (SingletonLoginResult.LR != null)
            {
                IsVisible = true;
                Console.WriteLine("Dev_CPAccessToken:" + SingletonLoginResult.LR.AccessToken);
            }
            else
            {
                IsVisible = false;
                Console.WriteLine("Dev_MVMPasEncoreConnecte");
            }
            return base.OnResume();
        }



    }

}

