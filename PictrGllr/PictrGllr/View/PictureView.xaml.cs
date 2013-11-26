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
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace PictrGllr.View
{

    // View to show list of pictures in gallery and search pages
    public partial class PictureView : UserControl
    {
        public PictureView()
        {
            InitializeComponent();
        }

        private void AlbumListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AlbumListBox.SelectedIndex == -1) return;
            if ((Application.Current.RootVisual as PhoneApplicationFrame).Source.OriginalString.Equals("/SearchPage.xaml"))
            {
                (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/PicturePage.xaml?SelectedIndex=" + AlbumListBox.SelectedIndex + "&Method=search", UriKind.Relative));
            }
            else if ((Application.Current.RootVisual as PhoneApplicationFrame).Source.OriginalString.Equals("/GalleryPage.xaml"))
            {
                (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/PicturePage.xaml?SelectedIndex=" + AlbumListBox.SelectedIndex, UriKind.Relative));
            }
        }
    }
}
