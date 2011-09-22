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
            InitializeComponent();
            this.DataContext = this;
        }

        public string UserName = "";
        public string Password = "";
        public bool SavePassword = false;
        public string ErrorMessage = "";

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
            UserName = (string)App.Settings["UserName"];
            //Password = (string)App.Settings["Password"]; // no longer using this way, using passwordvault instead
            try
            {
                var passwords = App.PasswordVault.FindAllByResource("redditMetro");
                foreach (var pass in passwords)
                {
                    if (pass.UserName == UserName)
                    {
                        pass.RetrievePassword();
                        Password = pass.Password;
                    }
                }
            }
            catch (Exception)
            {
                // means we don't have a password, ignore this error
            }
            SavePassword = (bool)App.Settings["SavePassword"];

            ErrorMessage = "";
            txtUsername.Text = UserName;
            txtPassword.Password = Password;
            tglSavePassword.IsOn = SavePassword;
        }

        private void tglSavePassword_Toggled(object sender, RoutedEventArgs e)
        {
            SavePassword = tglSavePassword.IsOn;
        }

        // Hate that we have to do this, but LostFocus isn't called on a soft-dismiss :(
        private void txtPassword_KeyUp(object sender, Windows.UI.Xaml.Input.KeyEventArgs e)
        {
            Password = txtPassword.Password;
        }
    }
}
