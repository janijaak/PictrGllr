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
using FlickrNet;

namespace PictrGllr
{
    public partial class GalleryPage : PhoneApplicationPage
    {
        public GalleryPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            Flickr f = FlickrManager.GetAuthInstance();


            var userid = App.Current.uvm.Items[0].UserId;

            if (!string.IsNullOrEmpty(userid))
            {
                PhotoSearchOptions options = new PhotoSearchOptions();
                options.UserId = userid;
                f.PhotosSearchAsync(options, r =>
                {
                    if (r.Error != null)
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            MessageBox.Show(AppResources.FlickrError + r.Error.Message);
                        });
                        return;
                    }

                    PhotoCollection photos = r.Result;
                    var i = 0;
                    App.Current.pvm.ClearPictures();
                    foreach (Photo p in photos)
                    {
                        App.Current.pvm.AddPicture(i, p.LargeUrl);
                        i++;
                    }

                    Dispatcher.BeginInvoke(() =>
                    {
                        PictureView.DataContext = App.Current.pvm;
                    });
                });
            }

        }


    }
}