using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace redditMetro.Controls
{
    public sealed partial class AccountSettingsPanel
    {
        public AccountSettingsPanel()
        {
            this.DataContext = this;
            UserName = (string)App.Settings["UserName"];
            Password = (string)App.Settings["Password"];
            SavePassword = (bool)App.Settings["SavePassword"];
            
            ErrorMessage = "";
            InitializeComponent();
        }

        public string UserName { get; set; }
        public string Password { get; set; }
        public bool SavePassword { get; set; }
        public string ErrorMessage { get; set; }

        private void txtUsername_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtUsername.Text != UserName)
                App.isLoggedIn = false;
            UserName = txtUsername.Text;
        }

        private void tglSavePassword_LostFocus(object sender, RoutedEventArgs e)
        {
            SavePassword = tglSavePassword.IsOn;
        }

        private void txtPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            Password = txtPassword.Password;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            txtUsername.Text = UserName;
            txtPassword.Password = Password;
            tglSavePassword.IsOn = SavePassword;
        }
    }
}
