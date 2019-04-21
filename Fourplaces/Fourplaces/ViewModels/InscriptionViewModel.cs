using System;
using Fourplaces.Models;
using Fourplaces.Models.Exceptions;
using Fourplaces.Views;
using Storm.Mvvm;
using TD.Api.Dtos;
using Xamarin.Forms;

namespace Fourplaces.ViewModels
{
    public class InscriptionViewModel : ViewModelBase
    {
        public LoginResult lr;
        private string exception;

        public InscriptionViewModel()
        {

            EMAIL = "test@test.com";
            FNAME = "FTest";
            LNAME = "LTest";
            MDP = "Test";
        }


        public String EMAIL { get; set; }

        public String FNAME { get; set; }

        public String LNAME { get; set; }

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
                return new Command(() => SendRegister());
            }
        }

        public async void SendRegister()
        {
            try
            {
                Console.WriteLine("Dev_Send:" + EMAIL + "|" + FNAME + "|" + LNAME + "|" + MDP);
                lr = await SingletonRestService.RS.RegisterDataAsync(EMAIL, FNAME, LNAME, MDP);
                if (lr != null) //IMPROVE AGAIN
                {
                    SingletonLoginResult.destroyLR();
                    SingletonLoginResult.LR = lr;
                    Console.WriteLine("Dev_RDAccessToken:" + SingletonLoginResult.LR.AccessToken);
                    await NavigationService.PopAsync();
                    //await NavigationService.PushAsync(new MainView());

                }
            }
            catch(AuthenticationException ae)
            {
                EXCEPTION = ae.ExceptionMess;
            }

            



        }

    }
}

