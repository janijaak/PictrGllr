using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using PictrGllr.View;
using System.Threading;
using FlickrNet;

namespace PictrGllr
{
    public partial class MainPage : PhoneApplicationPage
    {
        private BackgroundWorker backroungWorker;
        Popup popup;
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            OpenSplashScreen();
            
        }

        private void OpenSplashScreen()
        {
            this.popup = new Popup();
            this.popup.Child = new SplashScreenControl();
            this.popup.IsOpen = true;
            LoadData();
        }

        private void LoadData()
        {
            backroungWorker = new BackgroundWorker();
            backroungWorker.DoWork +=
                       new DoWorkEventHandler(backroungWorker_DoWork);
            backroungWorker.RunWorkerCompleted +=
                new RunWorkerCompletedEventHandler(backroungWorker_RunWorkerCompleted);
            backroungWorker.RunWorkerAsync();
        }
        void backroungWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                this.popup.IsOpen = false;
               
            }
        );
        }
        void backroungWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(2000);
            
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
           
            Flickr f = FlickrManager.GetAuthInstance();
            if (App.Current.uvm.Items.Count != 0)
            {
                LoginBtn.Visibility = System.Windows.Visibility.Collapsed;
                GalleryBtn.Visibility = System.Windows.Visibility.Visible;
                SearchBtn.Visibility = System.Windows.Visibility.Visible;
                ApplicationBar.IsVisible = true;
                
            }
            else
            {
                LoginBtn.Visibility = System.Windows.Visibility.Visible;
                GalleryBtn.Visibility = System.Windows.Visibility.Collapsed;
                SearchBtn.Visibility = System.Windows.Visibility.Collapsed;
                ApplicationBar.IsVisible = false;
            }
        }

        private void GalleryBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/GalleryPage.xaml", UriKind.Relative));
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/AuthenticationPage.xaml", UriKind.Relative));
        }

        private void LogoutMenuItem_Click(object sender, EventArgs e)
        {
            App.Current.LogOut();
            
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/CameraPage.xaml", UriKind.Relative));
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/SearchPage.xaml", UriKind.Relative));
        }

        
    }
}