using System;
using System.IO;
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

        private ImageSource _image;
        private byte[] imageB;
        //private Command _sendWPicture;
        //private Command _sendWGallery;
        private Command _send;
        private Command _picture;

        private bool typeP=false;


        public CreatePlaceViewModel()
        {
          

            NOM = "Test";
            DESCRIPTION = "t";
            LATITUDE = "2";
            LONGITUDE = "2";
            IMAGE = "profilDef.png";
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

        public String LATITUDE { get; set; }

        public String LONGITUDE { get; set; }


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

            Console.WriteLine("Dev_Send:" + NOM + "|" + DESCRIPTION + "|" + IMAGE + "|" + LATITUDE + "|" + LONGITUDE);

            if (SingletonLoginResult.LR != null)
            {
                Console.WriteLine("Dev_CPAccessToken:" + SingletonLoginResult.LR.AccessToken);
                bool send = await SingletonRestService.RS.SendPlaceDataAsync(NOM, DESCRIPTION, LATITUDE, LONGITUDE,imageB, SingletonLoginResult.LR);
                if (send)
                {
                    await NavigationService.PopAsync();
                    //await NavigationService.PushAsync(new MainView());
                }

            }
            else
            {
                Console.WriteLine("Dev_CPPasEncoreConnecte:");
            }

        }

        public async void ChoosePicture()
        {

            imageB =await SingletonRestService.RS.SendPicture(TYPEP); 
            IMAGE = ImageSource.FromStream(() => new MemoryStream(imageB));

            Console.WriteLine("Dev_ChoosePicture:"+ TYPEP);
        }

        //async void OnActionSheetSimpleClicked(object sender, EventArgs e) SEET ALERT LATER
        //{
        //    string action = await DisplayActionSheet("ActionSheet: Send to?", "Cancel", null, "Email", "Twitter", "Facebook");
        //    Debug.WriteLine("Action: " + action);
        //}

    }
}

