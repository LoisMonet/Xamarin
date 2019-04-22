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


            //Email = "test@test.com";

            //MDP = "Test";
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

            try
            {
                lr = await SingletonRestService.RS.ConnexionDataAsync(Email, MDP);

                SingletonLoginResult.destroyLR();
                SingletonLoginResult.LR = lr;
                await NavigationService.PopAsync();



            }
            catch (NoConnectE e)
            {
                EXCEPTION = e.ExceptionMess;
            }
            catch (ConnexionE e)
            {
                EXCEPTION = e.ExceptionMess;
            }









        }


    }
}

