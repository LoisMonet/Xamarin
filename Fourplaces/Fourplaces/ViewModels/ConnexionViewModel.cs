using System;
using System.Threading.Tasks;
using Fourplaces.Models;
using Fourplaces.Models.Exceptions;
using Fourplaces.Views;
using Storm.Mvvm;
using TD.Api.Dtos;
using Xamarin.Forms;

namespace Fourplaces.ViewModels
{
    public class ConnexionViewModel : ViewModelBase
    {
        public LoginResult lr;
        private string exception;

        public ConnexionViewModel()
        {


            Email = "test@test.com";

            MDP = "Test";
        }

      

        public String Email { get; set; }

        public String MDP { get; set; }

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

        public Command SEND
        {
            get
            {
                return new Command(() =>SendConnexion());
            }
        }

        public async void SendConnexion()
        {

            Console.WriteLine("Dev_Send:" + Email + "|" + MDP);


            try
            {
                lr = await SingletonRestService.RS.ConnexionDataAsync(Email, MDP);

                Console.WriteLine("Dev_CDAccessToken:" + lr.AccessToken);
                SingletonLoginResult.LR = lr;
                await NavigationService.PopAsync();
                //await NavigationService.PushAsync(new MainView());
                


            }
            catch(AuthenticationException ae)
            {
                EXCEPTION = ae.ExceptionMess;
            }









        }


    }
}

