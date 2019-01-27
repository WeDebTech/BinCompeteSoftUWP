using BinCompeteSoftUWP.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class AdministratorDashboardPage : Page
    {
        #region Class variables

        #endregion

        #region Class constructor
        public AdministratorDashboardPage()
        {
            this.InitializeComponent();
        }
        #endregion

        #region Class event handlers
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Load data to fill the lists.
            await LoadUsers();

            await LoadContests();

            await LoadCriterias();

            await Data.Instance.RefreshJudgesAsync();

            await Data.Instance.RefreshCategoriesAsync();
        }

        private async void RefreshContestsButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadContests();
        }

        private void AddCriteriaButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void RefreshCriteriasButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadCriterias();
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void RefreshUsersButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadUsers();
        }

        private void ShowContestDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            // Get which contest is associated with this grid.
            if (ContestsListView.SelectedItems.Count == 1)
            {
                ContestDetails contestDetails = (ContestDetails)ContestsListView.SelectedItems[0];


                this.Frame.Navigate(typeof(ContestPage), contestDetails);
            }
        }

        private void ContestGrid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            // Get which contest is associated with this grid.
            if (ContestsListView.SelectedItems.Count == 1)
            {
                ContestDetails contestDetails = (ContestDetails)ContestsListView.SelectedItems[0];

                this.Frame.Navigate(typeof(ContestPage), contestDetails);
            }
        }

        private void UserGrid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {

        }

        private void CriteriaGrid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {

        }
        #endregion

        #region Class methods
        /// <summary>
        /// Loads all the users.
        /// </summary>
        private async Task LoadUsers()
        {
            UsersListView.ItemsSource = null;

            LoadingUsersProgressRing.IsActive = true;
            LoadingUsersTextBlock.Visibility = Visibility.Visible;

            await Data.Instance.RefreshUsersAsync();

            LoadingUsersProgressRing.IsActive = false;
            LoadingUsersTextBlock.Visibility = Visibility.Collapsed;

            UsersListView.ItemsSource = Data.Instance.Users;
        }

        /// <summary>
        /// Loads all the contests.
        /// </summary>
        private async Task LoadContests()
        {
            ContestsListView.ItemsSource = null;

            LoadingContestProgressRing.IsActive = true;
            LoadingContestsTextBlock.Visibility = Visibility.Visible;

            await Data.Instance.RefreshContestsAdminAsync();

            LoadingContestProgressRing.IsActive = false;
            LoadingContestsTextBlock.Visibility = Visibility.Collapsed;

            ContestsListView.ItemsSource = Data.Instance.ContestDetails;
        }

        /// <summary>
        /// Loads all the criterias.
        /// </summary>
        private async Task LoadCriterias()
        {
            CriteriasListView.ItemsSource = null;

            LoadingCriteriasProgressRing.IsActive = true;
            LoadingCriteriasTextBlock.Visibility = Visibility.Visible;

            await Data.Instance.RefreshCriteriasAsync();

            LoadingCriteriasProgressRing.IsActive = false;
            LoadingCriteriasTextBlock.Visibility = Visibility.Collapsed;

            CriteriasListView.ItemsSource = Data.Instance.Criterias;
        }
        #endregion

        private void ShowUserDetailsButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
