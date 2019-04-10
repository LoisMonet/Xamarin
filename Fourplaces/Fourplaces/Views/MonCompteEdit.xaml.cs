using System;
using System.Collections.Generic;
using Fourplaces.ViewModels;
using Storm.Mvvm.Forms;
using TD.Api.Dtos;
using Xamarin.Forms;

namespace Fourplaces.Views
{
    public partial class MonCompteEdit : BaseContentPage
    {
        public MonCompteEdit()
        {
            InitializeComponent();
            BindingContext = new MonCompteEditViewModel();
        }
    }
}
