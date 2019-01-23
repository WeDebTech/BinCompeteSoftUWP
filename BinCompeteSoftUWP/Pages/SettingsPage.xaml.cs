using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        private void TextSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check which item was selected.
            var comboBoxItem = e.AddedItems[0] as ComboBoxItem;
            if (comboBoxItem == null) return;
            var content = comboBoxItem.Content as string;
            switch (content)
            {
                case "Smallest":
                    ((Settings)Application.Current.Resources["Settings"]).FontSizeSmaller = 14;
                    ((Settings)Application.Current.Resources["Settings"]).FontSizeSmall = 16;
                    ((Settings)Application.Current.Resources["Settings"]).FontSizeNormal = 18;
                    ((Settings)Application.Current.Resources["Settings"]).FontSizeTitle = 22;
                    break;
                case "Small":
                    ((Settings)Application.Current.Resources["Settings"]).FontSizeSmaller = 18;
                    ((Settings)Application.Current.Resources["Settings"]).FontSizeSmall = 20;
                    ((Settings)Application.Current.Resources["Settings"]).FontSizeNormal = 22;
                    ((Settings)Application.Current.Resources["Settings"]).FontSizeTitle = 26;
                    break;
                case "Normal":
                    ((Settings)Application.Current.Resources["Settings"]).FontSizeSmaller = 22;
                    ((Settings)Application.Current.Resources["Settings"]).FontSizeSmall = 24;
                    ((Settings)Application.Current.Resources["Settings"]).FontSizeNormal = 26;
                    ((Settings)Application.Current.Resources["Settings"]).FontSizeTitle = 30;
                    break;
                case "Medium":
                    ((Settings)Application.Current.Resources["Settings"]).FontSizeSmaller = 24;
                    ((Settings)Application.Current.Resources["Settings"]).FontSizeSmall = 26;
                    ((Settings)Application.Current.Resources["Settings"]).FontSizeNormal = 28;
                    ((Settings)Application.Current.Resources["Settings"]).FontSizeTitle = 32;
                    break;
                case "Large":
                    ((Settings)Application.Current.Resources["Settings"]).FontSizeSmaller = 26;
                    ((Settings)Application.Current.Resources["Settings"]).FontSizeSmall = 28;
                    ((Settings)Application.Current.Resources["Settings"]).FontSizeNormal = 30;
                    ((Settings)Application.Current.Resources["Settings"]).FontSizeTitle = 34;
                    break;
            }
        }
    }
}
