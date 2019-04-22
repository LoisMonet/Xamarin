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
        private Command _editC;
        private Command _editMDP;
        private UserItem _user;
        private string exception;
        private bool _isVisible;

        public MonCompteViewModel()
        {


            _editC = new Command(() => EditCompte());
            _editMDP = new Command(() => EditMDP());

          

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
            await NavigationService.PushAsync<MonCompteEdit>(new Dictionary<string, object>() { { "USER", USER } });
        }

        async private void EditMDP()
        {
            await NavigationService.PushAsync<MonMDPEdit>();
        }

        public async Task DataUser()
        {
            try
            {
                USER = await SingletonRestService.RS.UserDataAsync();
            }
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



        }


        public override Task OnResume()
        {
            Task t = DataUser();
            return base.OnResume();
        }

    }
}

