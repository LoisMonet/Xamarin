using System;
using System.Collections.Generic;
using Fourplaces.ViewModels;
using Storm.Mvvm.Forms;
using Xamarin.Forms;

namespace Fourplaces.Views
{
    public partial class Inscription : BaseContentPage
    {
        public Inscription()
        {
            InitializeComponent();
            BindingContext = new InscriptionViewModel();

            Console.WriteLine("Inscritpion");
        }
    }
}
