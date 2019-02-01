using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BinCompeteSoftUWP.Pages
{
    public sealed partial class SettingsContentDialog : ContentDialog
    {
        #region Class variables
        private string ServerName = "";
        private string DatabaseName = "";
        private string UserName = "";
        private string UserPassword = "";

        ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        LoginPage LoginPage;
        #endregion

        #region Class constructors
        public SettingsContentDialog(LoginPage loginPage)
        {
            LoginPage = loginPage;

            this.InitializeComponent();

            FillConnectiondetails();
        }
        #endregion

        #region Class event handlers
        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Check if data is filled out.
            if(!string.IsNullOrWhiteSpace(ServerNameTextBox.Text) && !string.IsNullOrWhiteSpace(DatabaseNameTextBox.Text) && !string.IsNullOrWhiteSpace(UserNameTextBox.Text) && !string.IsNullOrWhiteSpace(UserPasswordTextBox.Password))
            {
                ErrorTextBlock.Visibility = Visibility.Collapsed;

                ServerName = ServerNameTextBox.Text;
                DatabaseName = DatabaseNameTextBox.Text;
                UserName = UserNameTextBox.Text;
                UserPassword = UserPasswordTextBox.Password;

                // Save the settings.
                localSettings.Values["ServerName"] = ServerName;
                localSettings.Values["DatabaseName"] = DatabaseName;
                localSettings.Values["UserName"] = UserName;
                localSettings.Values["UserPassword"] = UserPassword;

                LoginPage.ReadConnectionSettings();
            }
            else
            {
                ErrorTextBlock.Visibility = Visibility.Visible;

                args.Cancel = true;
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
        #endregion

        #region Class methods
        /// <summary>
        /// Get the connection details from local app settings.
        /// </summary>
        private void FillConnectiondetails()
        {
            // Load the settings from local app settings.
            object ip = localSettings.Values["ServerName"];
            object dbName = localSettings.Values["DatabaseName"];
            object userId = localSettings.Values["UserName"];
            object password = localSettings.Values["UserPassword"];

            // Check if data exists.
            if(ip != null)
            {
                ServerName = ip.ToString();
            }
            
            if(dbName != null)
            {
                DatabaseName = dbName.ToString();
            }
            
            if(userId != null)
            {
                UserName = userId.ToString();
            }

            if(password != null)
            {
                UserPassword = password.ToString();
            }

            // Fill the text blocks with the correct values.
            ServerNameTextBox.Text = ServerName;
            DatabaseNameTextBox.Text = DatabaseName;
            UserNameTextBox.Text = UserName;
            UserPasswordTextBox.Password = UserPassword;
        }
        #endregion
    }
}
