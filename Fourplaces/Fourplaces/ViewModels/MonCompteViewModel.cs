using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fourplaces.Models;
using Fourplaces.Models.Exceptions;
using Fourplaces.Views;
using Storm.Mvvm;
using TD.Api.Dtos;
using Xamarin.Forms;

namespace Fourplaces.ViewModels
{
    public class MonCompteViewModel : ViewModelBase
    {
        //public RestService rs;
        private Command _editC;
        private Command _editMDP;
        //private ImageSource imageP;
        private UserItem _user;
        private string exception;
        private bool _isVisible;

        public MonCompteViewModel()
        {
            if (SingletonLoginResult.LR != null)
            {
               // Console.WriteLine("Dev_MCVM" + SingletonLoginResult.LR.AccessToken);
            }


            _editC = new Command(() => EditCompte());
            _editMDP = new Command(() => EditMDP());

            //rs = new RestService();

            //Task t = DataUser();

        }


        /*public ImageSource IMAGEP
        {
            get
            {
                return imageP;
            }

            set
            {
                SetProperty(ref imageP, value);
            }
        }*/
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

        public UserItem USER
        {

            get
            {
                return _user;
            }
            set
            {
                SetProperty(ref _user, value);

            }
        }

        public Command EDITCOMPTE
        {
            get
            {
                return _editC;
            }
        }

        public Command EDITMDP
        {
            get
            {
                return _editMDP;
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


        async private void EditCompte()
        {
           // Console.WriteLine("EditCompte");
            await NavigationService.PushAsync<MonCompteEdit>(new Dictionary<string, object>() { { "USER", USER } });
        }

        async private void EditMDP()
        {
            //Console.WriteLine("EditMdp");
            await NavigationService.PushAsync<MonMDPEdit>();
        }

        public async Task DataUser()
        {
            try
            {
                USER = await SingletonRestService.RS.UserDataAsync();
            }
            //catch(AuthenticationException ae) //no connected
            catch (NoConnectE e) //no connected
            {
                EXCEPTION = e.ExceptionMess;
                USER= SingletonRestService.RS.CacheData<UserItem>("Account");
            }

            if (USER == null)
            {
                IsVisible = false;
            }
            else
            {
                IsVisible = true;
            }

            /*Task t=getImage();*/



        }

        /*public async Task getImage()
        {
            Console.WriteLine("Dev_getImage");
            IMAGEP = await SingletonRestService.RS.GetRequestImage(USER.ImageId);
            //IMAGEP = "profilDef.png";

        }*/

        public override Task OnResume()
        {
            Task t = DataUser();
            return base.OnResume();
        }

    }
}

