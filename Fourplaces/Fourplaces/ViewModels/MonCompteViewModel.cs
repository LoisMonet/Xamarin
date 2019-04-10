using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fourplaces.Models;
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



        public MonCompteViewModel()
        {
            if (SingletonLoginResult.LR != null)
            {
                Console.WriteLine("Dev_MCVM" + SingletonLoginResult.LR.AccessToken);
            }


            _editC = new Command(() => EditCompte());
            _editMDP = new Command(() => EditMDP());

            //rs = new RestService();
            Task t = DataUser();

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

        async private void EditCompte()
        {
            Console.WriteLine("EditCompte");
            await NavigationService.PushAsync<MonCompteEdit>(new Dictionary<string, object>() { { "USER", USER } });
        }

        async private void EditMDP()
        {
            Console.WriteLine("EditMdp");
            await NavigationService.PushAsync<MonMDPEdit>();
        }

        public async Task DataUser()
        {
           USER=await SingletonRestService.RS.UserDataAsync();
           /*Task t=getImage();*/
           


        }

        /*public async Task getImage()
        {
            Console.WriteLine("Dev_getImage");
            IMAGEP = await SingletonRestService.RS.GetRequestImage(USER.ImageId);
            //IMAGEP = "profilDef.png";




        }*/

    }
}

