﻿using ROBO_Mobile.ViewModels;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ROBO_Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChartsPage : ContentPage
    {
        public ChartsPage()
        {
            InitializeComponent();
            BindingContext = ViewModel = new ChartsViewModel();
        }

        ChartsViewModel ViewModel { get; }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel.OnAppearing();
        }
    }
}