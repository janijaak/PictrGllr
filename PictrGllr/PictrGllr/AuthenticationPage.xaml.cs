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

    // Code for authentication. Web browser is used to authentication with OAuth.
    public partial class AuthenticationPage : PhoneApplicationPage
    {
        // Callback url
        private const string callbackUrl = "http://localhost/";

        private OAuthRequestToken requestToken = null;

        public AuthenticationPage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            Flickr f = FlickrManager.GetInstance();

            // obtain the request token from Flickr
            f.OAuthGetRequestTokenAsync(callbackUrl, r =>
            {
                // Check if an error was returned
                if (r.Error != null)
                {
                    Dispatcher.BeginInvoke(() => { MessageBox.Show(AppResources.RequestTokenError + r.Error.Message); });
                    return;
                }

                // Get the request token
                requestToken = r.Result;

                // get Authorization url
                string url = f.OAuthCalculateAuthorizationUrl(requestToken.Token, AuthLevel.Write);
                // Replace www.flickr.com with m.flickr.com for mobile version
                url = url.Replace("www.flickr.com", "m.flickr.com");

                // Navigate to url
                Dispatcher.BeginInvoke(() => { WebBrowser1.Navigate(new Uri(url)); });
                
            });
        
        }

        private void WebBrowser1_Navigating(object sender, NavigatingEventArgs e)
        {
            
            if (!e.Uri.AbsoluteUri.StartsWith(callbackUrl)) return;

           var oauthVerifier = e.Uri.Query.Split('&')
                .Where(s => s.Split('=')[0] == "oauth_verifier")
                .Select(s => s.Split('=')[1])
                .FirstOrDefault();

            if (String.IsNullOrEmpty(oauthVerifier))
            {
                MessageBox.Show(AppResources.VerifierError + e.Uri.AbsoluteUri);
                return;
            }

            
            e.Cancel = true;

         
            Flickr f = FlickrManager.GetInstance();

            f.OAuthGetAccessTokenAsync(requestToken, oauthVerifier, r =>
            {
                if (r.Error != null)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        MessageBox.Show(AppResources.AccessTokenError + r.Error.Message);
                    });
                    return;
                }
                WebBrowser1.Visibility = System.Windows.Visibility.Collapsed;
                OAuthAccessToken accessToken = r.Result;

                // Save the oauth token for later use
                FlickrManager.OAuthToken = accessToken.Token;
                FlickrManager.OAuthTokenSecret = accessToken.TokenSecret;
                // Save user for later use
                App.Current.uvm.AddUser(accessToken.UserId, accessToken.Username);
                               
                Dispatcher.BeginInvoke(() =>
                {
                   NavigationService.GoBack();
                });

            });

        }
    }
}