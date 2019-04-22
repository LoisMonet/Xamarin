using System;
using System.Collections.Generic;
using Fourplaces.ViewModels;
using Storm.Mvvm.Forms;
using Xamarin.Forms;

namespace Fourplaces.Views
{
    public partial class Connexion : BaseContentPage
    {
        public Connexion()
        {
            InitializeComponent();
            BindingContext = new ConnexionViewModel();

        }
    }
}
