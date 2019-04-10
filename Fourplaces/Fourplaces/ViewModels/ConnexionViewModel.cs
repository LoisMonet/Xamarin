using System;
using System.Threading.Tasks;
using Fourplaces.Models;
using Fourplaces.Views;
using Storm.Mvvm;
using TD.Api.Dtos;
using Xamarin.Forms;

namespace Fourplaces.ViewModels
{
    public class ConnexionViewModel : ViewModelBase
    {
        public LoginResult lr;

        public ConnexionViewModel()
        {


            LOGIN = "test@test.com";

            MDP = "Test";
        }

      

        public String LOGIN { get; set; }

        public String MDP { get; set; }


        public Command SEND
        {
            get
            {
                return new Command(() =>SendConnexion());
            }
        }

        public async void SendConnexion()
        {

            Console.WriteLine("Dev_Send:" + LOGIN + "|" + MDP);
            lr= await SingletonRestService.RS.ConnexionDataAsync(LOGIN, MDP);
            if (lr != null)
            {
                Console.WriteLine("Dev_CDAccessToken:" + lr.AccessToken);
                SingletonLoginResult.LR = lr;
                await NavigationService.PushAsync(new MainView());
            }


        }


    }
}

