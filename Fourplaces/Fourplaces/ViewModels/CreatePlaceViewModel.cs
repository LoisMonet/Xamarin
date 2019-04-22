using System;
using System.IO;
using System.Threading.Tasks;
using Fourplaces.Models;
using Fourplaces.Models.Exceptions;
using Fourplaces.Views;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Storm.Mvvm;
using TD.Api.Dtos;
using Xamarin.Forms;

namespace Fourplaces.ViewModels
{
    public class CreatePlaceViewModel : ViewModelBase
    {

        private ImageSource _image;
        private byte[] imageB;

        private Command _send;
        private Command _picture;

        private bool typeP=false;
        private string exception;
        private Position positionUser;
        private string _latitude;
        private string _longitude;

        public CreatePlaceViewModel()
        {
          

            Task t =  GetLocationAsync();

            IMAGE = "placeDef.png";

            _send = new Command(() => Send());
            _picture = new Command(() => ChoosePicture());


        }

        public String NOM { get; set; }

        public String DESCRIPTION { get; set; }

        public ImageSource IMAGE {

            get
            {   

                return _image;
            }
            set
            {
                SetProperty(ref _image, value);
            }


        }

        public bool TYPEP 
        {
            get
            {
            

                return (typeP);
            }

            set
            {
                SetProperty(ref typeP, value);
            }
        }

        public String LATITUDE
        {

            get
            {
                return _latitude;
            }

            set
            {
                SetProperty(ref _latitude, value);
            }
        }

        public String LONGITUDE
        {

            get
            {
                return _longitude;
            }

            set
            {
                SetProperty(ref _longitude, value);
            }
        }


        public Command SEND
        {
            get
            {
                return _send;
            }
        }

        public Command PICTURE
        {
            get
            {
                return _picture;
            }
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


        public async void Send()
        {

            try
            {
                String latMod = LATITUDE;
                String longMod = LONGITUDE;
                if (!(string.IsNullOrEmpty(latMod)) && !(string.IsNullOrEmpty(longMod)))
                { 
                
                    latMod = latMod.Replace(" ", "");
                    latMod = latMod.Replace(",", ".");
                    longMod = longMod.Replace(" ", "");
                    longMod = longMod.Replace(",", ".");

                    Console.WriteLine("Dev_Send:" + NOM + "|" + DESCRIPTION + "|" + IMAGE + "|" + latMod + "|" + longMod);

                }


                bool send = await SingletonRestService.RS.SendPlaceDataAsync(NOM, DESCRIPTION, latMod, longMod, imageB, SingletonLoginResult.LR);
                if (send)
                {
                    await NavigationService.PopAsync();
                }

               

            }
            catch (NoConnectE e)
            {
                EXCEPTION = e.ExceptionMess;
            }
            catch (AddPlaceE e)
            {
                EXCEPTION = e.ExceptionMess;
            }
            catch (Exception e)
            {
                EXCEPTION = e.Message;
            }


        }

        public async void ChoosePicture()
        {

            imageB =await SingletonRestService.RS.SendPicture(TYPEP);
            if (imageB != null)
            {
                IMAGE = ImageSource.FromStream(() => new MemoryStream(imageB));
            }

            Console.WriteLine("Dev_ChoosePicture:"+ TYPEP);
        }


        async Task GetLocationAsync()
        {
            Console.WriteLine("DevLoc_Getting Location");
            try
            {
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 100;

                var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(0.5f));
                positionUser = position;
              
                Console.WriteLine(string.Format("DevLoc_Lat: {0}  Long: {1}", positionUser.Latitude, positionUser.Longitude));
                LATITUDE = positionUser.Latitude.ToString().Replace(",","."); 
                LONGITUDE = positionUser.Longitude.ToString().Replace(",", ".");


            }
            catch (Exception)
            {
                throw new Exception("impossible d'obtenir une localisation gps");

            }
        }


    }
}

