using System;
using System.IO;
using System.Threading.Tasks;
using Fourplaces.Models;
using Fourplaces.Models.Exceptions;
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
        private Command _picture;

        private ImageSource _image;
        private byte[] imageB;

        private bool typeP=false;
        private string exception;
        private string _reussi;




        public MonCompteEditViewModel()
        {

    
            _editer = new Command(() => Editer());
            _picture = new Command(() => ChoosePicture());


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

        public String Reussi
        {
            get
            {
                return _reussi;
            }

            set
            {
                SetProperty(ref _reussi, value);
            }
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
                SetProperty(ref _user, value);
                IMAGE = USER.SOURCEIMAGE;



            }
        }

        public ImageSource IMAGE
        {

            get
            {

                return _image;
            }
            set
            {
                SetProperty(ref _image, value);

            }


        }

        public bool TYPEP
        {
            get
            {
                return (typeP);
            }

            set
            {
                SetProperty(ref typeP, value);
            }
        }


        public Command EDITER
        {
            get
            {
                return _editer;
            }
        }

        public Command PICTURE
        {
            get
            {
                return _picture;
            }
        }

        async private void Editer()
        {
            EXCEPTION = "";
            Reussi = "";
            try
            {
                await SingletonRestService.RS.EditCountAsync(USER.FirstName, USER.LastName, USER.ImageId, imageB);
                Reussi = "Modification effectuée";
            }
            catch (NoConnectE e)
            {
                EXCEPTION = e.ExceptionMess;
            }
            catch (EditCompteE e)
            {
                EXCEPTION = e.ExceptionMess;
            }
            catch (Exception e)
            {
                EXCEPTION = e.Message;
            }

        }

        public async void ChoosePicture()
        {

            imageB = await SingletonRestService.RS.SendPicture(TYPEP);
            if (imageB != null)
            {
                IMAGE = ImageSource.FromStream(() => new MemoryStream(imageB));
            }
        }

    }
}

