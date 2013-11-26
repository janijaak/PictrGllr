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
    // Data contract for picture search
    [System.Runtime.Serialization.DataContractAttribute()]
    public partial class PictureDataContract
    {

        [System.Runtime.Serialization.DataMemberAttribute()]
        public Photos photos;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string stat;
    }

    // Type created for JSON at <<root>> --> photos
    [System.Runtime.Serialization.DataContractAttribute(Name = "photos")]
    public partial class Photos
    {

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int page;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int pages;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int perpage;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string total;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public Photo[] photo;
    }

    // Type created for JSON at <<root>> --> photos --> photo
    [System.Runtime.Serialization.DataContractAttribute(Name = "photo")]
    public partial class Photo
    {

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string id;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string owner;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string secret;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string server;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int farm;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string title;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int ispublic;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int isfriend;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int isfamily;
    }

}
