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

namespace PictrGllr
{
    public class LocalizedStrings
    {
        /// <summary>
        /// Note that we do not need to specify the namespace when creating the instance
        /// </summary>
        private static AppResources localizedResources = new AppResources();

        /// <summary>
        /// Gets of AppResources class instance
        /// </summary>
        public AppResources AppResources
        {
            get { return localizedResources; }
        }
    }

}
