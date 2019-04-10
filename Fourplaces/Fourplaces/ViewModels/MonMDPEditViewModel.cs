using System;
using System.Threading.Tasks;
using Fourplaces.Models;
using Storm.Mvvm;
using TD.Api.Dtos;
using Xamarin.Forms;

namespace Fourplaces.ViewModels
{
    public class MonMDPEditViewModel : ViewModelBase
    {

        //private UpdatePasswordRequest _upr;
        private Command _editer;

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

        async private void Editer()
        {
            await SingletonRestService.RS.EditPWAsync(OPWD, NPWD);
        }




    }
}

