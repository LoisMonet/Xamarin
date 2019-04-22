using System;
using System.Threading.Tasks;
using Fourplaces.Models;
using Fourplaces.Models.Exceptions;
using Storm.Mvvm;
using TD.Api.Dtos;
using Xamarin.Forms;

namespace Fourplaces.ViewModels
{
    public class MonMDPEditViewModel : ViewModelBase
    {

        //private UpdatePasswordRequest _upr;
        private Command _editer;
        private string exception;
        private string _reussi;

        public MonMDPEditViewModel()
        {

            _editer = new Command(() => Editer());
        }

        public String OPWD { get; set; }

        public String NPWD { get; set; }


        public Command EDITER
        {
            get
            {
                return _editer;
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

        async private void Editer()
        {
            EXCEPTION = "";
            Reussi = "";

            try
            {
                await SingletonRestService.RS.EditPWAsync(OPWD, NPWD);
                Reussi = "Modification effectuée";
            }
            //catch(AuthenticationException ae)
            catch (NoConnectE e)
            {
                EXCEPTION = e.ExceptionMess;
            }
            catch (PwdCompteE e)
            {
                EXCEPTION = e.ExceptionMess;
            }
            catch (Exception e)
            {
                EXCEPTION = e.Message;
            }

        }




    }
}

