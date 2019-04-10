using System;

using Fourplaces.ViewModels;
using Fourplaces.Views;
using Storm.Mvvm;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Fourplaces
{
    public partial class App : MvvmApplication
    {
        public App():base(()=> new MainView())
        {
            InitializeComponent();

            //MainPage = new NavigationPage(new MainView());
        }




        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

      
    
    }
}
