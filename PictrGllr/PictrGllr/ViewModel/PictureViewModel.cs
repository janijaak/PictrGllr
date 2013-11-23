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
    public class PictureViewModel
    {

        private readonly ObservableCollection<PictureModel> pictureCollection = new ObservableCollection<PictureModel>();

        public PictureViewModel()
        {
        }


        public void ClearPictures()
        {
            pictureCollection.Clear();
        }
        public void AddPicture(int id, string url)
        {
            pictureCollection.Add(new PictureModel()
            {
                Id = id,
                ThumbnailUrl = url
            });
        }

        public ObservableCollection<PictureModel> Items
        {
            get { return pictureCollection; }
        }

    }
}
