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
        /*private String nom;
        private String description;
        private String image;
        private String latitude;
        private String longitude;*/

        private ImageSource _image;
        private byte[] imageB;
        //private Command _sendWPicture;
        //private Command _sendWGallery;
        private Command _send;
        private Command _picture;

        private bool typeP=false;
        private string exception;
        private Position positionUser;
        private string _latitude;
        private string _longitude;

        public CreatePlaceViewModel()
        {
          

            NOM = "Test";
            DESCRIPTION = "t";
            Task t =  GetLocationAsync();



            IMAGE = "placeDef.png";
            //_sendWGallery = new Command(() => SendPlaceWGallery());
            //_sendWPicture = new Command(() => SendPlaceWCamera());
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

        public bool TYPEP //SEE AGAIN SWITCH GALLERY Camera
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


        //public Command SENDWPICTURE
        //{
        //    get
        //    {
        //        return _sendWPicture;
        //    }
        //}

        //public Command SENDWGALLERY
        //{
        //    get
        //    {
        //        return _sendWGallery;
        //    }
        //}

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

        //public async void SendPlaceWCamera()
        //{

        //    Console.WriteLine("Dev_Send:" + NOM + "|" + DESCRIPTION + "|" + IMAGE + "|" + LATITUDE + "|" + LONGITUDE);

        //    if (SingletonLoginResult.LR != null)
        //    {
        //        Console.WriteLine("Dev_CPAccessToken:" + SingletonLoginResult.LR.AccessToken);
        //        bool send=await SingletonRestService.RS.SendPlaceDataAsync(NOM, DESCRIPTION, LATITUDE, LONGITUDE, SingletonLoginResult.LR, true);
        //        if (send)
        //        {
        //            await NavigationService.PopAsync();
        //            //await NavigationService.PushAsync(new MainView());
        //        }

        //    }
        //    else
        //    {
        //        Console.WriteLine("Dev_CPPasEncoreConnecte:");
        //    }

        //}

        //public async void SendPlaceWGallery()
        //{

        //    Console.WriteLine("Dev_Send:" + NOM + "|" + DESCRIPTION + "|" + IMAGE + "|" + LATITUDE + "|" + LONGITUDE);

        //    if (SingletonLoginResult.LR != null)
        //    {
        //        Console.WriteLine("Dev_CPAccessToken:" + SingletonLoginResult.LR.AccessToken);
        //        bool send = await SingletonRestService.RS.SendPlaceDataAsync(NOM, DESCRIPTION, LATITUDE, LONGITUDE, SingletonLoginResult.LR, false);
        //        if (send)
        //        {
        //            await NavigationService.PopAsync();
        //            //await NavigationService.PushAsync(new MainView());
        //        }

        //    }
        //    else
        //    {
        //        Console.WriteLine("Dev_CPPasEncoreConnecte:");
        //    }

        //}

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
                    //await NavigationService.PushAsync(new MainView());
                }

               

            }
            //catch(AuthenticationException ae)
            catch (NoConnectE e)
            {
                EXCEPTION = e.ExceptionMess;
               // Console.WriteLine("Dev_Exception:" + EXCEPTION);
            }
            catch (AddPlaceE e)
            {
                EXCEPTION = e.ExceptionMess;
                //Console.WriteLine("Dev_Exception:" + EXCEPTION);
            }
            catch (Exception e)
            {
                EXCEPTION = e.Message;
                //Console.WriteLine("Dev_Exception:" + EXCEPTION);
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


        //find a mean to get location before to sort when you have permission asked else bad sort maybe 
        //using TimeSpan.FromSeconds(20000)
        async Task GetLocationAsync()
        {
            //textLocation.Text = "Getting Location";
            Console.WriteLine("DevLoc_Getting Location");
            try
            {
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 100;

                var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(5));
                positionUser = position;
                //positionUser = null;


                //if (positionUser != null)
                //{
                Console.WriteLine(string.Format("DevLoc_Lat: {0}  Long: {1}", positionUser.Latitude, positionUser.Longitude));
                LATITUDE = positionUser.Latitude.ToString().Replace(",","."); //SEE AGAIN MAYBE
                LONGITUDE = positionUser.Longitude.ToString().Replace(",", ".");


            }
            catch (Exception ex)
            {
                //textLocation.Text = "Unable to get location: " + ex.ToString();
                Console.WriteLine("DevLoc_Unable to get location: " + ex.ToString());
                throw new AuthenticationException("impossible d'obtenir une localisation gps");

            }
        }

        //async void OnActionSheetSimpleClicked(object sender, EventArgs e) SEET ALERT LATER
        //{
        //    string action = await DisplayActionSheet("ActionSheet: Send to?", "Cancel", null, "Email", "Twitter", "Facebook");
        //    Debug.WriteLine("Action: " + action);
        //}


    }
}

