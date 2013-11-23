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
using System.IO.IsolatedStorage;
using FlickrNet;

namespace PictrGllr
{
    public class FlickrManager
    {
        public const string ApiKey = "366dc42d088453ab5355b6abc4098b68";
        public const string SharedSecret = "137db9af1c4ae340";

        public static Flickr GetInstance()
        {
            return new Flickr(ApiKey, SharedSecret);
        }

        public static Flickr GetAuthInstance()
        {
            var f = new Flickr(ApiKey, SharedSecret);
            f.OAuthAccessToken = OAuthToken;
            f.OAuthAccessTokenSecret = OAuthTokenSecret;
            return f;
        }

        public static string OAuthToken
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains("OAuthToken"))
                    return IsolatedStorageSettings.ApplicationSettings["OAuthToken"] as string;
                else
                    return null;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["OAuthToken"] = value;
            }
        }

        public static string OAuthTokenSecret
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains("OAuthTokenSecret"))
                    return IsolatedStorageSettings.ApplicationSettings["OAuthTokenSecret"] as string;
                else
                    return null;
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings["OAuthTokenSecret"] = value;
            }
        }
    }
}
