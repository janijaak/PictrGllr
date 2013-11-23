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
using System.Text;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Windows.Navigation;
using Microsoft.Phone.Shell;
using PictrGllr.Model;

namespace PictrGllr
{
    public partial class SearchPage : PhoneApplicationPage
    {
        public SearchPage()
        {
            InitializeComponent();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SearchTextBox.Text)) return;
            var url = CreateRequest(SearchTextBox.Text);
            MakeRequest(url);
        }


        // Create request URL
        public static string CreateRequest(string tags)
        {
            tags = tags.Replace(' ', '+');
            string UrlRequest = String.Format("http://api.flickr.com/services/rest/?method=flickr.photos.search&api_key={0}&tags={1}&format=json&nojsoncallback=1", FlickrManager.ApiKey, tags);
            return (UrlRequest);
        }

        // Make request to fetch weather data
        public void MakeRequest(string url)
        {
            WebClient client = new WebClient();
            client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(client_DownloadStringCompleted);
            client.DownloadStringAsync(new Uri(url));
        }


        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {

            if ((e.Result != null) && (e.Error == null))
            {
                string jsonString = e.Result.ToString();
                // If no data is found, show error message
                using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString)))
                {
                    var ser = new DataContractJsonSerializer(typeof(PictureDataContract));
                    try
                    {
                        try
                        {

                            PictureDataContract obj = (PictureDataContract)ser.ReadObject(ms);
                            App.Current.pnuvm.ClearPictures();
                            int i = 0;
                            foreach (var p in obj.photos.photo)
                            {
                                App.Current.pnuvm.AddPicture(i, p.id, p.owner, p.secret, p.server, p.farm);
                                i++;
                            }

                            Deployment.Current.Dispatcher.BeginInvoke(() =>
                            {
                                PictureView.DataContext = App.Current.pnuvm;
                            });
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex.Message);
                        }

                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }
                }



            }
            else if (e.Error != null)
            {
                System.Diagnostics.Debug.WriteLine(e.Error.Message);
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {

        }



    }
}