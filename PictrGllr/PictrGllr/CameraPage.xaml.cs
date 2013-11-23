// Directives
using Microsoft.Devices;
using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework.Media;
using Microsoft.Phone.Controls;
using System;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using FlickrNet;
using System.Threading;

namespace PictrGllr
{
    public partial class CameraPage : PhoneApplicationPage
    {

        // Variables
        private int savedCounter = 0;
        PhotoCamera cam;
        MediaLibrary library = new MediaLibrary();

        public CameraPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {

            // Check to see if the camera is available on the phone.
            if ((PhotoCamera.IsCameraTypeSupported(CameraType.Primary) == true) ||
                 (PhotoCamera.IsCameraTypeSupported(CameraType.FrontFacing) == true))
            {
                // Initialize the camera, when available.
                if (PhotoCamera.IsCameraTypeSupported(CameraType.FrontFacing))
                {
                    // Use front-facing camera if available.
                    cam = new Microsoft.Devices.PhotoCamera(CameraType.FrontFacing);
                }
                else
                {
                    // Otherwise, use standard camera on back of phone.
                    cam = new Microsoft.Devices.PhotoCamera(CameraType.Primary);
                }

                // Event is fired when the PhotoCamera object has been initialized.
                cam.Initialized += new EventHandler<Microsoft.Devices.CameraOperationCompletedEventArgs>(cam_Initialized);

                // Event is fired when the capture sequence is complete.
                cam.CaptureCompleted += new EventHandler<CameraOperationCompletedEventArgs>(cam_CaptureCompleted);

                // Event is fired when the capture sequence is complete and an image is available.
                cam.CaptureImageAvailable += new EventHandler<Microsoft.Devices.ContentReadyEventArgs>(cam_CaptureImageAvailable);



                //Set the VideoBrush source to the camera.
                viewfinderBrush.SetSource(cam);
            }
            else
            {
                // The camera is not supported on the phone.
                this.Dispatcher.BeginInvoke(delegate()
                {
                    // Write message.
                    //txtInfo.Text = AppResources.NoCameraText;
                });

                // Disable UI.
                ShutterButton.IsEnabled = false;
            }
        }
        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (cam != null)
            {
                // Dispose camera to minimize power consumption and to expedite shutdown.
                cam.Dispose();

                // Release memory, ensure garbage collection.
                cam.Initialized -= cam_Initialized;
                cam.CaptureCompleted -= cam_CaptureCompleted;
                cam.CaptureImageAvailable -= cam_CaptureImageAvailable;
            }
        }

        // Update the UI if initialization succeeds.
        void cam_Initialized(object sender, Microsoft.Devices.CameraOperationCompletedEventArgs e)
        {
            if (e.Succeeded)
            {
                this.Dispatcher.BeginInvoke(delegate()
                {
                    // Write message.
                    //txtInfo.Text = AppResources.CameraReady;
                });
            }
        }

        // Ensure that the viewfinder is upright in LandscapeRight.
        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            if (cam != null)
            {
                // LandscapeRight rotation when camera is on back of phone.
                int landscapeRightRotation = 180;

                // Change LandscapeRight rotation for front-facing camera.
                if (cam.CameraType == CameraType.FrontFacing) landscapeRightRotation = -180;

                // Rotate video brush from camera.
                if (e.Orientation == PageOrientation.LandscapeRight)
                {
                    // Rotate for LandscapeRight orientation.
                    viewfinderBrush.RelativeTransform =
                        new CompositeTransform() { CenterX = 0.5, CenterY = 0.5, Rotation = landscapeRightRotation };
                }
                else
                {
                    // Rotate for standard landscape orientation.
                    viewfinderBrush.RelativeTransform =
                        new CompositeTransform() { CenterX = 0.5, CenterY = 0.5, Rotation = 0 };
                }
            }

