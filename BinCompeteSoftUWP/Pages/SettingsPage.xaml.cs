using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BinCompeteSoftUWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        #region Class constructors
        public SettingsPage()
        {
            this.InitializeComponent();

            string userName = Data.Instance.LoggedInUser.Name;

            // This will get the initials of a name.
            string[] separatedName = userName.Split(" ");
            string userInitials = separatedName[0][0].ToString() + separatedName[separatedName.Length - 1][0].ToString();

            UserPersonPicture.DisplayName = userName;
            UserPersonPicture.Initials = userInitials;

            UserNameTextBlock.Text = userName;
            UserNickTextBlock.Text = "@" + Data.Instance.LoggedInUser.Username;

            TextSizeComboBox.SelectedIndex = (int)Data.Instance.FontSizeSettings;
        }
        #endregion

        #region Class event handlers
        private void TextSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check which item was selected.
            var comboBoxItem = e.AddedItems[0] as ComboBoxItem;
            if (comboBoxItem == null) return;
            var content = comboBoxItem.Content as string;

            switch (content)
            {
                case "Smallest":
                    Data.Instance.FontSizeSettings = Data.FontSizeSetting.Smallest;
                    break;
                case "Small":
                    Data.Instance.FontSizeSettings = Data.FontSizeSetting.Small;
                    break;
                case "Normal":
                    Data.Instance.FontSizeSettings = Data.FontSizeSetting.Normal;
                    break;
                case "Medium":
                    Data.Instance.FontSizeSettings = Data.FontSizeSetting.Medium;
                    break;
                case "Large":
                    Data.Instance.FontSizeSettings = Data.FontSizeSetting.Large;
                    break;
            }

            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["FontSize"] = (int)Data.Instance.FontSizeSettings;

            Data.Instance.ChangeFontSize();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog logoutDialog = new ContentDialog
            {
                Title = "Logout",
                Content = "Do you really want to logout?",
                PrimaryButtonText = "Logout",
                CloseButtonText = "Cancel"
            };

            Action<ContentDialogResult> callback = (result) =>
            {
                if (result == ContentDialogResult.Primary)
                {
                    Data.Instance.LogoutUser();
                }
            };

            App.ShowContentDialog(logoutDialog, callback);
        }
        #endregion
    }
}
