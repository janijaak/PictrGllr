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
    public class UserViewModel
    {

        private readonly ObservableCollection<UserModel> userInfoCollection = new ObservableCollection<UserModel>();

        public UserViewModel()
        {
        }

        public void AddUser(string userid, string username)
        {
            userInfoCollection.Clear();
            userInfoCollection.Add(new UserModel()
            {
                UserId = userid,
                UserName = username
            });
        }

        public ObservableCollection<UserModel> Items
        {
            get { return userInfoCollection; }
        }

    }
}
