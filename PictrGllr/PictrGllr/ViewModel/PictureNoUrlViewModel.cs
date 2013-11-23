using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using PictrGllr.Model;

namespace PictrGllr.ViewModel
{
    public class PictureNoUrlViewModel
    {

        private readonly ObservableCollection<PictureNoUrlModel> pictureCollection = new ObservableCollection<PictureNoUrlModel>();

        public PictureNoUrlViewModel()
        {
        }


        public void ClearPictures()
        {
            pictureCollection.Clear();
        }
        
        public void AddPicture(int ordernumber, string id, string owner, string secret, string server, int farm)
        {

            var largeUrl = String.Format("http://farm{0}.staticflickr.com/{1}/{2}_{3}_z.jpg", farm, server, id, secret);
            var thumbnailUrl = String.Format("http://farm{0}.staticflickr.com/{1}/{2}_{3}_t.jpg", farm, server, id, secret);

            pictureCollection.Add(new PictureNoUrlModel()
            {
                OrderNumber = ordernumber,
                Id = id,
                Owner = owner,
                Secret = secret,
                Server = server,
                Farm = farm,
                LargeUrl = largeUrl,
                ThumbnailUrl = thumbnailUrl
            });
        }

        public ObservableCollection<PictureNoUrlModel> Items
        {
            get { return pictureCollection; }
        }

    }
}
