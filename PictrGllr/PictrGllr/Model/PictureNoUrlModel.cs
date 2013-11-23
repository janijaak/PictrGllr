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

namespace PictrGllr.Model
{
    public class PictureNoUrlModel
    {
        public int OrderNumber { get; set; }
        public string Id { get; set; }
        public string Owner { get; set; }
        public string Secret { get; set; }
        public string Server { get; set; }
        public int Farm { get; set; }
        public string ThumbnailUrl { get; set; }
        public string LargeUrl { get; set; }

    }
}
