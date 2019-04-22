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

        [NavigationParameter] 
        public PlaceItemSummary PIS
        {
            get { return pis; }
           

            set
            {
                SetProperty(ref pis, value);

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

            }

        }

        public PlaceItem PI
        {

            get
            {

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
        {

        }

        private async Task FindPlaceItem(int id)
        {

            try
            {
                PI = await SingletonRestService.RS.PlaceItemDataAsync(id);
                CreateMap();
            }
            catch (NoConnectE e) //no connected
            {
                EXCEPTION = e.ExceptionMess;
                String url = e.urlSave;
                PI = SingletonRestService.RS.CacheData<PlaceItem>(url);
                if (PI != null)
                {
                    CreateMap();
                }

                if (SingletonLoginResult.LR != null)
                {
                    if (PI == null)
                    {
                        IsVisible = false;
                    }
                    else
                    {
                        IsVisible = true;
                    }
                }
            }

            try
            {
                await getImage();
            }
            catch (NoConnectE e)
            {
                String url = e.urlSave;
                IMAGEP = SingletonRestService.RS.CacheImage(url);

            }


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

                await SingletonRestService.RS.SendCommentDataAsync(PI.Id, INPUTCOM);
                await OnResume();
            }
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
                IsVisible = false;
            }

            INPUTCOM = "";
            Task t = FindPlaceItem(PIS.Id); 





            return base.OnResume();
        }




    }
}

