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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PictrGllr.ViewModel;

namespace PictrGllr
{
    public partial class App : Application
    {
        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }
        public UserViewModel uvm { get; private set; }
        public PictureViewModel pvm { get; private set; }
        public PictureNoUrlViewModel pnuvm { get; private set; }

        /// <summary>
        /// Gets the application.
        /// </summary>
        public new static App Current
        {
            get
            {
                return Application.Current as App;
            }
        }
        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }

        }

        public void LogOut()
        {
            FlickrManager.OAuthToken = string.Empty;
            FlickrManager.OAuthTokenSecret = string.Empty;
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/AuthenticationPage.xaml", UriKind.Relative));
        }

        private readonly string UserModelKey = "UserKey";
        private readonly string PictureModelKey = "PictureKey";
        private readonly string PictureNoUrlModelKey = "PictureNoUrlKey";
        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            // Initialize viewmodel
            uvm = new UserViewModel();
            pvm = new PictureViewModel();
            pnuvm = new PictureNoUrlViewModel();
            RootFrame.DataContext = uvm;
            RootFrame.DataContext = pvm;
            RootFrame.DataContext = pnuvm;
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            // Tombstoning, return the state of the viewmodel when returning ot app
            if (PhoneApplicationService.Current.State.ContainsKey(UserModelKey))
            {
                uvm = PhoneApplicationService.Current.State[UserModelKey] as UserViewModel;
            }
            if (PhoneApplicationService.Current.State.ContainsKey(PictureModelKey))
            {
                pvm = PhoneApplicationService.Current.State[PictureModelKey] as PictureViewModel;
            }
            if (PhoneApplicationService.Current.State.ContainsKey(PictureNoUrlModelKey))
            {
                pnuvm = PhoneApplicationService.Current.State[PictureNoUrlModelKey] as PictureNoUrlViewModel;
            }
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            // Tombstoning, save the state of the viewmodel when sent to background
            PhoneApplicationService.Current.State[UserModelKey] = uvm;
            PhoneApplicationService.Current.State[PictureModelKey] = pvm;
            PhoneApplicationService.Current.State[PictureNoUrlModelKey] = pnuvm;
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}