using System;
using System.Threading.Tasks;
using Fourplaces.Models;
using Storm.Mvvm;
using Storm.Mvvm.Navigation;
using TD.Api.Dtos;
using Xamarin.Forms;

namespace Fourplaces.ViewModels
{
    public class MonCompteEditViewModel : ViewModelBase
    {
        private UserItem _user;
        private Command _editer;
        private String imageId;

        public MonCompteEditViewModel()
        {

            //Task t = UserData();
            //Console.WriteLine("Dev_MCEVM:" + USER.Email);
            _editer = new Command(() => Editer());

        }



        [NavigationParameter]
        public UserItem USER
        {

            get
            {
                return _user;
            }
            set
            {
                Console.WriteLine("mcevm:" + value.FirstName);
                SetProperty(ref _user, value);

                IMAGEID=USER.ImageId.ToString();



            }
        }

        public String IMAGEID
        {

            get
            {
                Console.WriteLine("IMAGEIDC:" + imageId);
                return imageId;

            }
            set
            {
                SetProperty(ref imageId, value);


            }
        }

        public Command EDITER
        {
            get
            {
                return _editer;
            }
        }

        async private void Editer()
        {

            USER.ImageId = int.Parse(IMAGEID, System.Globalization.CultureInfo.InvariantCulture);
            Console.WriteLine("EditTest:" + USER.FirstName + "|" + USER.LastName+"|"+USER.ImageId);
            await SingletonRestService.RS.EditCountAsync(USER.FirstName,USER.LastName, USER.ImageId);
        }

    }
}

