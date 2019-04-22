using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fourplaces.Models;
using Fourplaces.Models.Exceptions;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Storm.Mvvm;
using Storm.Mvvm.Navigation;
using TD.Api.Dtos;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Fourplaces.ViewModels
{
    public class PlaceItemViewModel : ViewModelBase
    {
        private PlaceItem pi;

        private String inputCom= "";

        private PlaceItemSummary pis;

        private ImageSource imageP;

        private CustomMap map;
        private string exception;
        private bool _isVisible;

        [NavigationParameter] //see that later
        public PlaceItemSummary PIS
        {
            get { return pis; }
            //get { return 1; }

            set
            {
                Console.WriteLine("SETIDDD:"+value.Id);
                SetProperty(ref pis, value);
                //Task t = FindPlaceItem(pis.Id); //LATER MAYBE USE IT BECAUSE WITH ONRESUME TO LONG TO SWITCH PAGE


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

        public CustomMap MAP
        {
            get
            {
                return map;
            }

            set
            {

                SetProperty(ref map, value);
            }
        
        }
        public Command SENDCOM
        {
            get
            {
                return new Command(()=>AddComment());
            }
           
        }


        public String INPUTCOM
        {

            get
            {
                
                return inputCom;
            }

            set
            {
                SetProperty(ref inputCom, value);
                //AddComment();

            }

        }

        public PlaceItem PI
        {

            get
            {
                //Console.WriteLine("GETPI:"+pi[0].Id);

                return pi;
            }

            set => SetProperty(ref pi, value);



        }

        public ImageSource IMAGEP
        {
            get
            {
                return imageP;
            }

            set
            {
                SetProperty(ref imageP, value);
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


        public PlaceItemViewModel()
        //public PlaceItemViewModel(int id)
        {
            //Task t = FindPlaceItem(1); //Work on that later
            //Task t = FindPlaceItem(id); //Work on that later


            //Task tLoc = GetLocationAsync();

        }

        private async Task FindPlaceItem(int id)
        {
            Console.WriteLine("Dev_FPI_BIS");

            try
            {
                PI = await SingletonRestService.RS.PlaceItemDataAsync(id);
                CreateMap();
            }
            //catch (AuthenticationException ae) //no connected
            catch (NoConnectE e) //no connected
            {
                EXCEPTION = e.ExceptionMess;
                //Console.WriteLine("DEV_EXCEPTION:" + EXCEPTION);
                String url = e.urlSave;
                PI = SingletonRestService.RS.CacheData<PlaceItem>(url);
                if (PI != null)
                {
                    CreateMap();
                }

                if (SingletonLoginResult.LR != null)
                {
                    //Console.WriteLine("ISVISIBLETR:"+(PI == null));
                    if (PI == null)
                    {
                        //Console.WriteLine("ISVISIBLETRUE1");
                        IsVisible = false;
                    }
                    else
                    {
                        //Console.WriteLine("ISVISIBLETRUE2:");
                        IsVisible = true;
                    }
                }
            }

            try
            {
                await getImage();
            }
            //catch (AuthenticationException ae) //no connected
            catch (NoConnectE e) //no connected
            {
                Console.WriteLine("DEV_EXCEPTIONIMAGE:" + EXCEPTION);
                String url = e.urlSave;
                IMAGEP = SingletonRestService.RS.CacheImage(url);

            }






        Console.WriteLine("Dev_IDResponse:" + pi.Id);


        }

        public void CreateMap()
        {
            CustomMap mapTmp = new CustomMap();
            var pin = new CustomPin
            {
                Type = PinType.Place,
                Position = new Xamarin.Forms.Maps.Position(PI.Latitude, PI.Longitude),
                Label = PI.Title,

            };

            mapTmp.CustomPins = new List<CustomPin> { pin };
            mapTmp.MapType = MapType.Hybrid;
            mapTmp.WidthRequest = 320;
            mapTmp.HeightRequest = 200;
            mapTmp.Pins.Add(pin);
            mapTmp.MoveToRegion(MapSpan.FromCenterAndRadius(
              new Xamarin.Forms.Maps.Position(PI.Latitude, PI.Longitude), Distance.FromMiles(1.0)));

            MAP = mapTmp;
        }

        public async void AddComment()
        {
            try
            {
                //Initialize(new Dictionary<string, object> { "test":"testE"})
                Console.WriteLine("Dev_Comm:" + PI.Id + "|" + INPUTCOM);
                await SingletonRestService.RS.SendCommentDataAsync(PI.Id, INPUTCOM);
                //Console.WriteLine("Dev_OnResumeBef:" + PIS.Id);
                await OnResume();
            }
            //catch (AuthenticationException ae)
            catch (NoConnectE e) //no connected
            {

                EXCEPTION=e.ExceptionMess;
            }
            catch (Exception e)
            {

                EXCEPTION = e.Message;
            }



        }

        public async Task getImage()
        {
            Console.WriteLine("Dev_getImage");
            IMAGEP = await SingletonRestService.RS.GetRequestImage(PI.ImageId);



        }

        public override Task OnResume()
        {
            if (SingletonLoginResult.LR != null)
            {
                IsVisible = true;

            }
            else
            {
                //Console.WriteLine("ISVISIBLETRUE3");
                IsVisible = false;
            }

            //Console.WriteLine("Dev_OnResume:" +PIS.Id);
            INPUTCOM = "";
            Task t = FindPlaceItem(PIS.Id); //Work on that later





            return base.OnResume();
        }




    }
}

