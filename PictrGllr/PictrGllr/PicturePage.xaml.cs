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
using System.Windows.Media.Imaging;
using System.IO;
using Microsoft.Xna.Framework.Media;
using System.Windows.Resources;
using System.IO.IsolatedStorage;

namespace PictrGllr
{

    // Page to view single picture
    public partial class PicturePage : PhoneApplicationPage
    {
        public PicturePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Find selected image index from parameters
            IDictionary<string, string> parameters = this.NavigationContext.QueryString;
            if (parameters.ContainsKey("SelectedIndex") && !parameters.ContainsKey("Method"))
            {
                var selectedIndex = Int32.Parse(parameters["SelectedIndex"]);
                Uri uriR = new Uri(App.Current.pvm.Items[selectedIndex].ThumbnailUrl);
                BitmapImage imgSourceR = new BitmapImage(uriR);
                bigPicture.Source = imgSourceR;
            }
            else if (parameters.ContainsKey("SelectedIndex") && parameters.ContainsKey("Method"))
            {
                var selectedIndex = Int32.Parse(parameters["SelectedIndex"]);
                Uri uriR = new Uri(App.Current.pnuvm.Items[selectedIndex].LargeUrl);
                BitmapImage imgSourceR = new BitmapImage(uriR);
                bigPicture.Source = imgSourceR;
            }
        }
        // Below code for pinch zoom
        // these two fully define the zoom state:
        private double TotalImageScale = 1d;
        private Point ImagePosition = new Point(0, 0);

        private Point _oldFinger1;
        private Point _oldFinger2;
        private double _oldScaleFactor;

        private void OnPinchStarted(object s, PinchStartedGestureEventArgs e)
        {
            _oldFinger1 = e.GetPosition(bigPicture, 0);
            _oldFinger2 = e.GetPosition(bigPicture, 1);
            _oldScaleFactor = 1;
        }

        private void OnPinchDelta(object s, PinchGestureEventArgs e)
        {
            var scaleFactor = e.DistanceRatio / _oldScaleFactor;

            var currentFinger1 = e.GetPosition(bigPicture, 0);
            var currentFinger2 = e.GetPosition(bigPicture, 1);

            var translationDelta = GetTranslationDelta(
                currentFinger1,
                currentFinger2,
                _oldFinger1,
                _oldFinger2,
                ImagePosition,
                scaleFactor);

            _oldFinger1 = currentFinger1;
            _oldFinger2 = currentFinger2;
            _oldScaleFactor = e.DistanceRatio;

            UpdateImage(scaleFactor, translationDelta);
        }

        private void UpdateImage(double scaleFactor, Point delta)
        {
            TotalImageScale *= scaleFactor;
            ImagePosition = new Point(ImagePosition.X + delta.X, ImagePosition.Y + delta.Y);

            var transform = (CompositeTransform)bigPicture.RenderTransform;
            transform.ScaleX = TotalImageScale;
            transform.ScaleY = TotalImageScale;
            transform.TranslateX = ImagePosition.X;
            transform.TranslateY = ImagePosition.Y;
        }

        private Point GetTranslationDelta(
            Point currentFinger1, Point currentFinger2,
            Point oldFinger1, Point oldFinger2,
            Point currentPosition, double scaleFactor)
        {
            var newPos1 = new Point(
                currentFinger1.X + (currentPosition.X - oldFinger1.X) * scaleFactor,
                currentFinger1.Y + (currentPosition.Y - oldFinger1.Y) * scaleFactor);

            var newPos2 = new Point(
                currentFinger2.X + (currentPosition.X - oldFinger2.X) * scaleFactor,
                currentFinger2.Y + (currentPosition.Y - oldFinger2.Y) * scaleFactor);

            var newPos = new Point(
                (newPos1.X + newPos2.X) / 2,
                (newPos1.Y + newPos2.Y) / 2);

            return new Point(
                newPos.X - currentPosition.X,
                newPos.Y - currentPosition.Y);
        }


        // When holding picture, offer possibility to save the picture to phone
        private void bigPicture_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MessageBoxResult mb = MessageBox.Show(AppResources.DownloadQuestion, AppResources.DownloadTitle, MessageBoxButton.OKCancel);

            if (mb == MessageBoxResult.OK)
            {
                IDictionary<string, string> parameters = this.NavigationContext.QueryString;
                if (parameters.ContainsKey("SelectedIndex") && !parameters.ContainsKey("Method"))
                {
                    var selectedIndex = Int32.Parse(parameters["SelectedIndex"]);
                    SaveImage(App.Current.pvm.Items[selectedIndex].ThumbnailUrl);

                }
                else if (parameters.ContainsKey("SelectedIndex") && parameters.ContainsKey("Method"))
                {
                    var selectedIndex = Int32.Parse(parameters["SelectedIndex"]);
                    SaveImage(App.Current.pnuvm.Items[selectedIndex].LargeUrl);
                }


            }

        }

        private static string CreateFileName()
        {
            Guid g = Guid.NewGuid();
            var filename = g.ToString().Replace("-", "");
            return filename + ".jpg";
        }
        // Save picture to phone
        private void SaveImage(string source)
        {
            var fileName = CreateFileName();
            Uri url;
            if (Uri.TryCreate(source, UriKind.Absolute, out url))
            {
                WriteableBitmap wr;
                BitmapImage img = new BitmapImage(url);
                img.CreateOptions = BitmapCreateOptions.None;
                // When image is ready, show must go on.
                img.ImageOpened += (s, ee) =>
                {
                    wr = new WriteableBitmap((BitmapImage)s);
                    //fileName += ".jpg"; // we dont need that
                    var myStore = IsolatedStorageFile.GetUserStoreForApplication();
                    if (myStore.FileExists(fileName))
                    {
                        myStore.DeleteFile(fileName);
                    }
                    IsolatedStorageFileStream myFileStream = myStore.CreateFile(fileName);
                    //WriteableBitmap wr = img; // image source already given
                    wr.SaveJpeg(myFileStream, wr.PixelWidth, wr.PixelHeight, 0, 85);
                    myFileStream.Close();

                    // Create a new stream from isolated storage, and save the JPEG file to the media library on Windows Phone.
                    myFileStream = myStore.OpenFile(fileName, FileMode.Open, FileAccess.Read);
                    MediaLibrary library = new MediaLibrary();
                    //byte[] buffer = ToByteArray(qrImage);
                    library.SavePicture(fileName, myFileStream);
                };
            }
        }
    }
}