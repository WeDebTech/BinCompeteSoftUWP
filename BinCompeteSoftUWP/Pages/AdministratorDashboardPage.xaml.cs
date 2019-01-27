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
            await RefreshUsers();

            await RefreshContests();

            await RefreshCriterias();

            await RefreshCategories();

            await Data.Instance.RefreshJudgesAsync();

            await Data.Instance.RefreshCategoriesAsync();
        }

        private async void RefreshContestsButton_Click(object sender, RoutedEventArgs e)
        {
            await RefreshContests();
        }

        private void AddCriteriaButton_Click(object sender, RoutedEventArgs e)
        {
            EditCriteriaContentDialog editCriteriaContentDialog = new EditCriteriaContentDialog(null);

            // Create callback to be called when ContentDialog closes.
            Action<ContentDialogResult> callback = async (result) =>
            {
                await RefreshCriterias();
            };

            App.ShowContentDialog(editCriteriaContentDialog, callback);
        }

        private async void RefreshCriteriasButton_Click(object sender, RoutedEventArgs e)
        {
            await RefreshCriterias();
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            EditUserContentDialog editUserContentDialog = new EditUserContentDialog(null);

            // Create callback to be called when ContentDialog closes.
            Action<ContentDialogResult> callback = async (result) =>
            {
                await RefreshUsers();
            };

            App.ShowContentDialog(editUserContentDialog, callback);
        }

        private async void RefreshUsersButton_Click(object sender, RoutedEventArgs e)
        {
            await RefreshUsers();
        }

        private void ShowContestDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            // Get which contest is associated with this grid.
            if (ContestsListView.SelectedItems.Count == 1)
            {
                ContestDetails contestDetails = (ContestDetails)ContestsListView.SelectedItems[0];


                this.Frame.Navigate(typeof(ContestPage), contestDetails);
            }
            else
            {
                ContentDialog errorMsg = new ContentDialog
                {
                    Title = "Error",
                    Content = "You must select a contest to view it's details!",
                    CloseButtonText = "OK"
                };

                App.ShowContentDialog(errorMsg, null);
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
            // Get which contest is associated with this grid.
            if (UsersListView.SelectedItems.Count == 1)
            {
                User user = (User)UsersListView.SelectedItems[0];

                EditUserContentDialog editUserContentDialog = new EditUserContentDialog(user);

                // Create callback to be called when ContentDialog closes.
                Action<ContentDialogResult> callback = async (result) =>
                {
                    await RefreshUsers();
                };

                App.ShowContentDialog(editUserContentDialog, callback);
            }
        }

        private void CriteriaGrid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            // Get which contest is associated with this grid.
            if (CriteriasListView.SelectedItems.Count == 1)
            {
                Criteria criteria = (Criteria)CriteriasListView.SelectedItems[0];


                EditCriteriaContentDialog editCriteriaContentDialog = new EditCriteriaContentDialog(criteria);

                // Create callback to be called when ContentDialog closes.
                Action<ContentDialogResult> callback = async (result) =>
                {
                    await RefreshCriterias();
                };

                App.ShowContentDialog(editCriteriaContentDialog, callback);
            }
        }

        private void ShowUserDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            // Get which contest is associated with this grid.
            if (UsersListView.SelectedItems.Count == 1)
            {
                User user = (User)ContestsListView.SelectedItems[0];

                EditUserContentDialog editUserContentDialog = new EditUserContentDialog(user);

                // Create callback to be called when ContentDialog closes.
                Action<ContentDialogResult> callback = async (result) =>
                {
                    await RefreshUsers();
                };

                App.ShowContentDialog(editUserContentDialog, callback);
            }
            else
            {
                ContentDialog errorMsg = new ContentDialog
                {
                    Title = "Error",
                    Content = "You must select a user to view it's details!",
                    CloseButtonText = "OK"
                };

                App.ShowContentDialog(errorMsg, null);
            }
        }

        private void ShowCriteriaDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            // Get which contest is associated with this grid.
            if (CriteriasListView.SelectedItems.Count == 1)
            {
                Criteria criteria = (Criteria)CriteriasListView.SelectedItems[0];


                EditCriteriaContentDialog editCriteriaContentDialog = new EditCriteriaContentDialog(criteria);

                // Create callback to be called when ContentDialog closes.
                Action<ContentDialogResult> callback = async (result) =>
                {
                    await RefreshCriterias();
                };

                App.ShowContentDialog(editCriteriaContentDialog, callback);
            }
            else
            {
                ContentDialog errorMsg = new ContentDialog
                {
                    Title = "Error",
                    Content = "You must select a criteria to view it's details!",
                    CloseButtonText = "OK"
                };

                App.ShowContentDialog(errorMsg, null);
            }
        }

        private void AddCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            EditCategoryContentDialog editCategoryContentDialog = new EditCategoryContentDialog(null);

            // Create callback to be called when ContentDialog closes.
            Action<ContentDialogResult> callback = async (result) =>
            {
                await RefreshCategories();
            };

            App.ShowContentDialog(editCategoryContentDialog, callback);
        }

        private async void RefreshCategoriesButton_Click(object sender, RoutedEventArgs e)
        {
            await RefreshCategories();
        }

        private void CategoryGrid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            // Get which contest is associated with this grid.
            if (CategoriesListView.SelectedItems.Count == 1)
            {
                Category category = (Category)CategoriesListView.SelectedItems[0];


                EditCategoryContentDialog editCategoryContentDialog = new EditCategoryContentDialog(category);

                // Create callback to be called when ContentDialog closes.
                Action<ContentDialogResult> callback = async (result) =>
                {
                    await RefreshCategories();
                };

                App.ShowContentDialog(editCategoryContentDialog, callback);
            }
        }

        private void ShowCategoryDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            // Get which contest is associated with this grid.
            if (CategoriesListView.SelectedItems.Count == 1)
            {
                Category category = (Category)CategoriesListView.SelectedItems[0];


                EditCategoryContentDialog editCategoryContentDialog = new EditCategoryContentDialog(category);

                // Create callback to be called when ContentDialog closes.
                Action<ContentDialogResult> callback = async (result) =>
                {
                    await RefreshCategories();
                };

                App.ShowContentDialog(editCategoryContentDialog, callback);
            }
            else
            {
                ContentDialog errorMsg = new ContentDialog
                {
                    Title = "Error",
                    Content = "You must select a category to view it's details!",
                    CloseButtonText = "OK"
                };

                App.ShowContentDialog(errorMsg, null);
            }
        }
        #endregion

        #region Class methods
        /// <summary>
        /// Loads all the users.
        /// </summary>
        private async Task RefreshUsers()
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
        private async Task RefreshContests()
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
        private async Task RefreshCriterias()
        {
            CriteriasListView.ItemsSource = null;

            LoadingCriteriasProgressRing.IsActive = true;
            LoadingCriteriasTextBlock.Visibility = Visibility.Visible;

            await Data.Instance.RefreshCriteriasAsync();

            LoadingCriteriasProgressRing.IsActive = false;
            LoadingCriteriasTextBlock.Visibility = Visibility.Collapsed;

            CriteriasListView.ItemsSource = Data.Instance.Criterias;
        }

        /// <summary>
        /// Loads all the categories.
        /// </summary>
        private async Task RefreshCategories()
        {
            CategoriesListView.ItemsSource = null;

            LoadingCategoriesProgressRing.IsActive = true;
            LoadingCategoriesTextBlock.Visibility = Visibility.Visible;

            await Data.Instance.RefreshCategoriesAsync();

            LoadingCategoriesProgressRing.IsActive = false;
            LoadingCategoriesTextBlock.Visibility = Visibility.Collapsed;

            CategoriesListView.ItemsSource = Data.Instance.Categories;
        }
        #endregion
    }
}