            base.OnOrientationChanged(e);
        }

        private void ShutterButton_Click(object sender, RoutedEventArgs e)
        {
            if (cam != null)
            {
                try
                {
                    // Start image capture.
                    cam.CaptureImage();
                }
                catch (Exception ex)
                {
                    this.Dispatcher.BeginInvoke(delegate()
                    {
                        // Cannot capture an image until the previous capture has completed.
                        //txtInfo.Text = ex.Message;
                    });
                }
            }
        }

        void cam_CaptureCompleted(object sender, CameraOperationCompletedEventArgs e)
        {
            // Increments the savedCounter variable used for generating JPEG file names.
            savedCounter++;
        }

        // Informs when full resolution photo has been taken, saves to local media library and the local folder.
        void cam_CaptureImageAvailable(object sender, Microsoft.Devices.ContentReadyEventArgs e)
        {
            string fileName = savedCounter + ".jpg";

            try
            {   // Write message to the UI thread.
                Deployment.Current.Dispatcher.BeginInvoke(delegate()
                {
                    //txtInfo.Text = AppResources.SavingText;
                });

                // Save photo to the media library camera roll.
                library.SavePictureToCameraRoll(fileName, e.ImageStream);

                // Write message to the UI thread.
                Deployment.Current.Dispatcher.BeginInvoke(delegate()
                {
                    //txtInfo.Text = AppResources.SavedToRoll;

                });

                // Set the position of the stream back to start
                e.ImageStream.Seek(0, SeekOrigin.Begin);

                // Save photo as JPEG to the local folder.
                using (IsolatedStorageFile isStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream targetStream = isStore.OpenFile(fileName, FileMode.Create, FileAccess.Write))
                    {
                        // Initialize the buffer for 4KB disk pages.
                        byte[] readBuffer = new byte[4096];
                        int bytesRead = -1;

                        // Copy the image to the local folder. 
                        while ((bytesRead = e.ImageStream.Read(readBuffer, 0, readBuffer.Length)) > 0)
                        {
                            targetStream.Write(readBuffer, 0, bytesRead);
                        }
                    }
                }

                // Write message to the UI thread.
                Deployment.Current.Dispatcher.BeginInvoke(delegate()
                {
                    //txtInfo.Text = AppResources.SavedToLocal;

                });

                Deployment.Current.Dispatcher.BeginInvoke(delegate()
                {
                    previewImage.Visibility = System.Windows.Visibility.Visible;
                    previewImage.Source = GetImageFromIsolatedStorage(fileName);
                    FileNameHidden.Text = fileName;

                    ShutterButton.Visibility = System.Windows.Visibility.Collapsed;
                    UploadButton.Visibility = System.Windows.Visibility.Visible;
                    NewPictureButton.Visibility = System.Windows.Visibility.Visible;


                });

            }
            finally
            {
                // Close image stream
                e.ImageStream.Close();

            }

        }

        private static BitmapImage GetImageFromIsolatedStorage(string imageName)
        {


            var bimg = new BitmapImage();
            using (var iso = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var stream = iso.OpenFile(imageName, FileMode.Open, FileAccess.Read))
                {

                    bimg.SetSource(stream);
                }
            }

            return bimg;
        }

        private static string CreateFileName()
        {
            Guid g = Guid.NewGuid();
            var filename = g.ToString().Replace("-", "");
            return filename + ".jpg";
        }

        private void NewPictureButton_Click(object sender, RoutedEventArgs e)
        {
            NewPictureButton.Visibility = System.Windows.Visibility.Collapsed;
            UploadButton.Visibility = System.Windows.Visibility.Collapsed;
            ShutterButton.Visibility = System.Windows.Visibility.Visible;
            previewImage.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult mb = MessageBox.Show(AppResources.UploadQuestion, AppResources.UploadTitle, MessageBoxButton.OKCancel);
            if (mb == MessageBoxResult.OK)
            {

                Flickr f = FlickrManager.GetAuthInstance();

                using (var iso = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (!string.IsNullOrEmpty(FileNameHidden.Text))
                    {
                        using (var stream = iso.OpenFile(FileNameHidden.Text, FileMode.Open, FileAccess.Read))
                        {
                            f.UploadPictureAsync(stream, CreateFileName(), null, null, null, false, false, false, ContentType.Photo, SafetyLevel.Restricted, HiddenFromSearch.Hidden, (result) => Console.WriteLine(result));
                            previewImage.Visibility = System.Windows.Visibility.Collapsed;
                            NewPictureButton.Visibility = System.Windows.Visibility.Collapsed;
                            UploadButton.Visibility = System.Windows.Visibility.Collapsed;
                            ShutterButton.Visibility = System.Windows.Visibility.Visible;
                        }
                    }
                }
            }

        }




    }
}